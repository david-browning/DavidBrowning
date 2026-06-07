using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;
public class ProjectsController : Controller
{
   public IActionResult Index()
   {
      return View();
   }
}
