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
using Threesixty.Dal.Bll;
using Threesixty.Dal.Dll;
using Threesixty.Dal.Dll.Models;

namespace ThreesixtyService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        public IConfiguration Configuration { get; }

        private ImageManager _imageManager;

        public ImageController(IConfiguration configuration)
        {
            Configuration = configuration;
            _imageManager = new ImageManager(new DbContextOptionsBuilder<ThreesixtyContext>().UseMySql(Configuration.GetConnectionString("DefaultConnection")).Options);
        }

        // GET: api/Image
        [HttpGet]
        public IEnumerable<Image> Get([FromQuery]int skip, [FromQuery]int limit)
        {
            return _imageManager.GetImages(skip, limit);
        }

        // GET: api/Image/5
        [HttpGet("{id}")]
        public Image Get(int id)
        {
            return _imageManager.GetImage(id);
        }
        
        // POST: api/Image
        [HttpPost]
        public Image Post([FromBody]Image image)
        {
            if (image == null)
            {
                throw new ApiException("Image data was not provided", HttpStatusCode.BadRequest);
            }

            var item = new Image
            {
                Name = image.Name,
                ChunkHeight = image.ChunkHeight,
                ChunkWidth = image.ChunkWidth
            };

            return _imageManager.AddImage(item);
        }

        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw new ApiException("Feature not supported yet", HttpStatusCode.Forbidden);
        }
    }
}
