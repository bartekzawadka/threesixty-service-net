using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Models;
using Threesixty.Dal.Bll;
using Threesixty.Dal.Bll.Managers;

namespace ThreesixtyService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ChunkController : Controller
    {
        public IConfiguration Configuration { get; }
        private readonly ChunkManager _chunkManager;

        public ChunkController(IConfiguration configuration)
        {
            Configuration = configuration;
            _chunkManager = new ChunkManager(new DbContextOptionsBuilder<ThreesixtyContext>().UseMySql(Configuration.GetConnectionString("DefaultConnection")).Options);
        }

        // GET: api/Chunk/5
        [HttpGet("{id}")]
        public Chunk Get(int id)
        {
            return _chunkManager.GetChunk(id);
        }
        
        // POST: api/Chunk
        [HttpPost("{id}")]
        public int Post([FromBody]Chunk value, int id)
        {
            return _chunkManager.AddChunk(value, id);
        }
        
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw new ApiException("Feature not supported yet", HttpStatusCode.Forbidden);
        }
    }
}
