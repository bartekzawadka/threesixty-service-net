using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Threesixty.Dal.Bll;

namespace ThreesixtyService.Helpers
{
    public class AuthHelper
    {
        public static SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(CryptoUtils.SaltBytes);
        }

        private static SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256);
        }

        public static string BuildToken(IEnumerable<Claim> claims)
        {
            var creds = GetSigningCredentials();
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
