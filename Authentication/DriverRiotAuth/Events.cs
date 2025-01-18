using static RadiantConnect.Authentication.Authentication;

namespace RadiantConnect.Authentication.DriverRiotAuth
{
    public static class Events
    {

        internal delegate void RadiantConsoleDetected(string? data = null);
        internal delegate void RuntimeChanged();
        internal delegate void FrameChangedEvent(string? url, string frameId);

        public delegate void MultiFactorEvent();

        public delegate void DriverEvent(DriverStatus status);

    }
}
