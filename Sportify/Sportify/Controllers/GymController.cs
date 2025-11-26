using Sportify.Data;
using Sportify.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    public class GymController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GymController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public IActionResult Index()
        {
            var gym = _context.Salonlar.ToList();
            return View(gym);
        }

        // CREATE - GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - POST
        [HttpPost]
        public IActionResult Create(Gym gym)
        {
            if (ModelState.IsValid)
            {
                _context.Salonlar.Add(gym);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gym);
        }

        // EDIT - GET
        public IActionResult Edit(int id)
        {
            var salon = _context.Salonlar.Find(id);
            return View(salon);
        }

        // EDIT - POST
        [HttpPost]
        public IActionResult Edit(Gym gym)
        {
            if (ModelState.IsValid)
            {
                _context.Salonlar.Update(gym);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gym);
        }

        // DELETE - GET
        public IActionResult Delete(int id)
        {
            var gym = _context.Salonlar.Find(id);
            return View(gym);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var gym = _context.Salonlar.Find(id);
            _context.Salonlar.Remove(gym);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
