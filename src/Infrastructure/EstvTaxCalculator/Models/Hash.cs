using System.Security.Cryptography;
using System.Text;

internal static class Hash
{
    public static string Compute(string value)
    {
        var hash = SHA1.HashData(Encoding.UTF8.GetBytes(value));
        
        return string.Concat(hash.Select(b => b.ToString("x2")));
    }
}
