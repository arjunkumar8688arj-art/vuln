using System.Security.Cryptography;
using System.Text;

namespace BillingApi.Services;

/// <summary>
/// Hashes partner API keys before they are stored in the PartnerKeys table.
/// </summary>
public class ApiKeyHasher
{
    public string Hash(string apiKey)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public bool Verify(string apiKey, string storedHash) =>
        CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(Hash(apiKey)),
            Encoding.UTF8.GetBytes(storedHash));

    /// <summary>Generates a new partner key using the OS CSPRNG.</summary>
    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}
