﻿using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto.User;
using Threesixty.Common.Contracts.Models;
using Threesixty.Dal.Bll.Managers;
using ThreesixtyService.Helpers;

namespace ThreesixtyService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager _userManager;

        public UserController(IConfiguration configuration) : base(configuration)
        {
            _userManager = new UserManager(ContextOptions);
        }

        // GET: api/User
        [HttpGet]
        [Authorize]
        public IEnumerable<User> Get()
        {
            return _userManager.GetUsers();
        }

        // GET: api/User/5
        [HttpGet("{username}", Name = "Get")]
        [Authorize]
        public User Get(string username)
        {
            return _userManager.GetUser(username);
        }

        [HttpPost("token")]
        public IActionResult Authenticate([FromBody] LoginInfo loingInfo)
        {
            var result = _userManager.Authenticate(loingInfo.Username, loingInfo.Password);
            if (!result.Success)
            {
                throw new ApiException(result.ErrorMessage, HttpStatusCode.Unauthorized);
            }

            if (result.User == null)
                throw new ApiException("Unable to get user data", HttpStatusCode.Unauthorized);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, result.User.Username),
                new Claim(ClaimTypes.Surname, result.User.Fullname)
            };

            return Ok(new
            {
                token = AuthHelper.BuildToken(claims)
            });
        }

        [HttpPost("add")]
        [Authorize]
        public IActionResult AddUser([FromBody] RegisterInfo registerInfo)
        {
            if(!ModelState.IsValid || registerInfo == null)
                throw new ApiException("Invalid input data. No user data received", HttpStatusCode.BadRequest);

            if(string.IsNullOrEmpty(registerInfo.Username) || string.IsNullOrEmpty(registerInfo.Password))
                throw new ApiException("Invalid user data. No user name or password received", HttpStatusCode.BadRequest);

            if (string.IsNullOrEmpty(registerInfo.Fullname))
                throw new ApiException("Invalid input data. No full name received", HttpStatusCode.BadRequest);

            return Ok(_userManager.AddUser(registerInfo));
        }

        [HttpGet("remove/{username}")]
        [Authorize]
        public IActionResult RemoveUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ApiException("Invalid user data. User name is not specified", HttpStatusCode.BadRequest);
            _userManager.RemoveUser(username);

            return Ok();
        }

        [HttpPost("changePassword")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] ChangePasswordInfo changePasswordInfo)
        {
            if (!ModelState.IsValid)
            {
                throw new ApiException("Invalid input data", HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(changePasswordInfo?.Password) || string.IsNullOrEmpty(changePasswordInfo.Username))
            {
                throw new ApiException("Invalid input data - username or password is not specified",
                    HttpStatusCode.BadRequest);
            }
            _userManager.ChangePassword(changePasswordInfo.Username, changePasswordInfo.OldPassword,
                changePasswordInfo.Password);

            return Ok();
        }
    }
}
