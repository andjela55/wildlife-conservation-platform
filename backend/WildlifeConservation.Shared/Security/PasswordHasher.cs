using System.Security.Cryptography;
using System.Text;

namespace WildlifeConservation.Shared.Security;

public static class PasswordHasher
{
    private const int Iterations = 100_000;
    private const int KeySize = 32;

    public static string CreateSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
    }

    public static string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);

        return Convert.ToBase64String(pbkdf2.GetBytes(KeySize));
    }

    public static bool VerifyPassword(string password, string salt, string expectedHash)
    {
        var passwordHash = HashPassword(password, salt);
        var passwordHashBytes = Encoding.UTF8.GetBytes(passwordHash);
        var expectedHashBytes = Encoding.UTF8.GetBytes(expectedHash);

        return CryptographicOperations.FixedTimeEquals(passwordHashBytes, expectedHashBytes);
    }
}
