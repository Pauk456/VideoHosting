using System.Security.Cryptography;

namespace AuthorizationService.Controllers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = algorithm.GetBytes(HashSize);
            byte[] salt = algorithm.Salt;

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using var algorithm = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = algorithm.GetBytes(HashSize);

            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }
            return true;
        }
    }
}
