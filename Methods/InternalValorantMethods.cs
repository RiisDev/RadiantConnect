using System.Diagnostics;
namespace RadiantConnect.Methods;
// ReSharper disable All

public class InternalValorantMethods
{
    public static bool IsValorantProcessRunning() { return Process.GetProcessesByName("VALORANT").Length > 0; }
}
