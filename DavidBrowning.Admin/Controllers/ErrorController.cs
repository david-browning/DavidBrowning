using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;
public class ErrorController : Controller
{
   public IActionResult Index()
   {
      return View();
   }
}
