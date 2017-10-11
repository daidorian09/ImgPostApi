using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RestPostTestApp.ViewModels
{
    public class ImageUploadViewModel
    {
        [Required]
        [Display(Name = "Image Upload")]
        public IFormFile File { get; set; }
    }
}
