using System;

namespace Threesixty.Common.Contracts.Dto.User
{
    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}