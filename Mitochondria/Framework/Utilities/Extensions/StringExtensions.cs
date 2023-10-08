using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Mitochondria.Framework.Utilities.Extensions;

public static class StringExtensions
{
    private static readonly Regex WhitespaceRegex;

    static StringExtensions()
    {
        WhitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
    }

    public static byte[] Get64HashCode(this string value)
    {
        return Get128HashCode(value)[..8].ToArray();
    }
    
    public static byte[] Get128HashCode(this string value)
    {
        using var hasher = MD5.Create();
        var valueBytes = Encoding.UTF8.GetBytes(value);
        var hashBytes = hasher.ComputeHash(valueBytes);

        return hashBytes;
    }

    public static string RemoveWhitespace(this string input)
        => WhitespaceRegex.Replace(input, string.Empty);

    public static string ReplaceWhitespace(this string input, string replacement)
        => WhitespaceRegex.Replace(input, replacement);
}