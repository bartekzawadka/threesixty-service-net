using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto;
using Threesixty.Common.Contracts.Models;

namespace Threesixty.Dal.Bll.Managers
{
    public class UserManager: Manager
    {
        public UserManager(DbContextOptions<ThreesixtyContext> options) : base(options)
        {
        }

        public int AddUser(string userName, string fullName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName), "User name was not provided");
            if (string.IsNullOrEmpty(fullName))
                throw new ArgumentNullException(nameof(fullName), "User full name was not provided");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "User password was not provided");

            var user = new User
            {
                Fullname = fullName,
                Password = CryptoUtils.CalculateHash(password),
                Username = userName,
                CreatedAt = DateTime.Now
            };

            return ExecuteDb(db =>
            {
                if (db.Users.Any(x => x.Username == userName))
                    throw new ApiException("User '" + userName + "' already exists", HttpStatusCode.Conflict);

                db.Users.Add(user);
                db.SaveChanges();

                return user.Id;
            });
        }

        public static User CreateUser(string username, string fullName, string password)
        {
            return new User
            {
                Fullname = fullName,
                Username = username,
                CreatedAt = DateTime.Now,
                Password = CryptoUtils.CalculateHash(password)
            };
        }

        public List<User> GetUsers()
        {
            return ExecuteDb(db =>
            {
                return db.Users.Select(x => new User
                {
                    Id = x.Id,
                    Username = x.Username,
                    CreatedAt = x.CreatedAt,
                    Fullname = x.Fullname
                }).ToList();
            });
        }

        public User GetUser(string userName)
        {
            return ExecuteDb(db => db.Users.Where(x => x.Username == userName).Select(x => new User
            {
                Id = x.Id,
                Username = x.Username,
                CreatedAt = x.CreatedAt,
                Fullname = x.Fullname
            }).SingleOrDefault());
        }

        public AuthenticationResult Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ApiException("User name was not provided", HttpStatusCode.Forbidden);
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ApiException("Password was not provided", HttpStatusCode.Forbidden);
            }

            return ExecuteDb(db =>
            {
                var user = db.Users.FirstOrDefault(x => x.Username == username);
                if (user == null)
                    return new AuthenticationResult(false, "User '" + username + "' could not be found");

                if (string.Equals(user.Password, CryptoUtils.CalculateHash(password)))
                {
                    var result = new AuthenticationResult(true, string.Empty);
                    result.SetUser(new User
                    {
                        CreatedAt = user.CreatedAt,
                        Username = user.Username,
                        Fullname = user.Fullname,
                        Id = user.Id
                    });
                    return result;
                }
                return new AuthenticationResult(false, "Invalid password");
            });
        }
    }
}
