using System.Security.Cryptography;

namespace Boucher_Double_Back_End.Models.ServerModel
{
    /// <summary>
    /// Class that hash password before sending them to the database
    /// </summary>
    public class Security
    {
        /// <summary>
        /// Parameter of the salt
        /// </summary>
        private const int SaltSize = 16; //128 / 8, length in bytes
        /// <summary>
        /// Parameter of the salt
        /// </summary>
        private const int KeySize = 32; //256 / 8, length in bytes
        /// <summary>
        /// Parameter of the salt
        /// </summary>
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private const char SaltDelimeter = ';';
        /// <summary>
        /// Return the modified password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);
            return string.Join(SaltDelimeter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }
        /// <summary>
        /// Check if a password correspond to what is stored in the database
        /// </summary>
        /// <param name="passwordHash"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool Validate(string passwordHash, string password)
        {
            string[] pwdElements = passwordHash.Split(SaltDelimeter);
            byte[] salt = Convert.FromBase64String(pwdElements[0]);
            byte[] hash = Convert.FromBase64String(pwdElements[1]);
            byte[] hashInput = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);
            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }

        /// <summary>
        /// Generate a unique CSRF token to securise the application
        /// </summary>
        /// <returns>The CSRF token send to the client</returns>
        public static string GenerateCSRFToken()
        {
            byte[] tokenBytes = new byte[32]; // Generate a 32-byte CSRF token
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            string csrfToken = Convert.ToBase64String(tokenBytes);
            return csrfToken;
        }
    }
}
