namespace RadiantConnect.Authentication.DriverRiotAuth.Records
{
	/// <summary>
	/// Represents a Riot authentication or session cookie with full metadata as returned by Riot services.
	/// </summary>
	/// <param name="Name">The name of the cookie.</param>
	/// <param name="Value">The value assigned to the cookie.</param>
	/// <param name="Domain">The domain to which the cookie applies.</param>
	/// <param name="Path">The path for which the cookie is valid.</param>
	/// <param name="Expires">The expiration timestamp of the cookie, if applicable.</param>
	/// <param name="Size">The size of the cookie in bytes.</param>
	/// <param name="HttpOnly">Indicates whether the cookie is HTTP-only.</param>
	/// <param name="Secure">Indicates whether the cookie is restricted to secure protocols.</param>
	/// <param name="Session">Whether the cookie is a session cookie.</param>
	/// <param name="Priority">The priority level assigned to the cookie.</param>
	/// <param name="SameParty">Whether the cookie belongs to a same-party context.</param>
	/// <param name="SourceScheme">The URL scheme that created the cookie (e.g., https).</param>
	/// <param name="SourcePort">The originating port associated with the cookie.</param>
	/// <param name="SameSite">The SameSite attribute of the cookie.</param>
	/// <param name="PartitionKey">The partition key describing how the cookie is partitioned.</param>
	public record Cookie(
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("value")] string Value,
		[property: JsonPropertyName("domain")] string Domain,
		[property: JsonPropertyName("path")] string Path,
		[property: JsonPropertyName("expires")] double? Expires,
		[property: JsonPropertyName("size")] int? Size,
		[property: JsonPropertyName("httpOnly")] bool? HttpOnly,
		[property: JsonPropertyName("secure")] bool? Secure,
		[property: JsonPropertyName("session")] bool? Session,
		[property: JsonPropertyName("priority")] string Priority,
		[property: JsonPropertyName("sameParty")] bool? SameParty,
		[property: JsonPropertyName("sourceScheme")] string SourceScheme,
		[property: JsonPropertyName("sourcePort")] int? SourcePort,
		[property: JsonPropertyName("sameSite")] string SameSite,
		[property: JsonPropertyName("partitionKey")] PartitionKey PartitionKey
	);

	/// <summary>
	/// Represents the partition key metadata associated with a cookie, including top-level site origin 
	/// and cross-site behavior.
	/// </summary>
	/// <param name="TopLevelSite">The top-level site associated with the partition.</param>
	/// <param name="HasCrossSiteAncestor">Whether the partition has a cross-site ancestor.</param>
	public record PartitionKey(
		[property: JsonPropertyName("topLevelSite")] string TopLevelSite,
		[property: JsonPropertyName("hasCrossSiteAncestor")] bool? HasCrossSiteAncestor
	);

	internal record Result(
		[property: JsonPropertyName("cookies")] IReadOnlyList<Cookie> Cookies
	);

	internal record CookieRoot(
		[property: JsonPropertyName("id")] int? Id,
		[property: JsonPropertyName("result")] Result Result
	);
}
