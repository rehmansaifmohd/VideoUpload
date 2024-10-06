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
            var files = Directory.GetFiles(path, "*.mp4");
            int id = 0;

            foreach (var file in files)
            {
                id++;
                var fileInfo = new FileInfo(file);

                videos.Add(new Video { Id = id, Title = fileInfo.Name, Size = (fileInfo.Length / 1024), FilePath = Path.Combine("Content", fileInfo.Name) });
            }
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Upload(IEnumerable<IFormFile> videoFiles)
        {
            try
            {
                //Check if the form was posted with files
                if (videoFiles != null && videoFiles.ToList().Count > 0)
                {
                    //Loop through the files to save them to the Content directory
                    foreach (var file in videoFiles)
                    {
                        //Check if the file is not null and has a length
                        if (file != null && file.Length > 0)
                        {
                            //Get the file name and path
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(_env.WebRootPath, "Content", fileName);

                            //Save the file to the Content directory
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                                Console.WriteLine($"{fileName} uploaded");
                            }
                        }
                    }

                    //Return to the index page
                    return RedirectToAction("Index");
                }
                else
                {
                    //Return the upload page
                    return View();
                }
            }
            catch (Exception ex)
            {
                //Add an error message to the model state
                ModelState.AddModelError("File", "Error uploading file");
                Console.WriteLine($"Error uploading file: {ex.Message}");

                //Return to the upload page
                return RedirectToAction("Upload");
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
