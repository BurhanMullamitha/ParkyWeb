using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
      IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));

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
      objVM.Trail = await _trailRepo.GetAsync(SD.TrailsAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
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
          await _trailRepo.CreateAsync(SD.TrailsAPIPath, obj.Trail, HttpContext.Session.GetString("JWToken"));
        }
        else
        {
          await _trailRepo.UpdateAsync(SD.TrailsAPIPath + obj.Trail.Id, obj.Trail, HttpContext.Session.GetString("JWToken"));
        }
        return RedirectToAction(nameof(Index));
      }
      else
      {
        IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));

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
      return Json(new { data = await _trailRepo.GetAllAsync(SD.TrailsAPIPath, HttpContext.Session.GetString("JWToken")) });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
      var status = await _trailRepo.DeleteAsync(SD.TrailsAPIPath, id, HttpContext.Session.GetString("JWToken"));
      if (status)
      {
        return Json(new { success = true, message = "Delete Successful" });
      }
      return Json(new { success = false, message = "Delete not Successful" });
    }
  }
}