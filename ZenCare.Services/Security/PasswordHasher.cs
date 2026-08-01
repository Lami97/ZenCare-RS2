using System.Security.Cryptography;
using System.Text;

namespace ZenCare.Services.Security;

public static class PasswordHasher
{
    public static string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[16];
        rng.GetBytes(saltBytes);

        return Convert.ToBase64String(saltBytes);
    }

    public static string GenerateHash(string password, string salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(20);

        return Convert.ToBase64String(hash);
    }
}
