using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using inr.Models;
using System.IO;
using System.Web;

namespace inr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/submitform")]
        public async Task<IActionResult> FormSubmit(string email, IFormFile file)
        {
          Console.WriteLine(file.FileName);
            var response = await Uploader.UploadFileToS3(file);
            return View("Submitted", response);
        }

        [HttpGet("/submit")]
        public IActionResult Submitted()
        {
          return View();
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
