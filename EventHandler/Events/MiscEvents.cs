namespace RadiantConnect.EventHandler.Events
{
	public class MiscEvents
	{
		public delegate void MiscEvent();
	
		public event MiscEvent? OnHeartbeat;

		public void HandleInGameEvent(string invoker, string _)
		{
			switch (invoker)
			{
				case "Session_Heartbeat":
					OnHeartbeat?.Invoke();
					break;
			}
		}
	}
}