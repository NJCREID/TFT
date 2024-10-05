using System.Security.Cryptography;
using TFT_API.Interfaces;

namespace TFT_API.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int Iterations = 10000;
        private const int SaltSize = 128 / 8;
        private const int HashSize = 256 / 8;
        private const char Delimiter = ';';

        // Method to hash a password
        public string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

            return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        // Method to verify a password against a hashed password
        public bool VerifyPassword(string password, string hashedPassword)
        {
            var elements = hashedPassword.Split(Delimiter);
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);

            var hashInput = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}
