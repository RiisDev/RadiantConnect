namespace RadiantConnect.Authentication.DriverRiotAuth.Records
{
	// ReSharper disable twice StringLiteralTypo
	/// <summary>
	/// Defines configuration options for the browser driver used during driver-based authentication.
	/// </summary>
	/// <param name="ProcessName">The name of the browser process to automate.</param>
	/// <param name="BrowserExecutable">The full path to the browser executable.</param>
	/// <param name="KillBrowser">Whether to terminate the browser process after authentication.</param>
	/// <param name="CacheCookies">Whether to store cookies for future authentication attempts.</param>
	/// <param name="UseHeadless">Whether to run the browser in headless mode during authentication.</param>
	public record DriverSettings(
		string ProcessName = "msedge",
		string BrowserExecutable = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
		bool KillBrowser = false,
		bool CacheCookies = true,
		bool UseHeadless = true
	);

	/// <summary>
	/// Represents the full set of Riot Sign-On (RSO) authentication tokens, cookies, and metadata 
	/// returned after a successful authentication flow.
	/// </summary>
	/// <param name="Subject">The authenticated player's subject identifier.</param>
	/// <param name="Ssid">The SSID session token.</param>
	/// <param name="Tdid">The TDID device identifier.</param>
	/// <param name="Csid">The CSID session identifier.</param>
	/// <param name="Clid">The CLID login identifier.</param>
	/// <param name="AccessToken">The OAuth access token.</param>
	/// <param name="PasToken">The PAS token used for RSO services.</param>
	/// <param name="Entitlement">The entitlement token used for Valorant services.</param>
	/// <param name="Affinity">The region affinity value for game services.</param>
	/// <param name="ChatAffinity">The affinity value for chat services.</param>
	/// <param name="ClientConfig">Raw client configuration data from Riot APIs.</param>
	/// <param name="RiotCookies">The list of Riot cookies returned during authentication.</param>
	/// <param name="IdToken">The identity token associated with the login session.</param>
	public record RSOAuth(
		string? Subject,
		string? Ssid,
		string? Tdid,
		string? Csid,
		string? Clid,
		string? AccessToken,
		string? PasToken,
		string? Entitlement,
		string? Affinity,
		string? ChatAffinity,
		object? ClientConfig,
		IEnumerable<Cookie>? RiotCookies,
		string? IdToken
	)
	{
		/// <summary>
		/// Gets or sets the RMS token used for inventory and store-related Riot services.
		/// </summary>
		public string? RmsToken { get; set; }
	};
}