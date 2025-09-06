using System.Security.Claims;

namespace RadiantConnect.Utilities
{
    public class JsonWebToken
    {
        private readonly JsonElement _payloadElement;
        private readonly JsonElement _headerElement;

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

        public string? Algorithm => GetHeaderValue<string>("alg");
        public string? Type => GetHeaderValue<string>("typ");
        public string? KeyId => GetHeaderValue<string>("kid");

        public string Subject => GetPayloadValue<string>("sub") ?? throw new InvalidOperationException("Subject (sub) not found in JWT.");
        public string Issuer => GetPayloadValue<string>("iss") ?? throw new InvalidOperationException("Issuer (iss) not found in JWT.");
        public string Audience => GetPayloadValue<string>("aud") ?? throw new InvalidOperationException("Audience (aud) not found in JWT.");

        public DateTime? Expires => GetPayloadValue<long?>("exp") is { } seconds ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime : null;
        public DateTime? ExpiresAt => Expires;
        public DateTime? IssuedAt => GetPayloadValue<long?>("iat") is { } seconds ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime : null;
        public DateTime? NotBefore => GetPayloadValue<long?>("nbf") is { } seconds ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime : null;

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

				if (expiresAt != default && expiresAt <= now)
					return true;

				return false;
			}
        }

		public Claim[] GetClaims()
        {
            List<Claim> claims = [];
            AddClaimsFromElement(_payloadElement, claims, "");
            return [..claims];
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

        public T? GetHeaderValue<T>(string key) =>
	        _headerElement.ValueKind == JsonValueKind.Object &&
	        _headerElement.TryGetProperty(key, out JsonElement element)
		        ? JsonSerializer.Deserialize<T>(element.GetRawText())
		        : default;

        public T? GetPayloadValue<T>(string keyPath) => TryGetPayloadElement(keyPath, out JsonElement element) ? JsonSerializer.Deserialize<T>(element.GetRawText()) : default;

        [SuppressMessage("ReSharper", "RemoveRedundantBraces")]
        private bool TryGetPayloadElement(string keyPath, out JsonElement element)
        {
            element = _payloadElement;
            if (_payloadElement.ValueKind != JsonValueKind.Object)
                return false;

            string[] keys = keyPath.Split('.');
            foreach (string key in keys)
            {
                if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(key, out JsonElement next))
                {
                    element = next;
                }
                else
                {
                    element = default;
                    return false;
                }
            }
            return true;
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
