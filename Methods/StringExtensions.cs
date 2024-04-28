using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace RadiantConnect.Methods;

public static class StringExtensions
{
    internal static string ExtractValue(this string haystack, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern, int groupId)
    {
        Match match = Regex.Match(haystack, pattern);
        return match is not { Success: true } ? "" : match.Groups[groupId].Value.Replace("\r", "").Replace("\n", "");
    }

    internal static string TryExtractSubstring(this string log, string startToken, char endToken, Func<int, bool> condition, string prefix = " ")
    {
        int startIndex = log.IndexOf(startToken, StringComparison.Ordinal);
        int endIndex = log.IndexOf(endToken, startIndex);
        return startIndex != -1 && endIndex != -1 && condition(startIndex) ? log.Substring(startIndex, endIndex - startIndex).Replace(prefix, "") : "";
    }

    internal static string FromBase64(this string value)
    {
        try
        {
            if (value.Contains('<')) value = value[..value.IndexOf('<')];
            return Encoding.ASCII.GetString(Convert.FromBase64String(value));
        }
        catch
        {
            return "";
        }

    }

    internal static string ToBase64(this string value)
    {
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(value));
    }
}