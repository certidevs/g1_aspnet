using G1AspNet.Data;
using G1AspNet.Models;
using Microsoft.AspNetCore.Mvc;

namespace G1AspNet.Controllers;

public class ClothingItemsController : Controller
{
    private readonly ApplicationDbContext context;

    public ClothingItemsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IActionResult Index(string? search)
    {
        var query = context.ClothingItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i => i.Name.ToLower().Contains(search.ToLower()));
        }

        ViewBag.Search = search;

        var clothingItems = query.OrderBy(i => i.Name).ToList();

        return View(clothingItems);
    }
}