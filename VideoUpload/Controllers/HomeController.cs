using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoUpload.Models;

namespace VideoUpload.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            List<Video> videos = new List<Video> { };
            GetCatalogueFiles(videos);

            return View(videos);
        }

        private void GetCatalogueFiles(List<Video> videos)
        {
            //Create the Content directory if it does not exist
            Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "Content"));

            //Build the path to the Content directory
            var path = Path.Combine(_env.WebRootPath, "Content");
            var files = Directory.GetFiles(path);
            int id = 0;

            foreach (var file in files)
            {
                id++;
                var fileInfo = new FileInfo(file);

                videos.Add(new Video { Id = id, Title = fileInfo.Name, Size = (fileInfo.Length / 1024), FilePath = Path.Combine("Content", fileInfo.Name) });
            }
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(IEnumerable<IFormFile> videoFiles)
        {
            if (videoFiles != null && videoFiles.ToList().Count > 0)
            {
                foreach (var file in videoFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(_env.WebRootPath, "Content", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Append))
                        {
                            file.CopyTo(fileStream);
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
