using Microsoft.AspNetCore.Mvc;

namespace Boucher_Double_Back_End.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = currentDirectory + "/index.html";
            string htmlContent = System.IO.File.ReadAllText(filePath);
            return Content(htmlContent, "text/html");
        }
    }
}
