using System;
using System.Net;

namespace Threesixty.Common.Contracts
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ApiException(string message, HttpStatusCode code) : base(message)
        {
            StatusCode = code;
        }

        public ApiException(string message, Exception innerException, HttpStatusCode code) : base(message,
            innerException)
        {
            StatusCode = code;
        }
    }
}
