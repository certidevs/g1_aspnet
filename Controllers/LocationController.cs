using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using G1AspNet.Data;
using G1AspNet.Models;


namespace G1AspNet.Controllers;

// ENTIDAD DE EJEMPLO COMPLETA: Location tiene todo funcionando (listado, detalle,
// crear, editar y desactivar). Es el modelo a copiar para el resto de entidades:
// en los demás controladores tenéis este mismo código escrito pero comentado.
public class LocationController : Controller
{
    // El contexto es la puerta de entrada a la base de datos (lo recibe del sistema
    // gracias a la inyección de dependencias, no hay que crearlo a mano).
    private readonly ApplicationDbContext context;

    public LocationController(ApplicationDbContext context)
    {
        this.context = context;
    }



    // GET /Location?search=...&city=...
    // Lista las sedes. Los dos parámetros son opcionales y filtran por nombre y por ciudad.
    public IActionResult Index(string? search, string? city)
    {
        // AsQueryable() deja la consulta "abierta": todavía no se ha ejecutado,
        // así podemos añadirle filtros (Where) solo si el usuario ha escrito algo.
        var query = context.Locations.AsQueryable();


        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r =>  r.Name.ToLower().Contains(search.ToLower()));
        }


        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(r =>  r.City.ToLower().Contains(city.ToLower()));
        }


        // ToList() es lo que ejecuta la consulta en la base de datos.
        var locations = query.OrderBy(r => r.Name).ToList();
        return View(locations);
    }


    // GET /Location/Details/3
    // Muestra una sede con sus prendas y sus donaciones.
    public IActionResult Details(int id)
    {
        // Include trae también las listas relacionadas (si no, llegarían vacías).
        var location = context.Locations
            .Include(r => r.ClothingItems)
            .Include(r => r.Donations)
            .FirstOrDefault(r => r.Id == id);


        // Si el id no existe, 404 (nunca dejes que la vista falle con un null).
        if (location == null)
        {
            return NotFound();
        }

        return View(location);
    }



    // GET /Location/Create
    // Enseña el formulario vacío. Usa la vista Form, la misma que para editar.
    // [Authorize] ya venía puesto por el equipo: solo el Admin crea sedes.
    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Create()
    {
        // Una sede nueva (Id = 0) y activa por defecto.
        var location = new Location { IsActive = true };
        return View("Form", location);
    }



    // GET /Location/Edit/3
    // Enseña el mismo formulario, pero relleno con los datos de la sede.
    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        // Find busca por clave primaria.
        var location = context.Locations.Find(id);
        if (location == null)
        {
            return NotFound();
        }

        return View("Form", location);
    }



    // POST /Location/Save
    // Recibe el formulario (crear y editar usan esta misma acción).
    // El formulario envía el Id oculto: Id == 0 es una sede nueva, Id > 0 es una existente.
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public IActionResult Save(Location location)
    {
        // Validación: se comprueban los [Required], [StringLength]... del modelo.
        // Si algo falla, volvemos al formulario con los datos escritos y los mensajes de error.
        if (!ModelState.IsValid)
        {
            return View("Form", location);
        }

        if (location.Id == 0)
        {
            // Nueva: la añadimos al contexto (todavía no está en la base de datos).
            context.Locations.Add(location);
        }
        else
        {
            // Existente: Update marca todos sus campos como modificados.
            context.Locations.Update(location);
        }

        // SaveChanges es lo que realmente hace el INSERT o el UPDATE en la base de datos.
        context.SaveChanges();


        // TempData sobrevive a la redirección; el _Layout lo enseña como aviso verde.
        TempData["Message"] = "Localización guardada correctamente.";

        // Después de un POST se redirige (patrón Post/Redirect/Get) para que, al recargar la página,
        // el navegador no vuelva a enviar el formulario.
        return RedirectToAction("Details", new { id = location.Id });
    }



    // POST /Location/Delete/3
    // No borra la sede: solo la desactiva (IsActive = false). Se hace por POST porque cambia datos.
    // No se permite desactivar una sede que tenga prendas o donaciones.
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var location = context.Locations
            .Include(l => l.ClothingItems)
            .Include(l => l.Donations)
            .FirstOrDefault(l => l.Id == id);

        if (location == null)
        {
            return NotFound();
        }

        // Comprobamos los dependientes ANTES de tocar nada.
        if (location.ClothingItems.Count <= 0 && location.Donations.Count <= 0)
        {
            location.IsActive = false;
            context.SaveChanges();

            TempData["Message"] = "Localización desactivada correctamente.";
        }
        else
        {
            // TempData["Error"] se enseña como aviso rojo. Nunca dejamos que esto acabe en un error 500.
            TempData["Error"] = "No se puede desactivar la localización: tiene prendas o donaciones registradas.";
        }

        return RedirectToAction("Details", new { id });
    }
}