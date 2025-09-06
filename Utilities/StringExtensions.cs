namespace RadiantConnect.Utilities
{
	internal static class StringExtensions
	{
		internal static string ExtractValue(this string haystack, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern, int groupId)
		{
			Match match = Match(haystack, pattern);
			return match is not { Success: true } ? "" : match.Groups[groupId].Value.Replace("\r", "").Replace("\n", "").Trim();
		}

		internal static string TryExtractSubstring(this string log, string startToken, char endToken, Func<int, bool> condition, string prefix = " ")
		{
			int startIndex = log.IndexOf(startToken, StringComparison.Ordinal);
			int endIndex = log.IndexOf(endToken, startIndex);
			return startIndex != -1 && endIndex != -1 && condition(startIndex) ? log[startIndex..(endIndex - startIndex)].Replace(prefix, "") : "";
		}

		internal static string FromBase64(this string value)
		{
			if (string.IsNullOrEmpty(value)) return string.Empty;

			try
			{
				if (value.Contains('<')) value = value[..value.IndexOf('<')];

				int paddingNeeded = value.Length % 4;
				if (paddingNeeded > 0) value = value.PadRight(value.Length + (4 - paddingNeeded), '=');

				byte[] buffer = Convert.FromBase64String(value);
				return Encoding.UTF8.GetString(buffer);
			}
			catch { return string.Empty; }
		}

		internal static string ToBase64(this string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

		internal static bool IsNullOrEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);
	}
}