using G1AspNet.Data;
using G1AspNet.Enums;
using G1AspNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace G1AspNet.Controllers;

public class ClothingItemsController : Controller
{
    private readonly ApplicationDbContext context;

    public ClothingItemsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IActionResult Index(string? search, ClothingColor? color, ClothingMaterial? Material, SizeType? SizeType, string? Size, ItemCondition? Condition, ItemStatus? Status)
    {

        // TODO: ¿Filtrar por periodo en el que está libre
        // TODO: ¿Valores por defecto de Condition y Status?
        // TODO: ¿Filtrar por Location debe llegar parametro LocationId (o es un objeto tipo location)?
        var query = context.ClothingItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i => i.Name.ToLower().Contains(search.ToLower()));
        }

        ViewBag.Search = search;

        var clothingItems = query.OrderBy(i => i.Name)
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Include(i => i.Donation)
            .ToList();

        return View(clothingItems);
    }

    public IActionResult Details(int id)
    {
        var clothingItem = context.ClothingItems
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Include(i => i.Donation)
            .Include(i => i.Reservations)
            .FirstOrDefault(r => r.Id == id);
        
        if (clothingItem == null)
        {
            return NotFound();
        }

        return View(clothingItem);
    }
}