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
using Threesixty.Common.Contracts.Dto;
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
        public PageableList<Image> Get([FromQuery]int skip, [FromQuery]int limit)
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

        [HttpPost]
        [Route("upload")]
        public string Upload([FromForm] object form)
        {
            if (Request.Form?.Files == null || Request.Form.Files.Count == 0)
            {
                throw new ApiException("No file specified to be uploaded. No data received", HttpStatusCode.BadRequest);
            }

            try
            {
                Parallel.ForEach(Request.Form.Files, (file, state, arg3) =>
                {
                    _imageManager.AddImage(StrollerProcessor.ParseStrollerFile(file.OpenReadStream(), file.FileName));
                });
            }
            catch (System.Exception e)
            {
                throw new ApiException("Stroller JSON file import failed: " + e.Message, e,
                    HttpStatusCode.BadRequest);
            }

            return null;
        }

        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw new ApiException("Feature not supported yet", HttpStatusCode.Forbidden);
        }
    }
}
