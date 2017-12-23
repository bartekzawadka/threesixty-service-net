using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Threesixty.Common.Contracts.Models;
using Threesixty.Dal.Bll;

namespace ThreesixtyService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private UserManager _userManager;

        public UserController(IConfiguration configuration) : base(configuration)
        {
            _userManager = new UserManager(ContextOptions);
        }

        // GET: api/User
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _userManager.GetUsers();
        }

        // GET: api/User/5
        [HttpGet("{username}", Name = "Get")]
        public User Get(string username)
        {
            return _userManager.GetUser(username);
        }
        
//        // POST: api/User
//        [HttpPost]
//        public void Post([FromBody]string value)
//        {
//        }
//        
//        // PUT: api/User/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody]string value)
//        {
//        }
//        
//        // DELETE: api/ApiWithActions/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }
    }
}
