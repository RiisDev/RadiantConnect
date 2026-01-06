using System.Security.Claims;

namespace RadiantConnect.Utilities
{
	/// <summary>
	/// Represents a parsed JSON Web Token (JWT) and provides strongly-typed
	/// access to its header, payload, and claims.
	/// </summary>
	/// <remarks>
	/// This class does not perform signature validation. It is intended for
	/// inspecting JWT contents such as claims, timestamps, and metadata
	/// after the token has been obtained from a trusted source.
	/// </remarks>
	public class JsonWebToken
	{
		private readonly JsonElement _payloadElement;
		private readonly JsonElement _headerElement;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWebToken"/> class
		/// from a raw JWT string.
		/// </summary>
		/// <param name="jwt">
		/// The raw JWT string in compact serialization format.
		/// </param>
		public JsonWebToken(string? jwt)
		{
			if (string.IsNullOrWhiteSpace(jwt))
				throw new ArgumentException("JWT cannot be null or whitespace.", nameof(jwt));

			string[] parts = jwt.Split('.');
			if (parts.Length != 3)
				throw new ArgumentException("JWT must have three parts separated by '.'", nameof(jwt));

			_headerElement = ParseJsonElement(DecodeBase64Url(parts[0]), "header");
			_payloadElement = ParseJsonElement(DecodeBase64Url(parts[1]), "payload");
		}

		private static JsonElement ParseJsonElement(string json, string partName)
		{
			try
			{
				using JsonDocument doc = JsonDocument.Parse(json);
				return doc.RootElement.Clone();
			}
			catch (JsonException ex)
			{
				throw new InvalidOperationException($"Failed to parse JWT {partName}.", ex);
			}
		}

		/// <summary>
		/// Gets the signing algorithm specified in the JWT header.
		/// </summary>
		public string? Algorithm => GetHeaderValue<string>("alg");

		/// <summary>
		/// Gets the token type specified in the JWT header.
		/// </summary>
		public string? Type => GetHeaderValue<string>("typ");

		/// <summary>
		/// Gets the key identifier used to sign the JWT.
		/// </summary>
		public string? KeyId => GetHeaderValue<string>("kid");

		/// <summary>
		/// Gets the subject (<c>sub</c>) claim identifying the token principal.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the subject claim is not present.
		/// </exception>
		public string Subject => GetPayloadValue<string>("sub") ?? throw new InvalidOperationException("Subject (sub) not found in JWT.");

		/// <summary>
		/// Gets the issuer (<c>iss</c>) claim identifying the token issuer.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the issuer claim is not present.
		/// </exception>
		public string Issuer => GetPayloadValue<string>("iss") ?? throw new InvalidOperationException("Issuer (iss) not found in JWT.");

		/// <summary>
		/// Gets the audience (<c>aud</c>) claim identifying the intended recipients.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the audience claim is not present.
		/// </exception>
		public string Audience => GetPayloadValue<string>("aud") ?? throw new InvalidOperationException("Audience (aud) not found in JWT.");

		/// <summary>
		/// Gets the UTC expiration time (<c>exp</c>) of the token, if present.
		/// </summary>
		public DateTime? Expires => GetPayloadValue<long?>("exp") is { } seconds ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime : null;

		/// <summary>
		/// Gets the UTC expiration time of the token.
		/// </summary>
		/// <remarks>
		/// This property is an alias for <see cref="Expires"/>.
		/// </remarks>
		public DateTime? ExpiresAt => Expires;

		/// <summary>
		/// Gets the UTC issue time (<c>iat</c>) of the token, if present.
		/// </summary>
		public DateTime? IssuedAt => GetPayloadValue<long?>("iat") is { } seconds ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime : null;

		/// <summary>
		/// Gets the UTC time before which the token is not valid (<c>nbf</c>), if present.
		/// </summary>
		public DateTime? NotBefore => GetPayloadValue<long?>("nbf") is { } seconds ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime : null;

		/// <summary>
		/// Gets a value indicating whether the token is expired or otherwise invalid
		/// based on its time-based claims.
		/// </summary>
		/// <remarks>
		/// A small clock skew is tolerated. Tokens lacking both expiration and
		/// issuance times are considered expired.
		/// </remarks>
		public bool IsExpired
		{
			get
			{
				DateTime expiresAt = Expires.GetValueOrDefault();
				DateTime issuedAt = IssuedAt.GetValueOrDefault();
				DateTime now = DateTime.UtcNow.AddSeconds(-5);

				if (expiresAt == default && issuedAt == default)
					return true;

				if (expiresAt == default && issuedAt <= now)
					return true;

				return expiresAt != default && expiresAt <= now;
			}
		}

		/// <summary>
		/// Retrieves all claims contained in the JWT payload.
		/// </summary>
		/// <returns>
		/// An array of <see cref="Claim"/> objects representing the token claims.
		/// </returns>
		public Claim[] GetClaims()
		{
			List<Claim> claims = [];
			AddClaimsFromElement(_payloadElement, claims, "");
			return [.. claims];
		}

