using Messenger.Api.Areas.Cryptography.Abstractions;
using Messenger.Api.Areas.Cryptography.Models;
using System.Security.Cryptography;
using System.Text;

namespace Messenger.Api.Areas.Cryptography.Services
{
    public class PasswordChecker : IPasswordChecker
    {
        private readonly int _keySize;
        private readonly int _iterations;
        private readonly HashAlgorithmName _algo;

        public PasswordChecker(IConfiguration configuration)
        {
            _keySize = int.Parse(configuration["PasswordHasher:KeySize"]!);
            _iterations = int.Parse(configuration["PasswordHasher:Iterations"]!);
            _algo = GetHashAlgorithm(configuration["PasswordHasher:HashAlgorithm"]!);
        }

        public bool CheckPassword(string userPassword, HashedPassword hashedPassword)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(userPassword);
            byte[] hash = Convert.FromHexString(hashedPassword.Hash);
            byte[] salt = Convert.FromHexString(hashedPassword.Salt);
            byte[] comparison = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, _iterations, _algo, _keySize);
            bool isValid = CryptographicOperations.FixedTimeEquals(hash, comparison);
            return isValid;
        }

        private HashAlgorithmName GetHashAlgorithm(string hashAlgorithm)
        {
            return hashAlgorithm switch
            {
                "SHA1" => HashAlgorithmName.SHA1,
                "SHA256" => HashAlgorithmName.SHA256,
                "SHA384" => HashAlgorithmName.SHA384,
                "SHA512" => HashAlgorithmName.SHA512,
                _ => throw new NotSupportedException($"Hash algorithm '{hashAlgorithm}' is not supported.")
            };
        }
    }
}
