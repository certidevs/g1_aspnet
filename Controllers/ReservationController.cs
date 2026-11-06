using G1AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using G1AspNet.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using G1AspNet.ViewModels;
using System.Runtime.CompilerServices;
using System.Data;


namespace G1AspNet.Controllers;

public class ReservationsController : Controller
{
    
    private readonly ApplicationDbContext context;

    public ReservationsController(ApplicationDbContext context)
    {
        
        this.context = context;

    }
    
    public IActionResult Index()
    {
        
        var reservations = context.Reservations
            .Include(r => r.ClothingItem)
            .Include(r => r.ApplicationUser)
            .OrderBy(r => r.Id)
            .ToList();

        return View(reservations);


    }

    public IActionResult Details( int id)
    {
        

        var reservation = context.Reservations
        .Include(r => r.ClothingItem)
        .Include(r => r.ApplicationUser)
        .FirstOrDefault(r => r.Id == id);

        if (reservation == null)
        {
            
            return NotFound();

        }
        
        return View(reservation);

    }

    // TODO (issue #17): Create, Edit, Save y Delete con su vista Form (las vistas, en la issue #18)
    // El código ya está escrito aquí abajo, comentado. El modelo que funciona es LocationController.

    // TODO (issue #17): Create, Edit, Save y Delete
    // Pasos: 1) borra la línea «/* DESCOMENTA DESDE AQUÍ» y la línea «FIN DESCOMENTA */», 2) ajusta los campos de tu entidad, 3) crea Views/Reservations/Form.cshtml, 4) descomenta los botones de Index y Details.
    /* DESCOMENTA DESDE AQUÍ

    // Rellena los desplegables del formulario (la vista los lee de ViewBag.ClothingItems y ViewBag.Users).
    private void LoadLists()
    {
        // Ajusta: aquí salen todas las prendas; podrías mostrar solo las que están Available.
        ViewBag.ClothingItems = new SelectList(context.ClothingItems.OrderBy(c => c.Name), "Id", "Name");
        // context.Users son las cuentas de Identity (ApplicationUser).
        ViewBag.Users = new SelectList(context.Users.OrderBy(u => u.UserName), "Id", "UserName");
    }

    // GET /Reservations/Create
    // Enseña el formulario vacío (la vista Form sirve para crear y para editar).
    [HttpGet]
    public IActionResult Create()
    {
        // Una reserva nueva (Id = 0): se recoge mañana y caduca dentro de una semana.
        var reservation = new Reservation
        {
            PickupDate = DateTime.Today.AddDays(1),
            ExpiresAt = DateTime.Today.AddDays(7)
        };
        LoadLists();
        return View("Form", reservation);
    }

    // GET /Reservations/Edit/3
    // Enseña el mismo formulario relleno con los datos de la reserva.
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var reservation = context.Reservations.Find(id);
        if (reservation == null)
        {
            return NotFound();
        }

        LoadLists();
        return View("Form", reservation);
    }

    // POST /Reservations/Save
    // Recibe el formulario. Id == 0 es una reserva nueva; Id > 0 es una que se edita.
    [HttpPost]
    public IActionResult Save(Reservation reservation)
    {
        // Si falla alguna validación, volvemos al formulario (y hay que volver a cargar los desplegables).
        if (!ModelState.IsValid)
        {
            LoadLists();
            return View("Form", reservation);
        }

        if (reservation.Id == 0)
        {
            context.Reservations.Add(reservation);
        }
        else
        {
            context.Reservations.Update(reservation);
        }

        context.SaveChanges();

        TempData["Message"] = "Reserva guardada correctamente.";
        return RedirectToAction("Details", new { id = reservation.Id });
    }

    // POST /Reservations/Delete/3
    // Una reserva no tiene entidades que dependan de ella, así que se puede borrar directamente.
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var reservation = context.Reservations.Find(id);
        if (reservation == null)
        {
            return NotFound();
        }

        context.Reservations.Remove(reservation);
        context.SaveChanges();

        TempData["Message"] = "Reserva eliminada correctamente.";
        return RedirectToAction("Index");
    }
    FIN DESCOMENTA */

}

