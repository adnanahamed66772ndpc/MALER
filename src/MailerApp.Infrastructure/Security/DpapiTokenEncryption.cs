using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using MailerApp.Domain.Interfaces;

namespace MailerApp.Infrastructure.Security;

/// <summary>Encrypts/decrypts using Windows DPAPI (Data Protection API). Falls back to a fixed app key on non-Windows or if DPAPI fails.</summary>
public class DpapiTokenEncryption : ITokenEncryption
{
    private static readonly byte[] Entropy = [0x4d, 0x61, 0x69, 0x6c, 0x65, 0x72, 0x41, 0x70, 0x70]; // "MailerApp"

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var bytes = Encoding.UTF8.GetBytes(plainText);
                var encrypted = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encrypted);
            }
        }
        catch { /* fallback */ }
        return EncryptFallback(plainText);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return string.Empty;
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var bytes = Convert.FromBase64String(cipherText);
                var decrypted = ProtectedData.Unprotect(bytes, Entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted);
            }
        }
        catch { /* fallback */ }
        return DecryptFallback(cipherText);
    }

    private static string EncryptFallback(string plainText)
    {
        var key = Encoding.UTF8.GetBytes("MailerApp.Secret.Key.ChangeInProduction!");
        var bytes = Encoding.UTF8.GetBytes(plainText);
        for (var i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)(bytes[i] ^ key[i % key.Length]);
        return Convert.ToBase64String(bytes);
    }

    private static string DecryptFallback(string cipherText)
    {
        var key = Encoding.UTF8.GetBytes("MailerApp.Secret.Key.ChangeInProduction!");
        var bytes = Convert.FromBase64String(cipherText);
        for (var i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)(bytes[i] ^ key[i % key.Length]);
        return Encoding.UTF8.GetString(bytes);
    }
}
