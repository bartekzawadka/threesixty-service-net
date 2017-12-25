using System;
using Microsoft.EntityFrameworkCore;
using Threesixty.Common.Contracts.Dto.User;
//using NUnit.Framework;
using Xunit;
using Threesixty.Dal.Bll;
using Threesixty.Dal.Bll.Managers;

namespace BllTests
{
    public class UserManagerTests
    {
        private string _uname = "testUser";
        private string _pass = "testPass";
        private string _fname = "Test User";

        private readonly DbContextOptions<ThreesixtyContext> _contextOptions = new DbContextOptionsBuilder<ThreesixtyContext>()
            .UseMySql("Server=192.168.1.113;Port=3330;Uid=threesixty;Pwd=ThreeSixty8*;Database=threesixty4win").Options;

        private readonly UserManager _userManager;

        public UserManagerTests()
        {
            _userManager = new UserManager(_contextOptions);
        }

        [Fact]
        public void CreateUser()
        {
            Assert.True(_userManager.AddUser(new RegisterInfo
            {
                Username = _uname,
                Fullname = _fname,
                Password = _pass,
                PasswordConfirm = _pass
            }) > 0);
        }

        [Fact]
        public void DeleteUser()
        {
            _userManager.RemoveUser(_uname);
            Assert.True(true);
        }
    }
}
