using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
  public class TrailsController : Controller
  {

    private readonly INationalParkRepository _npRepo;
    private readonly ITrailRepository _trailRepo;

    public TrailsController(INationalParkRepository npRepo, ITrailRepository trailRepo)
    {
      _npRepo = npRepo;
      _trailRepo = trailRepo;
    }

    public IActionResult Index()
    {
      return View(new Trail() { });
    }

    public async Task<IActionResult> Upsert(int? id)
    {
      IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

      TrailsVM objVM = new TrailsVM()
      {
        NationalParkList = npList.Select(i => new SelectListItem
        {
          Text = i.Name,
          Value = i.Id.ToString()
        }),
        Trail = new Trail()
      };

      if (id == null)
      {
        // CREATE trail
        return View(objVM);
      }
      // UPDATE trail
      objVM.Trail = await _trailRepo.GetAsync(SD.TrailsAPIPath, id.GetValueOrDefault());
      if (objVM.Trail == null)
      {
        return NotFound();
      }
      return View(objVM);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(TrailsVM obj)
    {
      if (ModelState.IsValid)
      {

        if (obj.Trail.Id == 0)
        {
          await _trailRepo.CreateAsync(SD.TrailsAPIPath, obj.Trail);
        }
        else
        {
          await _trailRepo.UpdateAsync(SD.TrailsAPIPath + obj.Trail.Id, obj.Trail);
        }
        return RedirectToAction(nameof(Index));
      }
      else
      {
        IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

        TrailsVM objVM = new TrailsVM()
        {
          NationalParkList = npList.Select(i => new SelectListItem
          {
            Text = i.Name,
            Value = i.Id.ToString()
          }),
          Trail = obj.Trail
        };
        return View(objVM);
      }
    }

    public async Task<IActionResult> GetAllTrails()
    {
      return Json(new { data = await _trailRepo.GetAllAsync(SD.TrailsAPIPath) });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
      var status = await _trailRepo.DeleteAsync(SD.TrailsAPIPath, id);
      if (status)
      {
        return Json(new { success = true, message = "Delete Successful" });
      }
      return Json(new { success = false, message = "Delete not Successful" });
    }
  }
}