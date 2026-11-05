using G1AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using G1AspNet.Models;
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
        
        var reservations = context.Reservations.OrderBy(r => r.Id).ToList();

        return View(reservations);


    }

    public IActionResult Details( int id)
    {
        

        var reservation = context.Reservations
        .FirstOrDefault(r => r.Id == id);

        if (reservation == null)
        {
            
            return NotFound();

        }
        
        return View(reservation);

    }


}

