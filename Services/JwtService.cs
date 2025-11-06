using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using vocafind_api.Models;

namespace vocafind_api.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        // Generate Access Token (expired pendek - 15 menit)
        public string GenerateAccessToken(Talent talent)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);

            var claims = new[]
            {
                new Claim("TalentId", talent.TalentId),                                 //Id Talent
                new Claim(ClaimTypes.Email, talent.Email),                              //Email
                new Claim(ClaimTypes.Name, talent.Nama),                                //Nama 
                new Claim(ClaimTypes.Role, "Talent"),                                   //Role untuk otorisasi
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())       //Token 
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),                               //Expired 15 Menit
                SigningCredentials = new SigningCredentials(                            //Algoritma enkripsi
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        // Generate Refresh Token (random string)

        // Random 64-byte cryptographic token
        // Base64 encoded
        // Disimpan di database untuk validasi
        // Expiry 7 hari
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }



        // ✅ Validate Token (opsional - untuk validasi manual)
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false, // Jangan validasi expiry untuk refresh
                    ClockSkew = TimeSpan.Zero
                };

                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }

        // ✅ Method lama tetap ada untuk backward compatibility
        public string GenerateToken(Talent talent)
        {
            return GenerateAccessToken(talent);
        }
    }
}