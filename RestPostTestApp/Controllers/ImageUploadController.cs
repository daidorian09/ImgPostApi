using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestPostTestApp.Controllers
{
    public class ImageUploadController : Controller
    {
        // GET: ImageUpload/Create
        public ActionResult Create()
        {
            return View();
        }      
    }
}