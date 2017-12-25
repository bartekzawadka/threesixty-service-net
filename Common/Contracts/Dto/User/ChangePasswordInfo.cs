namespace Threesixty.Common.Contracts.Dto.User
{
    public class ChangePasswordInfo : LoginInfo
    {
        public string OldPassword { get; set; }
    }
}
