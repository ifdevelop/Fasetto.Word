using Microsoft.AspNetCore.Mvc;

namespace Fasetto.Word.Web.Server.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TellMeMore(string id = "")
        {
            return new JsonResult(new { name = "TellMeMore", content = id });
            return View();
        }
    }
}
