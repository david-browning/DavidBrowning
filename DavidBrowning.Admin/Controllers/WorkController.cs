using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;
public class WorkController : Controller
{
   public IActionResult Index()
   {
      return View();
   }
}
