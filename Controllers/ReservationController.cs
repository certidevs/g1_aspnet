using G1AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using G1AspNet.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using G1AspNet.ViewModels;
using System.Runtime.CompilerServices;
using System.Data;
using Microsoft.VisualBasic;
using G1AspNet.Enums;
using G1AspNet.Utilities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace G1AspNet.Controllers;

public class ReservationsController : Controller
{

    private readonly ApplicationDbContext context;

    public ReservationsController(ApplicationDbContext context)
    {

        this.context = context;

    }

    
    [Authorize]
    public IActionResult Index()
    {



        if (User.IsInRole(RoleNames.Admin))
        {
            var reservations = context.Reservations
            .Include(r => r.ClothingItem)
            .Include(r => r.ApplicationUser)
            .OrderBy(r => r.Id)
            .ToList();



            return View(reservations);




        }


        var reservationsUser = context.Reservations.
        Where(r => r.ApplicationUserId == User.GetRequiredUserId())
        .ToList();

        return View(reservationsUser);




    }

    public IActionResult Details(int id)
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



    private void LoadData()
    {

        ViewBag.Users = new SelectList(context.Users.OrderBy(u => u.UserName), "Id", "UserName");

        ViewBag.ClothingItems = new SelectList(context.ClothingItems.OrderBy(c => c.Name), "Id", "Name");

    }

    [HttpGet]
    public IActionResult Create()
    {

        var reservation = new Reservation
        {

            PickupDate = DateTime.Today.AddDays(1),
            ExpiresAt = DateTime.Today.AddDays(7)

        };

        LoadData();
        return View("Form", reservation);


    }

    [HttpGet]
    public IActionResult Edit(int id)
    {

        var reservation = context.Reservations.Find(id);
        if (reservation == null)
        {

            return NotFound();

        }

        LoadData();
        return View("Form", reservation);


    }


    [HttpPost]
    public IActionResult Save(Reservation reserve)
    {
        if (!ModelState.IsValid)
        {
            LoadData();
            return View("Form", reserve);
        }


        bool expires = reserve.Status == Enums.ReservationStatus.Cancelled ||
        reserve.Status == Enums.ReservationStatus.Expired;



        if (expires)
        {

            ClothingItem prenda = context.ClothingItems.
            FirstOrDefault(p => p.Id == reserve.ClothingItemId);

            prenda.Status = ItemStatus.Available;


        }





        bool reservaActiva =
        reserve.Status == Enums.ReservationStatus.Confirmed ||
        reserve.Status == Enums.ReservationStatus.Pending;



        if (reservaActiva)
        {
            int reservasActivas = context.Reservations.Count(r =>
                r.ClothingItemId == reserve.ClothingItemId &&
                r.Id != reserve.Id &&
                (r.Status == Enums.ReservationStatus.Confirmed ||
                 r.Status == Enums.ReservationStatus.Pending));

            if (reservasActivas > 0)
            {
                ModelState.AddModelError(
                    "",
                    "Solo puede haber una reserva activa para esta prenda.");

                LoadData();
                return View("Form", reserve);
            }
        }

        if (reserve.Id == 0)
        {
            context.Reservations.Add(reserve);
        }
        else
        {
            context.Reservations.Update(reserve);
        }

        context.SaveChanges();

        TempData["Message"] = "Reserva guardada correctamente";
        return RedirectToAction("Details", new { id = reserve.Id });
    }

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

        TempData["Message"] = "Reserva borrada";
        return RedirectToAction("Index");


    }


    // este metodo establece el estado de la reserva indicada como "PickedUp" y su prenda correspondiente como "Delivered"
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public IActionResult SetPickedUp(int id)
    {
        var reservation = context.Reservations.Find(id);

        // se comprueba que la reserva existe y si su estado es Pending o confirmed
        if (reservation != null &&
            (reservation.Status == ReservationStatus.Pending ||
             reservation.Status == ReservationStatus.Confirmed)
        )
        {
            reservation.Status = ReservationStatus.PickedUp; // asigna la reserva como Recogida

            // si tiene prenda, asigna la prenda como enviada
            var clothingItem = context.ClothingItems.Find(reservation.ClothingItemId);
            if (clothingItem != null)
            {
                clothingItem.Status = ItemStatus.Delivered;
                context.Update(clothingItem);
            }

            context.Update(reservation);
            context.SaveChanges();

            TempData["Message"] = "Entrega realizada con exito.";
            return RedirectToAction("Index");
        }

        TempData["Message"] = "No se pudo actualizar el estado de la reserva.";
        return RedirectToAction("Details", new { id });

    }
    // Estados que ocupan una prenda
    private static readonly ReservationStatus[] ActiveStatuses =
    {
    ReservationStatus.Pending,
    ReservationStatus.Confirmed
};

    // Comprueba si la reserva sigue activa
    private static bool IsActive(Reservation reservation)
    {
        return ActiveStatuses.Contains(reservation.Status);
    }
    private IQueryable<Reservation> VisibleReservations()
    {
        var query = context.Reservations.AsQueryable();

        if (!User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            query = query.Where(r => r.ApplicationUserId == userId);
        }

        return query;
    }
    
    [HttpPost]
    public IActionResult Cancel(int id)
    {
        // Buscar la reserva dentro de las reservas visibles para el usuario
        var reservation = VisibleReservations()
            .Include(r => r.ClothingItem)
            .FirstOrDefault(r => r.Id == id);

        // Si no existe o no pertenece al usuario, devolver 404
        if (reservation == null)
        {
            return NotFound();
        }

        // Comprobar si todavía se puede cancelar
        if (!IsActive(reservation))
        {
            TempData["Error"] = "Solo se puede cancelar una reserva pendiente o confirmada.";
            return RedirectToAction("Details", new { id });
        }

        // Cambiar el estado de la reserva
        reservation.Status = ReservationStatus.Cancelled;

        // Liberar la prenda
        if (reservation.ClothingItem != null)
        {
            reservation.ClothingItem.Status = ItemStatus.Available;
        }

        context.SaveChanges();

        TempData["Message"] = "Reserva cancelada. La prenda vuelve a estar disponible.";

        return RedirectToAction("Details", new { id });
    }

    // TODO (issue #17): Create, Edit, Save y Delete con su vista Form (las vistas, en la issue #18)
    // El código ya está escrito aquí abajo, comentado. El modelo que funciona es LocationController.

    // TODO (issue #17): Create, Edit, Save y Delete
    // Pasos: 1) borra la línea «/* DESCOMENTA DESDE AQUÍ» y la línea «FIN DESCOMENTA */», 2) ajusta los campos de tu entidad, 3) crea Views/Reservations/Form.cshtml, 4) descomenta los botones de Index y Details.

    /*
    // Rellena los desplegables del formulario (la vista los lee de ViewBag.ClothingItems y ViewBag.Users).
    private void LoadLists()
    {
        // Ajusta: aquí salen todas las prendas; podrías mostrar solo las que están Available.
        ViewBag.ClothingItems = new SelectList(context.ClothingItems.OrderBy(c => c.Name), "Id", "Name");
        // context.Users son las cuentas de Identity (ApplicationUser).
        ViewBag.Users = new SelectList(context.Users.OrderBy(u => u.UserName), "Id", "UserName");
    }
/*
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
    */

}

