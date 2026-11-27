using G1AspNet.Data;
using G1AspNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace G1AspNet.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext context;

    public CategoriesController(ApplicationDbContext context)
    {
        this.context = context;
    }

    // Listado de categorías con su número de prendas.
    public IActionResult Index(string? search, int? cantPrendas)
    {
        var query = context.Categories.AsQueryable();


        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r =>  r.Name.ToLower().Contains(search.ToLower().Trim()));
        }

        if (cantPrendas != null)
        {
            query = query.Where(r => r.ClothingItems.Count() == cantPrendas);
        }

        query = query.Include(c => c.ClothingItems);

        ViewBag.search = search;
        ViewBag.cantPrendas = cantPrendas;

        // ToList() es lo que ejecuta la consulta en la base de datos.
        var categories = query.OrderBy(r => r.Name).ToList();
        return View(categories);
    }

    // Detalle de una categoría con sus prendas.
    public IActionResult Details(int id)
    {
        var category = context.Categories
            .Include(c => c.ClothingItems)
            .FirstOrDefault(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }



    // TODO (issue #11): Create, Edit, Save y Delete con su vista Form (las vistas, en la issue #12)
    // El código ya está escrito aquí abajo, comentado. El modelo que funciona es LocationController.

    // TODO (issue #11): Create, Edit, Save y Delete
    // Pasos: 1) borra la línea «/* DESCOMENTA DESDE AQUÍ» y la línea «FIN DESCOMENTA */», 2) ajusta los campos de tu entidad, 3) crea Views/Categories/Form.cshtml, 4) descomenta los botones de Index y Details.

    // GET /Categories/Create
    // Enseña el formulario vacío (la vista Form sirve para crear y para editar).
    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Create()
    {
        // Una categoría nueva (Id = 0).
        var category = new Category();
        return View("Form", category);
    }

    // GET /Categories/Edit/3
    // Enseña el mismo formulario relleno con los datos de la categoría.
    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var category = context.Categories.Find(id);
        if (category == null)
        {
            return NotFound();
        }

        return View("Form", category);
    }



    // POST /Categories/Save
    // Recibe el formulario. Id == 0 es una categoría nueva; Id > 0 es una que se edita.
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public IActionResult Save(Category category)
    {
        // Si falla alguna validación ([Required], [StringLength]...), volvemos al formulario.
        if (!ModelState.IsValid)
        {
            return View("Form", category);
        }


        // El nombre no puede repetirse con el de otra categoría (la propia, al editar, no cuenta).
        // Si se repite, se vuelve al formulario con lo escrito y el error junto al campo.
        var name = category.Name.Trim();
        if (context.Categories.Any(c => c.Name.ToLower() == name.ToLower() && c.Id != category.Id))
        {
            ModelState.AddModelError(nameof(Category.Name), "Ya existe una categoría con ese nombre.");
            return View("Form", category);
        }

        category.Name = name;

        if (category.Id == 0)
        {
            context.Categories.Add(category);
        }
        else
        {
            context.Categories.Update(category);
        }

        context.SaveChanges();

        TempData["Message"] = "Categoría guardada correctamente.";

        return RedirectToAction("Details", new { id = category.Id });
    }



    // POST /Categories/Delete/3
    // Borra la categoría solo si no tiene prendas (la relación es Restrict: la base de datos no lo permitiría).
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var category = context.Categories.Find(id);
        if (category == null)
        {
            return NotFound();
        }

        // Comprobamos los dependientes antes de borrar, para no provocar un error 500.
        var hasItems = context.ClothingItems.Any(i => i.CategoryId == id);
        if (hasItems)
        {
            TempData["Error"] = "No se puede eliminar la categoría: tiene prendas.";
            return RedirectToAction("Details", new { id });
        }

        context.Categories.Remove(category);
        context.SaveChanges();

        TempData["Message"] = "Categoría eliminada correctamente.";
        return RedirectToAction("Index");
    }
}
