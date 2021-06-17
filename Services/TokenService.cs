using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using jwt_identity_api.Data;
using jwt_identity_api.Models;
using Microsoft.IdentityModel.Tokens;

namespace jwt_identity_api.Services
{
    public static class TokenService
    {
        public static TokenReturned GenerateToken(ApplicationUser user) 
        { 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Audience = JwtSettings.AudienceToken,
                Issuer = JwtSettings.IssuerToken,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(JwtSettings.ExpireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenReturned
            {
                Token = tokenHandler.WriteToken(token),
                Role = user.Role,
                ExpireTime = token.ValidTo.AddMinutes(JwtSettings.ExpireMinutes)
            };
        }
    }
}