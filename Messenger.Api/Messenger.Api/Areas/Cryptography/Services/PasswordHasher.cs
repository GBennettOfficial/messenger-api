using Messenger.Api.Areas.Cryptography.Abstractions;
using Messenger.Api.Areas.Cryptography.Models;
using System.Security.Cryptography;
using System.Text;

namespace Messenger.Api.Areas.Cryptography.Services
{

    public class PasswordHasher : IPasswordHasher
    {
        private readonly int _keySize;
        private readonly int _saltSize;
        private readonly int _iterations;
        private readonly HashAlgorithmName _algo;

        public PasswordHasher(IConfiguration configuration)
        {
            _keySize = int.Parse(configuration["PasswordHasher:KeySize"]!);
            _saltSize = int.Parse(configuration["PasswordHasher:SaltSize"]!);
            _iterations = int.Parse(configuration["PasswordHasher:Iterations"]!);
            _algo = GetHashAlgorithm(configuration["PasswordHasher:HashAlgorithm"]!);
        }

        public HashedPassword HashPassword(string userPassword)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(userPassword);
            byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, _iterations, _algo, _keySize);
            string saltString = Convert.ToHexString(salt);
            string hashString = Convert.ToHexString(hash);
            return new HashedPassword(hashString, saltString);
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
