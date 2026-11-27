using System.Runtime.InteropServices;
using G1AspNet.Data;
using G1AspNet.Enums;
using G1AspNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace G1AspNet.Controllers;

public class ClothingItemsController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly IWebHostEnvironment environment;

    public ClothingItemsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }

    public IActionResult Index(ClothingColor? color, 
        ClothingMaterial? material, 
        ItemCondition? condition, 
        ItemStatus? status,
        string size = "",
        int locationId = 0,
        int categoryId = 0,
        string search = "",
        string sortBy = "name",
        bool sortDescending = false,
        string returnTo = "",
        int returnToId = 0)
    {

// Limpiar

        // Conservar la búsqueda tal como la introduce el usuario en el Viewbag antes de limpiarla
        ViewBag.Search = search;
        search = search.Trim();

        string[] validSortBy = {"name", "category", "size", "color", "state", "location"};
        sortBy = !validSortBy.Contains(sortBy.ToLower()) ? validSortBy[0] : sortBy.ToLower();
        
        ViewBag.SortBy = sortBy;
        ViewBag.SortDescending = sortDescending;

        string[] validReturnTo ={"index", "donation", "category"};
        returnTo = !validReturnTo.Contains(returnTo.ToLower()) ? validReturnTo[0] : returnTo.ToLower();

        ViewBag.ReturnTo = returnTo;
        ViewBag.ReturnToId = returnToId;

        // Filtrar

        var query = context.ClothingItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i => i.Name.ToLower().Contains(search.Trim().ToLower()));
        }

        if (categoryId > 0)
        {
            query = query.Where(i => i.CategoryId == categoryId);
            ViewBag.CategoryId = categoryId;
        }

        if (color is not null)
        {
            query = query.Where(i => i.Color == color);
        }

        if (material is not null)
        {
            query = query.Where(i => i.Material == material);
        }

        if (!string.IsNullOrWhiteSpace(size))
        {
            query = query.Where(i => i.Size == size);
        }

        if (condition is not null)
        {
            query = query.Where(i => i.Condition == condition);
        }

        if (status is not null)
        {
            query = query.Where(i => i.Status == status);
        }

        if (locationId > 0)
        {
            query = query.Where(i => i.LocationId == locationId);
        }

        // Ordenar

        query = query
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Include(i => i.Donation);

        query = sortBy switch
        {
            "category" => sortDescending
                ? query.OrderByDescending(i => i.Category!.Name)
                : query.OrderBy(i => i.Category!.Name),
            "size"=> sortDescending
                ? query.OrderByDescending(i => i.Size)
                : query.OrderBy(i => i.Size),
            "color"=> sortDescending
            ?query.OrderByDescending(i => i.Color!.ToString())
                : query.OrderBy(i => i.Color!.ToString()),
            "state"=> sortDescending
                ? query.OrderByDescending(i => i.Status!.ToString())
                : query.OrderBy(i => i.Status!.ToString()),
            "location"=> sortDescending
                ?query.OrderByDescending(i => i.Location!.Name)
                : query.OrderBy(i => i.Location!.Name),
            _ => sortDescending
                ? query.OrderByDescending(i => i.Name)
                : query.OrderBy(i => i.Name)
        };

        var clothingItems = query.ToList();

        // Options para el formulario de filtrado
        LoadCategories("Todas", categoryId.ToString());
        LoadLocations("Todas", locationId.ToString());

        // TODO: crear LoadSizes con las tallas de la query

        return View(clothingItems);

    }

    public IActionResult Details(int id, bool returnToDonation = false)
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

        ViewBag.ReturnToDonation = returnToDonation;
        return View(clothingItem);
    }



    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Create(int? donationId)
    {
        bool returnToDonation = false;
        var clothingItem = new ClothingItem { };
        if (donationId is not null)
        {
            clothingItem.DonationId = donationId;
            ViewBag.ReturnToDonation = true;
        }

        LoadDonations("Selecciona la donacion");
        LoadLocations("Selecciona la sede");
        LoadCategories("Selecciona la categoria");
        ViewBag.ReturnToDonation = returnToDonation;

        return View("Form", clothingItem);
    }


    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ClothingItem clothingItem, bool returnToDonation = false)
    {
        if (!ModelState.IsValid)
        {
            // Hay que volver a cargar los desplegables, o el formulario reaparece sin opciones.
            LoadDonations("Selecciona la donacion");
            LoadLocations("Selecciona la sede");
            LoadCategories("Selecciona la categoria");
            ViewBag.ReturnToDonation = returnToDonation;
            return View("Form", clothingItem);
        }

        // detectar si ya habia una imagen antes
        ClothingItem? actual = await context.ClothingItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == clothingItem.Id);
        if (actual != null)
        {
            if (actual.ImageFileName != clothingItem.ImageFileName)
            {
                DeleteImage(actual.ImageFileName);
            }
        }

        // guardado de imagenes
        if (clothingItem.ImageFile != null && clothingItem.ImageFile.Length > 0)
        {
            // saca la ubicación de las imagenes
            string carpeta = Path.Combine(
                environment.WebRootPath,
                "images/clothingItems"
            );

            // crea las carpetas si no existen
            Directory.CreateDirectory(carpeta);

            // genera un nombre único para el archivo
            string extension = Path.GetExtension(clothingItem.ImageFile.FileName);
            string nombreArchivo = $"{Guid.NewGuid()}{extension}";

            string rutaCompleta = Path.Combine(
                carpeta,
                nombreArchivo
            );

            // Guarda el archivo fisicamente
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await clothingItem.ImageFile.CopyToAsync(stream);
            }

            // guarda la ruta en la base de datos
            clothingItem.ImageFileName = $"/images/clothingItems/{nombreArchivo}";
        }

        if (clothingItem.Id == 0)
        {
            context.ClothingItems.Add(clothingItem);
        }
        else
        {
            context.ClothingItems.Update(clothingItem);
        }
        ;
        ViewBag.ReturnToDonation = returnToDonation;
        context.SaveChanges();

        TempData["Message"] = "Prenda guardada correctamente.";
        if (returnToDonation && clothingItem.DonationId is not null)
        {
            return RedirectToAction("Details", "Donations", new { id = clothingItem.DonationId });
        }
        else
        {
            return RedirectToAction("Details", new { id = clothingItem.Id });

        }
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Edit(int id, int? donationId, bool returnToDonation = false)
    {
        var clothingItem = context.ClothingItems.Find(id);
        if (clothingItem == null)
        {
            return NotFound();
        }

        LoadDonations("Selecciona la donacion");
        LoadLocations("Selecciona la sede");
        LoadCategories("Selecciona la categoria");
        ViewBag.ReturnToDonation = returnToDonation;

            return View("Form", clothingItem);
    }



    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public IActionResult Delete(int id, bool returnToDonation = false)
    {
        var clothingItem = context.ClothingItems.Find(id);
        if (clothingItem == null)
        {
            return NotFound();
        }

        // Las reservas son historial (la relación es Restrict): se comprueba antes de borrar para no provocar un error 500.
        if (context.Reservations.Any(r => r.ClothingItemId == id))
        {
            TempData["Error"] = "No se puede eliminar la prenda: tiene reservas. Edítala y ponla en estado Retirada.";
            return RedirectToAction("Details", new
            {
                id,
                returnToDonation
            });
        }

        // eliminación de la imagen si contiene una relacionada
        if (clothingItem.ImageFileName != null)
        {
            DeleteImage(clothingItem.ImageFileName);
        }

        // Se elimina la prenda de la base de datos, pero el objeto clothingItem sigue en RAM y se puede consultar sus mnétodos antes de que desapareca con el return
        context.ClothingItems.Remove(clothingItem);
        context.SaveChanges();

        TempData["Message"] = "Prenda borrada.";

        if (returnToDonation && clothingItem.DonationId is not null)
        {
            return RedirectToAction("Details", "Donations", new { id = clothingItem.DonationId });
        }
        else
        {
            return RedirectToAction("Index");
        }
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    private void DeleteImage(string? imageFileName)
    {
        if (string.IsNullOrEmpty(imageFileName))
            return;

        // Obtener la ruta física de wwwroot
        string webRoot = environment.WebRootPath;

        // Convertir la ruta relativa en ruta física
        string relativePath = imageFileName.TrimStart('/')
            .Replace('/', Path.DirectorySeparatorChar);

        string fullPath = Path.Combine(webRoot, relativePath);

        // Comprobar que el archivo existe
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }
    }

    private void LoadCategories(string placeHolder = "", string selectedItemId = "")
    {
        var categories = context.Categories
            .OrderBy(c => c.Name)
            .ToList();

        var placeholderOption = new SelectListItem
        {
            Value = "",
            Text = placeHolder,
        };
        
        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedItemId)
            .Prepend(placeholderOption);
    }

    private void LoadDonations(string placeHolder = "", string selectedItemId = "")
    {
        var donations = context.Donations
            .Include(d => d.Location)
            .OrderByDescending(d => d.DonatedAt)
            .ThenBy(d => d.Location!.Name)
            .Select(d => new
                {
                    Id = d.Id,
                    Name = $"{d.DonorName} ({d.Location!.Name} {d.DonatedAt.ToString("dd-MM-yyyy")})"
                })
            .ToList();

        var placeholderOption = new SelectListItem
        {
            Value = "",
            Text = placeHolder,
        };

        ViewBag.Donations = new SelectList(donations, "Id", "Name", selectedItemId)
            .Prepend(placeholderOption);
    }

    private void LoadLocations(string placeHolder = "", string selectedItemId = "")
    {
        var locations = context.Locations
            .Where(l => l.IsActive)
            .OrderBy(l => l.Name)
            .ToList();

        var placeholderOption = new SelectListItem
        {
            Value = "",
            Text = placeHolder,
        };

        ViewBag.Locations = new SelectList(locations, "Id", "Name", selectedItemId)
            .Prepend(placeholderOption);
    }

    // TODO (issue #15): Create, Edit, Save y Delete con su vista Form (las vistas, en la issue #16)
    // El código ya está escrito aquí abajo, comentado. El modelo que funciona es LocationController.

    // TODO (issue #15): Create, Edit, Save y Delete
    // Pasos: 1) borra la línea «/* DESCOMENTA DESDE AQUÍ» y la línea «FIN DESCOMENTA */», 2) ajusta los campos de tu entidad, 3) crea Views/ClothingItems/Form.cshtml, 4) descomenta los botones de Index y Details.
    /* DESCOMENTA DESDE AQUÍ

    // Rellena los desplegables del formulario con lo que hay en la base de datos.
    // La vista los lee de ViewBag (asp-items="ViewBag.Categories", etc.).
    private void LoadLists()
    {
        ViewBag.Categories = new SelectList(context.Categories.OrderBy(c => c.Name), "Id", "Name");
        ViewBag.Locations = new SelectList(context.Locations.OrderBy(l => l.Name), "Id", "Name");
        // La donación es opcional (DonationId es int?): en la vista hay una opción "Sin donación".
        ViewBag.Donations = new SelectList(context.Donations.OrderBy(d => d.DonorName), "Id", "DonorName");
    }

    // GET /ClothingItems/Create
    // Enseña el formulario vacío (la vista Form sirve para crear y para editar).
    [HttpGet]
    public IActionResult Create()
    {
        // Una prenda nueva (Id = 0), disponible por defecto.
        var clothingItem = new ClothingItem { Status = ItemStatus.Available };
        LoadLists();
        return View("Form", clothingItem);
    }

    // GET /ClothingItems/Edit/3
    // Enseña el mismo formulario relleno con los datos de la prenda.
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var clothingItem = context.ClothingItems.Find(id);
        if (clothingItem == null)
        {
            return NotFound();
        }

        LoadLists();
        return View("Form", clothingItem);
    }

    // POST /ClothingItems/Save
    // Recibe el formulario. Id == 0 es una prenda nueva; Id > 0 es una que se edita.
    [HttpPost]
    public IActionResult Save(ClothingItem clothingItem)
    {
        // Si falla alguna validación, volvemos al formulario (y hay que volver a cargar los desplegables).
        if (!ModelState.IsValid)
        {
            LoadLists();
            return View("Form", clothingItem);
        }

        if (clothingItem.Id == 0)
        {
            context.ClothingItems.Add(clothingItem);
        }
        else
        {
            context.ClothingItems.Update(clothingItem);
        }

        context.SaveChanges();

        TempData["Message"] = "Prenda guardada correctamente.";
        return RedirectToAction("Details", new { id = clothingItem.Id });
    }

    // POST /ClothingItems/Delete/3
    // Borra la prenda solo si no tiene reservas (la relación es Restrict: son historial).
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var clothingItem = context.ClothingItems.Find(id);
        if (clothingItem == null)
        {
            return NotFound();
        }

        // Comprobamos los dependientes antes de borrar, para no provocar un error 500.
        var hasReservations = context.Reservations.Any(r => r.ClothingItemId == id);
        if (hasReservations)
        {
            TempData["Error"] = "No se puede eliminar la prenda: tiene reservas. Edítala y ponla en estado Retirada.";
            return RedirectToAction("Details", new { id });
        }

        context.ClothingItems.Remove(clothingItem);
        context.SaveChanges();

        TempData["Message"] = "Prenda eliminada correctamente.";
        return RedirectToAction("Index");
    }
    FIN DESCOMENTA */
}
