using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using G1AspNet.Data;
using G1AspNet.Models;


namespace G1AspNet.Controllers;

public class LocationController : Controller
{
    private readonly ApplicationDbContext context;

    public LocationController(ApplicationDbContext context)
    {
        this.context = context;
    }



    public IActionResult Index(string? search, string? city)
    {
        var query = context.Locations.AsQueryable();


        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r =>  r.Name.ToLower().Contains(search.ToLower()));
        }


        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(r =>  r.City.ToLower().Contains(city.ToLower()));
        }


        var locations = query.OrderBy(r => r.Name).ToList();
        return View(locations);
    }


    public IActionResult Details(int id)
    {
        var location = context.Locations
            .Include(r => r.ClothingItems)
            .Include(r => r.Donations)
            .FirstOrDefault(r => r.Id == id);
        

        if (location == null)
        {
            return NotFound();
        }

        return View(location);
    }



    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Create()
    {
        var location = new Location { IsActive = true };
        return View("Form", location);
    }



    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var location = context.Locations.Find(id);
        if (location == null)
        {
            return NotFound();
        }

        return View("Form", location);
    }



    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public IActionResult Save(Location location)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", location);
        }

        if (location.Id == 0)
        {
            context.Locations.Add(location);
        }
        else
        {
            context.Locations.Update(location);
        }

        context.SaveChanges();


        TempData["Message"] = "Localización guardado correctamente.";
        return RedirectToAction("Details", new { id = location.Id });
    }



    // no borra la localizacion, solo lo desactiva
    // no permitir borrar una sede con prendas o donaciones
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public IActionResult Delete(int id) 
    {
        var location = context.Locations
            .Include(l => l.ClothingItems)
            .Include(l => l.Donations)
            .FirstOrDefault(l => l.Id == id);

        if (location == null)
        {
            return NotFound();
        }

        // verifica que no tenga ni prendas o donaciones
        if (location.ClothingItems.Count <= 0 && location.Donations.Count <= 0)
        {
            location.IsActive = false;
            context.SaveChanges();

            TempData["Message"] = "Localización desactivada correctamente.";
        }
        else
        {
            TempData["Message"] = "No se puede desactivar la Localización, tiene prendas o donaciones registrados.";
        }
        
        return RedirectToAction("Details", new { id });
    }
}