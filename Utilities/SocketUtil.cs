namespace RadiantConnect.Utilities
{
	internal static class SocketUtil
	{
		internal static string GetUnixTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

		internal static bool IsValidGuid([StringSyntax(StringSyntaxAttribute.GuidFormat)] string guid) => Guid.TryParse(guid, out Guid _) && guid.Contains('-');
	}
}
