#nullable disable

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using jwt_identity_api.Models.Response;
using jwt_identity_api.Options;
using Microsoft.IdentityModel.Tokens;

namespace jwt_identity_api.Services
{
    public interface ITokenGeneratorService
    {
        TokenResponse GenerateToken(string userId, string role);
    }

    public class TokenGeneratorService : ITokenGeneratorService
    {
        private readonly JwtOptions _jwtOptions;

        public TokenGeneratorService(JwtOptions jwtOptions) => _jwtOptions = jwtOptions;

        public TokenResponse GenerateToken(string userId, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOptions.AudienceToken,
                Issuer = _jwtOptions.IssuerToken,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(JwtRegisteredClaimNames.Typ, role),
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("id", userId)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResponse
            {
                Role = role.ToString(),
                Token = tokenHandler.WriteToken(token),
                RefreshToken = Guid.NewGuid().ToString(),
                ExpireTime = token.ValidTo.AddHours(_jwtOptions.ExpireMinutes)
            };
        }
    }
}