		private static void AddClaimsFromElement(JsonElement element, List<Claim> claims, string prefix)
		{
			switch (element.ValueKind)
			{
				case JsonValueKind.Object:
				{
					foreach (JsonProperty property in element.EnumerateObject())
					{
						string claimType = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
						AddClaimsFromElement(property.Value, claims, claimType);
					}
					break;
				}
				case JsonValueKind.Array:
					claims.AddRange(element.EnumerateArray().Select(item => new Claim(prefix, item.ToString())));
					break;
				case JsonValueKind.Undefined:
				case JsonValueKind.String:
				case JsonValueKind.Number:
				case JsonValueKind.True:
				case JsonValueKind.False:
				case JsonValueKind.Null:
				default:
					claims.Add(new Claim(prefix, element.ToString()));
					break;
			}
		}

		/// <summary>
		/// Retrieves a claim value as a string.
		/// </summary>
		/// <param name="claimType">
		/// The claim type or JSON path.
		/// </param>
		/// <returns>
		/// The claim value as a string, or an empty string if the claim is not present.
		/// </returns>
		public string GetClaim(string claimType) =>
			TryGetPayloadElement(claimType, out JsonElement element)
				? element.ValueKind switch
				{
					JsonValueKind.String => element.GetString() ?? string.Empty,
					JsonValueKind.Number => element.ToString(),
					JsonValueKind.True or JsonValueKind.False => element.GetBoolean().ToString(),
					_ => element.GetRawText()
				}
				: string.Empty;

		/// <summary>
		/// Retrieves a value from the JWT header.
		/// </summary>
		/// <typeparam name="T">
		/// The expected value type.
		/// </typeparam>
		/// <param name="key">
		/// The header key.
		/// </param>
		/// <returns>
		/// The deserialized header value, or <c>null</c> if not present.
		/// </returns>
		public T? GetHeaderValue<T>(string key) =>
			_headerElement.ValueKind == JsonValueKind.Object &&
			_headerElement.TryGetProperty(key, out JsonElement element)
				? JsonSerializer.Deserialize<T>(element.GetRawText())
				: default;

		/// <summary>
		/// Retrieves a value from the JWT payload using a JSON path.
		/// </summary>
		/// <typeparam name="T">
		/// The expected value type.
		/// </typeparam>
		/// <param name="keyPath">
		/// The payload property name or JSON path.
		/// </param>
		/// <returns>
		/// The deserialized payload value, or <c>null</c> if not present.
		/// </returns>
		public T? GetPayloadValue<T>(string keyPath) => TryGetPayloadElement(keyPath, out JsonElement element) ? JsonSerializer.Deserialize<T>(element.GetRawText()) : default;
		
		/// <summary>
		/// Retrieves a required value from the JWT header.
		/// </summary>
		/// <typeparam name="T">
		/// The expected value type.
		/// </typeparam>
		/// <param name="key">
		/// The header key.
		/// </param>
		/// <returns>
		/// The deserialized header value.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the required header value is not present.
		/// </exception>
		public T GetRequiredHeaderValue<T>(string key) => GetHeaderValue<T>(key) ?? throw new InvalidOperationException($"Required header value '{key}' not found in JWT.");
		
		/// <summary>
		/// Retrieves a required value from the JWT payload.
		/// </summary>
		/// <typeparam name="T">
		/// The expected value type.
		/// </typeparam>
		/// <param name="keyPath">
		/// The payload property name or JSON path.
		/// </param>
		/// <returns>
		/// The deserialized payload value.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the required payload value is not present.
		/// </exception>
		public T GetRequiredPayloadValue<T>(string keyPath) => GetPayloadValue<T>(keyPath) ?? throw new InvalidOperationException($"Required payload value '{keyPath}' not found in JWT.");

		[SuppressMessage("ReSharper", "RemoveRedundantBraces")]
		private bool TryGetPayloadElement(string keyPath, out JsonElement element)
		{
			element = _payloadElement;

			if (_payloadElement.ValueKind != JsonValueKind.Object)
				return false;

			string[] keys = keyPath.Split('.');
			JsonElement current = _payloadElement;

			foreach (string key in keys)
			{
				if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(key, out JsonElement next)) current = next;
				else { current = default; break; }
			}

			if (current.ValueKind != JsonValueKind.Undefined) { element = current; return true; }

			// Try fallback of full path
			// Should fix issue with key 'desired.affinity' not being found
			if (_payloadElement.TryGetProperty(keyPath, out JsonElement fallback)) { element = fallback; return true; }

			element = default;
			return false;
		}
		
		private static string DecodeBase64Url(string input)
		{
			input = input.Replace('-', '+').Replace('_', '/');

			int padLength = 4 - (input.Length % 4);
			if (padLength < 4)
				input += new string('=', padLength);

			try
			{
				return input.FromBase64();
			}
			catch (FormatException ex)
			{
				throw new ArgumentException("Invalid Base64Url string in JWT.", ex);
			}
		}
	}
}
