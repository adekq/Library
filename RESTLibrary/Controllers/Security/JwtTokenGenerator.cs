using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtSecurity
{
    public interface IJwtTokenGenerator
    {
        string GenerateJwtToken(string email, string role);
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtTokenConfiguration jwtTokenConfiguration;
        private readonly byte[] secret;

        public JwtTokenGenerator(JwtTokenConfiguration jwtTokenConfiguration) 
        {
            this.jwtTokenConfiguration = jwtTokenConfiguration;
            secret = Encoding.ASCII.GetBytes(jwtTokenConfiguration.Secret);
        }

        public string GenerateJwtToken(string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Iss, jwtTokenConfiguration.Claims.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, jwtTokenConfiguration.Claims.Audience)
            };

            var jwtToken = new JwtSecurityToken(
                jwtTokenConfiguration.Claims.Issuer,
                jwtTokenConfiguration.Claims.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(jwtTokenConfiguration.Claims.ExpirationTime),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature));
            
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}