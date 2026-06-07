using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.Asset;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;
public class AssetController : Controller
{
   public AssetController()
   {

   }

   public IActionResult Index(CancellationToken cancellationToken)
   {
      return View();
   }

   [HttpGet]
   public IActionResult Create(CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public IActionResult Create(
      EditViewModel model, 
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult Edit(int id, CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public IActionResult Edit(
      int id, 
      EditViewModel mode, 
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult Delete(int id, CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ActionName(nameof(Delete))]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> DeleteConfirmed(
      DeleteViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public async Task<IActionResult> Download(
      string assetKey,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }
}
