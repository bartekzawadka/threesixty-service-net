namespace Threesixty.Common.Contracts.Dto
{
    public class AuthenticationResult
    {
        public bool Success { get; }

        public string ErrorMessage { get; }

        public Models.User User { get; private set; }

        public AuthenticationResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public void SetUser(Models.User user)
        {
            User = user;
        }
    }
}
