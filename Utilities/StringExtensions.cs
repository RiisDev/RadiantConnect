using System.Globalization;

namespace RadiantConnect.Utilities
{
	internal static class StringExtensions
	{
		internal static CultureInfo CultureInfo { get; } = new ("en-us");

        extension(string haystack)
        {
	        internal string ExtractValue([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, int groupId)
	        {
		        Match match = Match(haystack, pattern);
		        return match is not { Success: true } ? "" : match.Groups[groupId].Value.Replace("\r", "", StringComparison.InvariantCultureIgnoreCase).Replace("\n", "", StringComparison.InvariantCultureIgnoreCase).Trim();
	        }

	        internal string TryExtractSubstring(string startToken, char endToken, Func<int, bool> condition, string prefix = " ")
	        {
		        int startIndex = haystack.IndexOf(startToken, StringComparison.Ordinal);
		        int endIndex = haystack.IndexOf(endToken, startIndex);
		        return startIndex != -1 && endIndex != -1 && condition(startIndex) ? haystack[startIndex..(endIndex - startIndex)].Replace(prefix, "", StringComparison.InvariantCultureIgnoreCase) : "";
	        }

	        internal string FromBase64()
	        {
		        if (string.IsNullOrEmpty(haystack)) return string.Empty;

		        try
		        {
			        if (haystack.Contains('<', StringComparison.InvariantCultureIgnoreCase)) haystack = haystack[..haystack.IndexOf('<', StringComparison.InvariantCultureIgnoreCase)];

			        int paddingNeeded = haystack.Length % 4;
			        if (paddingNeeded > 0) haystack = haystack.PadRight(haystack.Length + (4 - paddingNeeded), '=');

			        byte[] buffer = Convert.FromBase64String(haystack);
			        return Encoding.UTF8.GetString(buffer);
		        }
		        catch { return string.Empty; }
	        }

	        internal string ToBase64() => Convert.ToBase64String(Encoding.UTF8.GetBytes(haystack));
        }

        internal static bool IsNullOrEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);
	}
}