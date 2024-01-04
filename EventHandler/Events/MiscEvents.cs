namespace RadiantConnect.EventHandler.Events;

public class MiscEvents
{
    public delegate void MiscEvent();
    
    public event MiscEvent? OnHeartbeat;

    public void HandleInGameEvent(string invoker, string logData)
    {
        switch (invoker)
        {
            case "Session_Heartbeat":
                OnHeartbeat?.Invoke();
                break;
        }
    }
}