using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Threesixty.Dal.Bll;

namespace ThreesixtyService.Controllers
{
    public class ControllerBase : Controller
    {
        private IConfiguration Configuration { get; }
        protected DbContextOptions<ThreesixtyContext> ContextOptions { get; }

        protected ControllerBase(IConfiguration configuration)
        {
            Configuration = configuration;
            ContextOptions = new DbContextOptionsBuilder<ThreesixtyContext>()
                .UseMySql(Configuration.GetConnectionString("DefaultConnection")).Options;
        }
    }
}
