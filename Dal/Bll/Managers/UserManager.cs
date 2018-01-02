using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto;
using Threesixty.Common.Contracts.Dto.User;
using Threesixty.Common.Contracts.Models;

namespace Threesixty.Dal.Bll.Managers
{
    public class UserManager: Manager
    {
        public UserManager(DbContextOptions<ThreesixtyContext> options) : base(options)
        {
        }

        public int AddUser(RegisterInfo registerInfo)
        {
            var user = CreateUser(registerInfo);

            return ExecuteDb(db =>
            {
                if (db.Users.Any(x => x.Username == user.Username))
                    throw new ApiException("User '" + user.Username + "' already exists", HttpStatusCode.Conflict);

                db.Users.Add(user);
                db.SaveChanges();

                return user.Id;
            });
        }

        public static User CreateUser(RegisterInfo registerInfo)
        {
            if(registerInfo == null)
                throw new ApiException("Invalid user data. No data received", HttpStatusCode.BadRequest);

            if (!registerInfo.IsValid())
            {
                throw new ApiException("Invalid user data. User info is not complete", HttpStatusCode.BadRequest);
            }

            if (!Equals(CryptoUtils.CalculateHash(registerInfo.Password),
                CryptoUtils.CalculateHash(registerInfo.PasswordConfirm)))
                throw new ApiException("User password mismatch", HttpStatusCode.Forbidden);

            return new User
            {
                Fullname = registerInfo.Fullname,
                Username = registerInfo.Username,
                CreatedAt = DateTime.Now,
                Password = CryptoUtils.CalculateHash(registerInfo.Password)
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

        private User GetUser(string userName, bool includePassword = false)
        {
            return ExecuteDb(db =>
            {
                var query = db.Users.Where(x => x.Username == userName);
                if (includePassword)
                {
                    query = query.Select(x => new User
                    {
                        Id = x.Id,
                        Username = x.Username,
                        CreatedAt = x.CreatedAt,
                        Fullname = x.Fullname,
                        Password = x.Password
                    });
                }
                else
                {
                    query = query.Select(x => new User
                    {
                        Id = x.Id,
                        Username = x.Username,
                        CreatedAt = x.CreatedAt,
                        Fullname = x.Fullname
                    });
                }

                var user = query.SingleOrDefault();

                if (user == null)
                {
                    throw new ApiException("User '" + userName + "' does not exist", HttpStatusCode.NotFound);
                }

                return user;
            });
        }

        public User GetUser(string userName)
        {
            return GetUser(userName, false);
        }

        public void RemoveUser(string username)
        {
            ExecuteDb(db =>
            {
                var user = GetUser(username);
                if (user == null)
                {
                    throw new ApiException("User '" + username + "' does not exist", HttpStatusCode.NotFound);
                }

                db.Entry(user).State = EntityState.Deleted;

                return true;
            });
        }

        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            ExecuteDb(db =>
            {
                var user = GetUser(username, true);

                if (!Equals(user.Password, CryptoUtils.CalculateHash(oldPassword)))
                    throw new ApiException("Invalid password", HttpStatusCode.BadRequest);

                user.Password = CryptoUtils.CalculateHash(newPassword);
                db.Entry(user).State = EntityState.Modified;
                return true;
            });
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
