using Microsoft.AspNetCore.Mvc;
using RestPostTestApp.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace RestPostTestApp.Controllers
{
    [Route("api/[controller]/upload/")]
    public class ImageUploadApiController : Controller
    {
        #region Member

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor
        public ImageUploadApiController(IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion


        #region Get
        //Get api/imageuploaapi/upload/file.png
        [HttpGet("{filename}")]
        public IActionResult GetImage(string filename)
        {
            int queryCount =(int) _httpContextAccessor.HttpContext?.Request?.Query.Count;
            int widthCount = (int)_httpContextAccessor.HttpContext?.Request?.Query["w"].Count;
            int heightCount = (int)_httpContextAccessor.HttpContext?.Request?.Query["h"].Count;

            if (queryCount <= 0 ||  widthCount == 0 && heightCount == 0)
                return NotFound("File is not found");
            
            int width = 0;
            int height = 0;

            bool isImageUploaded = _hostingEnvironment.WebRootFileProvider.GetFileInfo("/uploads/" + filename).Exists;

            if (!isImageUploaded)
                return StatusCode((int)HttpStatusCode.Conflict);

            else
            {
                string filePath = _hostingEnvironment.WebRootFileProvider.GetFileInfo("/uploads/" + filename).PhysicalPath;

                using (Image<Rgba32> image = Image.Load(filePath))
                {
                    width = widthCount == 0 ? image.Width * height / image.Height : Convert.ToInt32(_httpContextAccessor.HttpContext?.Request?.Query["w"]);
                    height = heightCount == 0 ? image.Height * width / image.Width : Convert.ToInt32(_httpContextAccessor.HttpContext?.Request?.Query["h"]);
                
                    bool isDirectoryExistent = Directory.Exists(_hostingEnvironment.WebRootPath + "/uploads/" + width + "X" + height);

                    if (!isDirectoryExistent)
                        Directory.CreateDirectory(_hostingEnvironment.WebRootPath + "/uploads/" + width + "X" + height);

                    bool isDirectoryFileExistent = _hostingEnvironment.WebRootFileProvider.GetFileInfo("/uploads/" + width + "X" + height + "/" + filename).Exists;

                    if (isDirectoryFileExistent)
                        return File(System.IO.File.OpenRead(_hostingEnvironment.WebRootFileProvider.GetFileInfo("/uploads/" + width + "X" + height + "/" + filename).PhysicalPath), "image/jpeg");

                    image.
                        Mutate(x => x
                              .Resize(width, height)
                        );
                    image.Save(_hostingEnvironment.WebRootPath + $"/uploads/" + width + "X" + height + "/" + filename);
                }
            }
            return File(System.IO.File.OpenRead(_hostingEnvironment.WebRootFileProvider.GetFileInfo("/uploads/" + width + "X" + height + "/" + filename).PhysicalPath), "image/jpeg");
        }

        #endregion



        #region Post

        // POST api/imageuploaapi/upload
        [HttpPost(Name = "Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(ImageUploadViewModel model)
        {
            if(model.File is null)
                return BadRequest(error: $"File cannot be empty");
            else
            {
                bool isDirectoryExistent = Directory.Exists(_hostingEnvironment.WebRootPath + "/uploads");

                if (!isDirectoryExistent)
                    Directory.CreateDirectory(_hostingEnvironment.WebRootPath + "/uploads");

                string uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                string fileExtension = Path.GetExtension(model.File.FileName);
                string fileName = Guid.NewGuid().ToString("N") + fileExtension;

                using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                    return CreatedAtRoute("Create", value: "File is successfully uploaded");
                }
            }
        }

        #endregion
    }
}
