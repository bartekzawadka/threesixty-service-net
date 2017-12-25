namespace Threesixty.Common.Contracts.Dto.User
{
    public class RegisterInfo : LoginInfo
    {
        public string PasswordConfirm { get; set; }

        public string Fullname { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) &&
                   !string.IsNullOrEmpty(PasswordConfirm) && !string.IsNullOrEmpty(Fullname);
        }
    }
}
