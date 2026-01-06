using static RadiantConnect.Authentication.Authentication;

namespace RadiantConnect.Authentication.DriverRiotAuth
{
	/// <summary>
	/// Provides public event delegate types used for authentication-related callbacks.
	/// </summary>
	public static class Events
	{
		internal delegate void RadiantConsoleDetected(string? data = null);
		internal delegate void RuntimeChanged();
		internal delegate void FrameChangedEvent(string? url, string frameId);

		/// <summary>
		/// Represents a callback invoked when multi-factor authentication is requested.
		/// </summary>
		public delegate void MultiFactorEvent();

		/// <summary>
		/// Represents a callback invoked when the driver reports an updated status during authentication.
		/// </summary>
		/// <param name="status">The current driver status.</param>
		public delegate void DriverEvent(DriverStatus status);
	}
}
