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

        // ========================= TALENT =========================
        public string GenerateAccessToken(Talent talent)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);

            var claims = new[]
            {
                new Claim("TalentId", talent.TalentId),
                new Claim(ClaimTypes.Email, talent.Email),
                new Claim(ClaimTypes.Name, talent.Nama),
                new Claim(ClaimTypes.Role, "Talent"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        // ========================= ADMIN =========================
        public string GenerateAccessTokenAdmin(Admin admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);

            var claims = new[]
            {
                new Claim("AdminId", admin.AdminId.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                //new Claim(ClaimTypes.Role, admin.HakAkses), // Disnaker, Perusahaan, Vokasi
                new Claim(ClaimTypes.Role, "Admin"), // Disnaker, Perusahaan, Vokasi
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
                
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        // ========================= ADMIN SECURITY =========================
        public string GenerateAccessTokenSecurity(AdminSecurity adminSecurity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);

            var claims = new[]
            {
                new Claim("AdminSecurityId", adminSecurity.AdminSecurityId.ToString()),
                new Claim("NIM", adminSecurity.Nim),
                new Claim(ClaimTypes.Name, adminSecurity.NamaLengkap ?? adminSecurity.Nim),
                new Claim(ClaimTypes.Role, "Security"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        // ========================= REFRESH TOKEN =========================
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        // ========================= VALIDASI TOKEN =========================
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
                    ValidateLifetime = false, // Tidak cek expiry (buat refresh)
                    ClockSkew = TimeSpan.Zero
                };

                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }

        // Backward compatibility
        public string GenerateToken(Talent talent) => GenerateAccessToken(talent);
    }
}
