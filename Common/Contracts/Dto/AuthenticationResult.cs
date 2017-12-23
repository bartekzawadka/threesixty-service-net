using System;
using System.Collections.Generic;
using System.Text;

namespace Threesixty.Common.Contracts.Dto
{
    public class AuthenticationResult
    {
        public bool Success { get; }

        public string ErrorMessage { get; }

        public AuthenticationResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}
