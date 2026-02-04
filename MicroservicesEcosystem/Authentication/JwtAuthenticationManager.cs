using MicroservicesEcosystem.Authentication.Models;

using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
namespace MicroservicesEcosystem.Authentication
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {      
        private readonly string key;
        public JwtAuthenticationManager(string key)
        {
            this.key = key;
        }
       
        public TokenResponse AuthenticateOTP(Guid id,string otp)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenType = "";
            var expire = DateTime.UtcNow.AddHours(14);
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Claims = new Dictionary<string, object> {
                    { "id", id },
                    { "otp", otp },

                },
                Subject = new ClaimsIdentity(new Claim[] {
                     new Claim(ClaimTypes.AuthenticationMethod,"OTP"),
                }),

                TokenType = tokenType,
                Expires = expire,
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            TokenResponse tokenResponse = new TokenResponse(tokenHandler.WriteToken(token), expire, tokenType);
            return tokenResponse;
        }
     
    }
}
