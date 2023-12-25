using System.Text.Json;
using static System.String;
// ReSharper disable All

namespace RadiantConnect.Network.PartyEndpoints;

public class AuthEndpoints
{
    internal static ValorantNet Net = Initiator.InternalSystem.Net;

    internal static async Task<T?> GetAsync<T>(string baseUrl, string endPoint)
    {
        string? jsonData = await Net.GetAsync(baseUrl, endPoint);

        return IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
    }

    public class Shared
    {
        internal static string Url = Initiator.InternalSystem.ClientData.SharedUrl;

    }

    public class Pd
    {
        internal static string Url = Initiator.InternalSystem.ClientData.PdUrl;

    }

    public class Glz
    {
        internal static string Url = Initiator.InternalSystem.ClientData.SharedUrl;

    }
}