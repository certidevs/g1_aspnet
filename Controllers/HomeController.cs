using G1AspNet.Data;
using G1AspNet.Enums;
using G1AspNet.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace G1AspNet.Controllers;

/// <summary>Página de inicio de CorporaDress: cifras, categorías y sedes del ropero solidario.</summary>
public sealed class HomeController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewData["AvailableItems"] = await context.ClothingItems.CountAsync(i => i.Status == ItemStatus.Available);
        ViewData["Donations"] = await context.Donations.CountAsync();
        ViewData["Reservations"] = await context.Reservations.CountAsync();
        ViewData["Locations"] = await context.Locations.CountAsync(l => l.IsActive);

        // Categorías con el número de prendas disponibles, para mostrarlas como etiquetas.
        var categories = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategorySummary(c.Name, c.ClothingItems.Count(i => i.Status == ItemStatus.Available)))
            .ToListAsync();
        ViewData["Categories"] = categories;

        // Sedes activas donde recoger las prendas.
        var locations = await context.Locations
            .AsNoTracking()
            .Where(l => l.IsActive)
            .OrderBy(l => l.City)
            .Take(3)
            .ToListAsync();
        ViewData["ActiveLocations"] = locations;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });

    /// <summary>Nombre de categoría y prendas disponibles en ella.</summary>
    public sealed record CategorySummary(string Name, int Available);
}
