using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportify.Data;
using Sportify.Models;
using Microsoft.AspNetCore.Authorization;

namespace Sportify.Controllers
{
    public class GymController : Controller
    {
        public ApplicationDbContext _context;
        public GymController(ApplicationDbContext context)
        {
            _context = context;
        }

        //CREATE

        [Authorize]
        [HttpGet]
        public IActionResult CreateGym()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateGym(Gym model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool exists = await _context.Salonlar
                .AnyAsync(x => x.GymName.ToLower() == model.GymName.ToLower());

            if (exists)
            {
                ModelState.AddModelError("GymName", "Bu isimde bir salon zaten var!");
                return View(model);
            }

            if(model.OpenTime>=model.CloseTime)
            {
                ModelState.AddModelError("OpenTime", "Salon kapanış saatinden sonra açılamaz!");
                ModelState.AddModelError("CloseTime", "Salon açılış saatinden önce kapanamaz!");
                return View(model);
            }

            _context.Salonlar.Add(model);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        //READ

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListGym()
        {
            var gymList = await _context.Salonlar.ToListAsync();
            return View(gymList);
        }

        //UPDATE

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditGym(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gym = await _context.Salonlar.FindAsync(id);
            if (gym == null)
            {
                return NotFound();
            }
            return View(gym);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGym(Gym model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymExists(model.GymId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        //DELETE

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteGym(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gym = await _context.Salonlar.FirstOrDefaultAsync(m => m.GymId == id);
            if (gym == null)
            {
                return NotFound();
            }

            return View(gym);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gym = await _context.Salonlar.FindAsync(id);
            if (gym != null)
            {
                _context.Salonlar.Remove(gym);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        private bool GymExists(int id)
        {
            return _context.Salonlar.Any(e => e.GymId == id);
        }
    }
}
