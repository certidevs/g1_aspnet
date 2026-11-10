using G1AspNet.Data;
using G1AspNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace G1AspNet.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext context;

        public DonationController(ApplicationDbContext context)
        {
            this.context = context;
        }
        // GET: DonationController
        public IActionResult Index()
        {
            var donations = context.Donations
            .Include(l => l.Location)
            .Include(c => c.ClothingItems)
            .ToList();
            return View(donations);
        }
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

        // TODO (issue #19): Create, Edit, Save y Delete con su vista Form (las vistas, en la issue #20)
        // El código ya está escrito aquí abajo, comentado. El modelo que funciona es LocationController.

        // TODO (issue #19): Create, Edit, Save y Delete
        // Pasos: 1) borra la línea «/* DESCOMENTA DESDE AQUÍ» y la línea «FIN DESCOMENTA */», 2) ajusta los campos de tu entidad, 3) crea Views/Donation/Form.cshtml, 4) descomenta los botones de Index y Details.


        // Rellena el desplegable de sedes del formulario (la vista lo lee de ViewBag.Locations).
        private void LoadLists()
        {
            ViewBag.Locations = new SelectList(context.Locations.OrderBy(l => l.Name), "Id", "Name");
        }

        // GET /Donation/Create
        // Enseña el formulario vacío (la vista Form sirve para crear y para editar).
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
