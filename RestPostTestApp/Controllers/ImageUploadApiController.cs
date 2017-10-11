using Microsoft.AspNetCore.Mvc;
using RestPostTestApp.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace RestPostTestApp.Controllers
{
    [Route("api/[controller]")]
    public class ImageUploadApiController : Controller
    {
        #region Member

        private readonly IHostingEnvironment _hostingEnvironment;

        #endregion

        #region Ctor
        public ImageUploadApiController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        //// GET api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}


        [HttpGet]
        public string GetManipulation()
        {
            var isImageUploaded = _hostingEnvironment.WebRootFileProvider.GetFileInfo("/uploads/test.png").Exists;    
            
            if (!isImageUploaded)
                return "file not found";
            else
            {
                var filePath = _hostingEnvironment.WebRootFileProvider.GetFileInfo("/uploads/test.png").PhysicalPath;
                using (Image<Rgba32> image = Image.Load(filePath))
                {
                    image.Mutate(x => x
                         .Resize(image.Width / 2, image.Height / 2)
                         .Grayscale());
                    image.Save("bar.jpg"); // automatic encoder selected based on extension.
                }
            }
                return "ok";
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        [HttpPost(Name = "Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(ImageUploadViewModel model)
        {
            if(model.File is null)
                return BadRequest(error: $"File cannot be empty");
            else
            {
                var isDirectoryExistent = Directory.Exists(_hostingEnvironment.WebRootPath + "/uploads");

                if (!isDirectoryExistent)
                    Directory.CreateDirectory(_hostingEnvironment.WebRootPath + "/uploads");

                var upload = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                using (var fileStream = new FileStream(Path.Combine(upload, model.File.FileName), FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                    return CreatedAtRoute("Create", value: "File is successfully uploaded");
                }
            }
        }
    }
}
