using G1AspNet.Data;
using G1AspNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace G1AspNet.Controllers
{
    [Authorize]
    public class DonationsController : Controller
    {
        private const string StaffRoles = RoleNames.Admin + "," + RoleNames.User;
        private readonly ApplicationDbContext context;

        // Rellena el desplegable de sedes del formulario (la vista lo lee de ViewBag.Locations).
        private void LoadLists(int? selectedLocation = null)
        {
            ViewBag.Locations = new SelectList(context.Locations.OrderBy(l => l.Name), "Id", "Name", selectedLocation);
        }

        public DonationsController(ApplicationDbContext context)
        {
            this.context = context;
        }
        // GET: DonationController
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult Index(string? search, DateTime? date, int? location ,string? shortBy, bool descending)
        {
            var query = context.Donations.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.DonorName.ToLower().Contains(search.ToLower().Trim()));
            }
            if (date is not null)
            {
                query = query.Where(r => r.DonatedAt.Date == date.Value.Date );
            }
            if (location is not null)
            {
                query  = query.Where(r => r.LocationId == location);
            }
            ViewBag.date = date;
            ViewBag.shortBy = shortBy;
            ViewBag.Search = search;
            ViewBag.location = location;
            LoadLists(location);

            query = query.Include(c => c.ClothingItems).Include(l => l.Location);

            if (!string.IsNullOrEmpty(shortBy))
            {
                switch (shortBy)
                {
                    case "name":
                        query = descending ? query.OrderBy(d => d.DonorName) : query.OrderByDescending(d => d.DonorName);
                        break;
                    case "date":
                        query = descending ? query.OrderBy(d => d.DonatedAt) : query.OrderByDescending(d => d.DonatedAt);
                        break;
                    case "location":
                        query = descending
                            ? query.OrderBy(d => d.Location == null ? null : d.Location.Name)
                            : query.OrderByDescending(d => d.Location == null ? null : d.Location.Name);
                        break;
                    default:
                        query = query.OrderBy(d => d.DonorName);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(d => d.DonorName);
            }

            return View(query.ToList());

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

        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet]
        public IActionResult Create()
        {
            var donation = new Donation();
            LoadLists();
            return View("Form", donation);
        }

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

        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost]
        public IActionResult Save(Donation donation)
        {

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
