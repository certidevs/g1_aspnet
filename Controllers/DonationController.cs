using G1AspNet.Data;
using G1AspNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace G1AspNet.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private const string StaffRoles = RoleNames.Admin + "," + RoleNames.User;
        private readonly ApplicationDbContext context;

        public DonationController(ApplicationDbContext context)
        {
            this.context = context;
        }
        // GET: DonationController
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult Index(string? search)
        {
            var query = context.Donations.AsQueryable();


            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.DonorName.ToLower().Contains(search.ToLower()));
            }
            ViewBag.Search = search;

            query = query.Include(c => c.ClothingItems);

            // ToList() es lo que ejecuta la consulta en la base de datos.
            var donations = query.OrderBy(d => d.DonorName)
            .Include(l => l.Location)
            .Include(c => c.ClothingItems)
            .ToList();
            return View(donations);
        }
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult Details(int id)
        {

            var donation = context.Donations
            .Include(d => d.Location)
            .Include(d => d.ClothingItems)
            .FirstOrDefault(d => d.Id == id);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        // Rellena el desplegable de sedes del formulario (la vista lo lee de ViewBag.Locations).
        private void LoadLists()
        {
            ViewBag.Locations = new SelectList(context.Locations.OrderBy(l => l.Name), "Id", "Name");
        }

        // GET /Donation/Create
        // Enseña el formulario vacío (la vista Form sirve para crear y para editar).
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet]
        public IActionResult Create()
        {
            // Una donación nueva (Id = 0); DonatedAt ya trae la fecha de hoy por defecto.
            var donation = new Donation();
            LoadLists();
            return View("Form", donation);
        }

        // GET /Donation/Edit/3
        // Enseña el mismo formulario relleno con los datos de la donación.
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var donation = context.Donations.Find(id);
            if (donation == null)
            {
                return NotFound();
            }

            LoadLists();
            return View("Form", donation);
        }

        // POST /Donation/Save
        // Recibe el formulario. Id == 0 es una donación nueva; Id > 0 es una que se edita.
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost]
        public IActionResult Save(Donation donation)
        {
            // Si falla alguna validación, volvemos al formulario (y hay que volver a cargar el desplegable).
            if (!ModelState.IsValid)
            {
                LoadLists();
                return View("Form", donation);
            }

            if (donation.Id == 0)
            {
                context.Donations.Add(donation);
            }
            else
            {
                context.Donations.Update(donation);
            }

            context.SaveChanges();

            TempData["Message"] = "Donación guardada correctamente.";
            return RedirectToAction("Details", new { id = donation.Id });
        }

        // POST /Donation/Delete/3
        // Borra la donación. Sus prendas NO se borran: la relación es SetNull, así que
        // a cada prenda su DonationId le pasa a NULL (la prenda física sigue existiendo).
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var donation = context.Donations.Find(id);
            if (donation == null)
            {
                return NotFound();
            }

            context.Donations.Remove(donation);
            context.SaveChanges();

            TempData["Message"] = "Donación eliminada correctamente.";
            return RedirectToAction("Index");
        }
    }
}
