using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto;
using Threesixty.Dal.Bll;
using Threesixty.Dal.Bll.Converters;
using Threesixty.Dal.Bll.Mappers;
using Threesixty.Dal.Dll;
using Threesixty.Dal.Dll.Models;
using System.Net.Http.Headers;
using System.Net.Mime;
using ThreesixtyService.Helpers;

namespace ThreesixtyService.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        public IConfiguration Configuration { get; }

        private ImageManager _imageManager;

        public List<string> _downloadedFiles = new List<string>();

        public ImageController(IConfiguration configuration)
        {
            Configuration = configuration;
            _imageManager = new ImageManager(new DbContextOptionsBuilder<ThreesixtyContext>().UseMySql(Configuration.GetConnectionString("DefaultConnection")).Options);
        }

        // GET: api/Image
        [Produces("application/json")]
        [HttpGet]
        public PageableList<Image> Get([FromQuery]int skip, [FromQuery]int limit)
        {
            return _imageManager.GetImages(skip, limit);
        }

        // GET: api/Image/5
        [Produces("application/json")]
        [HttpGet("{id}")]
        public Image Get(int id)
        {
            return _imageManager.GetImage(id);
        }

        [HttpGet("{id}/download")]
        public IActionResult Download(int id, [FromQuery] string format, [FromQuery]string fileName)
        {
            var filePath = _imageManager.GetStrollerImageFile(id, DownloadFileTypeMapper.GetFileType(format));

            if (string.IsNullOrEmpty(format))
            {
                throw new ApiException("Invalid/unsupported file type", HttpStatusCode.BadRequest);
            }

            var fName = Path.GetFileName(filePath);
            _downloadedFiles.Add(filePath);

            if (!string.IsNullOrEmpty(fileName))
            {
                fName = fileName + Path.GetExtension(filePath);
            }
            fName = fName.Replace(' ', '-');
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream,
                "application/octet-stream", fName);
        }

        // POST: api/Image
        [Produces("application/json")]
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
        [Produces("application/json")]
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
                    _imageManager.AddImage(StrollerConverter.JsonStreamToStrollerFileInfo(file.OpenReadStream(), file.FileName));
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var downloadedFile in _downloadedFiles)
                {
                    try
                    {
                        System.IO.File.Delete(downloadedFile);
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine("Unable to remove file: '" + downloadedFile + "'");
                        Console.WriteLine(e);
                    }
                }
                _downloadedFiles = null;
            }

            base.Dispose(disposing);
        }
    }
}
