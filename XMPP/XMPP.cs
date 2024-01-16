using RadiantConnect.Services;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
// Credit to https://github.com/molenzwiebel/Deceive for guide

namespace RadiantConnect.XMPP
{
    public class XMPP
    {
        public static void KillRiot()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes.Where(proc => proc.ProcessName is "Riot Client" or "VALORANT")) process.Kill();
        }

        internal static bool IsRiotRunning()
        {
            Process[] processes = Process.GetProcesses();
            return processes.Any(proc => proc.ProcessName is "Riot Client" or "VALORANT");
        }

        internal static (TcpListener, int) NewTcpListener()
        {
            TcpListener listener = new(IPAddress.Loopback, 0);
            listener.Start();
            return (listener, ((IPEndPoint)listener.LocalEndpoint).Port);
        }

        public void InitializeConnection()
        {
            string valorantPath = ValorantService.GetValorantPath();
            if (IsRiotRunning()) throw new Exception("Riot/Valorant cannot be running.");
            if (!File.Exists(valorantPath)) throw new Exception($"Valorant executable not found: {valorantPath}");

            (TcpListener currentTcpListener, int currentPort) = NewTcpListener();


        }
    }
}
