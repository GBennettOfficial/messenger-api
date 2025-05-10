using Messenger.Api.Areas.Cryptography.Abstractions;
using Messenger.Api.Areas.Data.Models;
using Messenger.Common;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Messenger.Api.Areas.Cryptography.Services
{
    public class JsonWebTokenProvider : IJsonWebTokenProvider
    {
        private readonly string _secretKey;
        private readonly int _expirationMinutes;
        private readonly string _issuer;
        private readonly string _audience;
        public JsonWebTokenProvider(IConfiguration configuration)
        {
            _secretKey = configuration["JsonWebToken:SecretKey"]!;
            _expirationMinutes = int.Parse(configuration["JsonWebToken:ExpirationMinutes"]!);
            _issuer = configuration["JsonWebToken:Issuer"]!;
            _audience = configuration["JsonWebToken:Audience"]!;
        }

        public JsonWebTokenDto CreateJsonWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("UserId", user.Id.ToString()),
                    new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Username),
                    new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Email),
                    new System.Security.Claims.Claim("IsEmailVerified", user.IsEmailVerified.ToString() ?? false.ToString()),
                    new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.PhoneNumber, user.Phone ?? string.Empty),
                    new System.Security.Claims.Claim("IsPhoneVerified", user.IsPhoneVerified?.ToString() ?? false.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
                SigningCredentials = credentials,
                Issuer = _issuer,
                Audience = _audience
            };

            var jsonWebTokenHandler = new JsonWebTokenHandler();
            string tokenString = jsonWebTokenHandler.CreateToken(tokenDescriptor);

            return new JsonWebTokenDto(tokenString);
        }
    }
}
