using System.Security.Cryptography;
using System.Text;

namespace Mitochondria.Framework.Utilities.Extensions;

public static class StringExtensions
{
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
}