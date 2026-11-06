using G1AspNet.Data;
using G1AspNet.Models;
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
    public IActionResult Index()
    {
        var categories = context.Categories
            .Include(c => c.ClothingItems)
            .OrderBy(c => c.Name)
            .ToList();

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
    /* DESCOMENTA DESDE AQUÍ

    // GET /Categories/Create
    // Enseña el formulario vacío (la vista Form sirve para crear y para editar).
    [HttpGet]
    public IActionResult Create()
    {
        // Una categoría nueva (Id = 0).
        var category = new Category();
        return View("Form", category);
    }

    // GET /Categories/Edit/3
    // Enseña el mismo formulario relleno con los datos de la categoría.
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
    [HttpPost]
    public IActionResult Save(Category category)
    {
        // Si falla alguna validación ([Required], [StringLength]...), volvemos al formulario.
        if (!ModelState.IsValid)
        {
            return View("Form", category);
        }

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
    FIN DESCOMENTA */
}
