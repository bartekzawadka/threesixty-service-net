using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Threesixty.Common.Contracts.Dto.User;
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

        public static TokenInfo BuildToken(IEnumerable<Claim> claims)
        {
            var expires = DateTime.Now.AddMinutes(30);
            
            var creds = GetSigningCredentials();
            var token = new JwtSecurityToken(claims: claims, expires: expires,
                signingCredentials: creds);
            return new TokenInfo
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = expires
            };
        }
    }
}
