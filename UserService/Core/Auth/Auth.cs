using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Serilog;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;


namespace Core.Auth
{
    public static class Jwt
    {
        private const string SecretKey = "YourSuperLongSecureKey_ReplaceThisWithAReallyLongRandomString_AtLeast64Bytes1234567890!@#"; // Замени на свой ключ
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(SecretKey);

        private static string CreateToken(string email, TimeSpan expiry)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var uniqueJti = Guid.NewGuid().ToString() + "-" + RandomNumberGenerator.GetInt32(int.MaxValue);

            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, uniqueJti)
        };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.Add(expiry),
                signingCredentials: creds
            );

            return tokenHandler.WriteToken(token);
        }



        public static async Task<(string accessToken, string refreshToken)> GenerateKey(string email)
        {
            return await Task.Run(() =>
            {
                var accessToken = CreateToken(email, TimeSpan.FromMinutes(15));
                var refreshToken = CreateToken(email, TimeSpan.FromDays(30));
                return (accessToken, refreshToken);
            });
        }

        public static async Task<bool> CheckKey(string token, string email)
        {
            return await Task.Run(() =>
            {
                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                try
                {
                    var principal = handler.ValidateToken(token, validationParameters, out _);
                    var tokenEmail = principal.FindFirst(ClaimTypes.Email)?.Value;
                    return tokenEmail == email;
                }
                catch
                {
                    return false;
                }
            });
        }

        public static async Task<(string newAccessToken, string newRefreshToken)> Refresh(string refreshToken, string email)
        {
            return await Task.Run(() =>
            {
                if (!CheckKey(refreshToken, email).Result) throw new SecurityTokenException("Invalid refresh token");

                var newAccessToken = CreateToken(email, TimeSpan.FromMinutes(15));
                var newRefreshToken = CreateToken(email, TimeSpan.FromDays(30));
                return (newAccessToken, newRefreshToken);
            });
        }

        
    }
}