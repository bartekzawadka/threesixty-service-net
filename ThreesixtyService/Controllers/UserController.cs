using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto.User;
using Threesixty.Common.Contracts.Models;
using Threesixty.Dal.Bll;
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
    }
}
