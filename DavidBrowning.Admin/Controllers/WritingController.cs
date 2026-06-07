using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;
public class WritingController : Controller
{
   public IActionResult Index()
   {
      return View();
   }
}
