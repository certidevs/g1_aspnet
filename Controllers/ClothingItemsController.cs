using System.Runtime.InteropServices;
using G1AspNet.Data;
using G1AspNet.Enums;
using G1AspNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

    public IActionResult Index(string? search, ClothingColor? color, ClothingMaterial? Material, SizeType? SizeType, string? Size, ItemCondition? Condition, ItemStatus? Status, int? donationId)
    {

        // TODO: ¿Filtrar por periodo en el que está libre
        // TODO: ¿Valores por defecto de Condition y Status?
        // TODO: ¿Filtrar por Location debe llegar parametro LocationId (o es un objeto tipo location)?
        var query = context.ClothingItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i => i.Name.ToLower().Contains(search.Trim().ToLower()));
        }

        ViewBag.Search = search;
        ViewBag.donationId = donationId;

        var clothingItems = query.OrderBy(i => i.Name)
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Include(i => i.Donation)
            .ToList();

        if (donationId is not null)
        {
            return RedirectToAction("Details", "Donations", new { id = donationId });
        }
        else
        {
            return View(clothingItems);

        }
    }

    

    public IActionResult Details(int id, int? donationId)
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
        ViewBag.DonationId = donationId;

        return View(clothingItem);



    }



    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Create(int? donationId)
    {
        var clothingItem = new ClothingItem { };
        if (donationId != null)
        {
            clothingItem.DonationId = donationId;
        }

        LoadDonations();
        LoadLocations();
        LoadCategories();

        return View("Form", clothingItem);
    }


    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ClothingItem clothingItem, int? donationId)
    {
        if (!ModelState.IsValid)
        {
            // Hay que volver a cargar los desplegables, o el formulario reaparece sin opciones.
            LoadDonations();
            LoadLocations();
            LoadCategories();
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
        if(clothingItem.ImageFile != null && clothingItem.ImageFile.Length > 0)
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

        context.SaveChanges();


        TempData["Message"] = "Prenda guardada correctamente.";
        if (donationId is not null)
        {
            return RedirectToAction("Details", "Donations", new { id = donationId });
        }
        else
        {
            return RedirectToAction("Details", new { id = clothingItem.Id });

        }
    }



    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var clothingItem = context.ClothingItems.Find(id);
        if (clothingItem == null)
        {
            return NotFound();
        }

        LoadDonations();
        LoadLocations();
        LoadCategories();

        return View("Form", clothingItem);
    }



    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public IActionResult Delete(int id, int? donationId)
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
            return RedirectToAction("Details", new { id });
        }

        // eliminación de la imagen si contiene una relacionada
        if (clothingItem.ImageFileName != null)
        {
            DeleteImage(clothingItem.ImageFileName);
        }

        context.ClothingItems.Remove(clothingItem);
        context.SaveChanges();

        TempData["Message"] = "Prenda borrada.";
        if (donationId is not null)
        {
            return RedirectToAction("Details", "Donations", new { id = donationId });
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




    private void LoadDonations()
    {
        var donations = context.Donations.OrderByDescending(d => d.DonatedAt).ToList();

        var donationOptions = donations.Select(d => new
        {
            Id = d.Id,
            Name = $"{d.DonorName} ({d.DonatedAt.ToString("dd-MM-yyyy")})"
        });

        var placeholderOption = new SelectListItem
        {
            Value = "",
            Text = "Selecciona la donación de origen",
        };

        ViewBag.Donations = new SelectList(donationOptions, "Id", "Name")
        .Prepend(placeholderOption);
    }



    private void LoadLocations()
    {
        var locations = context.Locations
        .Where(l => l.IsActive)
        .OrderBy(l => l.Name)
        .ToList();

        var placeholderOption = new SelectListItem
        {
            Value = "",
            Text = "Selecciona la sede",
        };

        ViewBag.Locations = new SelectList(locations, "Id", "Name")
        .Prepend(placeholderOption);
    }



    private void LoadCategories()
    {
        var categories = context.Categories.OrderBy(c => c.Name).ToList();
        var placeholderOption = new SelectListItem
        {
            Value = "",
            Text = "Selecciona la categoría",
        };

        ViewBag.Categories = new SelectList(categories, "Id", "Name")
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
