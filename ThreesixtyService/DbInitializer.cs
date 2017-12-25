using System;
using System.Linq;
using Threesixty.Common.Contracts.Dto.User;
using Threesixty.Common.Contracts.Models;
using Threesixty.Dal.Bll;
using Threesixty.Dal.Bll.Managers;

namespace ThreesixtyService
{
    public static class DbInitializer
    {
        public static void Initialize(ThreesixtyContext context)
        {
            context.Database.EnsureCreated();

            // Adding first user
            if (!context.Users.Any())
            {
                var regInfo = new RegisterInfo
                {
                    Username = "admin",
                    Fullname = "Admin",
                    Password = "admin123",
                    PasswordConfirm = "admin123"
                };

                var user = UserManager.CreateUser(regInfo);
                context.Users.Add(user);
            }

            context.SaveChanges();
        }
    }
}
