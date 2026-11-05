using G1AspNet.Data;
using G1AspNet.Models;
using Microsoft.AspNetCore.Mvc;
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
        // GET: DonationControler
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
            var donations = context.Donations
            .Include(l => l.Location)
            .Include(c => c.ClothingItems)
            .FirstOrDefault(d => d.Id == id);
            return View(donations);
        }
        /*[HttpGet]
        public IActionResult Create()
        {
            var donation = new Donation
            {
                DonorName = string.Empty,
                DonorEmail = string.Empty
            };
            return View("Form", donation);
        }

        [HttpPost]
        public IActionResult Save(Donation donation)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", donation);
            }
            context.Donations.Add(donation);
            context.SaveChanges();

            TempData["Message"] = "Donación guardado correctamente.";
            return RedirectToAction("Details", new { id = donation.Id });
        }
*/


    }
}
