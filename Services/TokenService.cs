using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using jwt_identity_api.Data;
using jwt_identity_api.Models;
using jwt_identity_api.Options;
using Microsoft.IdentityModel.Tokens;

namespace jwt_identity_api.Services
{
    public interface ITokenService
    {
        TokenReturned GenerateToken(ApplicationUser user);
    }

    public class TokenService : ITokenService
    {
        public readonly JwtSettings _jwtSettings;

        public TokenService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public TokenReturned GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtSettings.AudienceToken,
                Issuer = _jwtSettings.IssuerToken,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Typ, user.Role),
                    new Claim("id", user.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenReturned
            {
                Id = Guid.Parse(user.Id),
                Token = tokenHandler.WriteToken(token),
                Role = user.Role,
                ExpireTime = token.ValidTo.AddHours(_jwtSettings.ExpireMinutes)
            };
        }
    }
}