using System;
using System.Linq;
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
                var user = UserManager.CreateUser("admin", "Admin", "admin123");
                context.Users.Add(user);
            }

            context.SaveChanges();
        }
    }
}
