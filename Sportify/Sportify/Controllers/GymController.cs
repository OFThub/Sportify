using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportify.Data;
using Sportify.Models;

namespace Sportify.Controllers
{
    public class GymController : Controller
    {
        public ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public GymController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        //CREATE

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult CreateGym()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateGym(Gym model)
        {
            ModelState.Remove(nameof(model.Name));
            ModelState.Remove(nameof(model.Trainers));
            ModelState.Remove(nameof(model.Services));
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

            var currentUser = await _userManager.GetUserAsync(User);
            var userName = currentUser.FullName;

            var gym = new Gym
            {
                Name = userName,
                GymName = model.GymName,
                OpenTime = model.OpenTime,
                CloseTime = model.CloseTime
            };
            _context.Salonlar.Add(gym);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        //READ

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListGym()
        {
            var gymList = await _context.Salonlar.Include(x=>x.Trainers).Include(x=>x.Services).ToListAsync();
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
        public async Task<IActionResult> DeleteGym(int? id)
        {
            if (id == null)
                return NotFound();

            var gym = await _context.Salonlar
                .FirstOrDefaultAsync(m => m.GymId == id);

            if (gym == null)
                return NotFound();

            return View(gym);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceIds = await _context.Servisler
                .Where(s => s.GymId == id)
                .Select(s => s.ServiceId)
                .ToListAsync();

            var randevular = await _context.Randevular
                .Where(r => r.ServiceId != null && serviceIds.Contains(r.ServiceId.Value))
                .ToListAsync();

            if (randevular.Any())
            {
                _context.Randevular.RemoveRange(randevular);
                await _context.SaveChangesAsync();
            }

            var servisler = await _context.Servisler
                .Where(s => s.GymId == id)
                .ToListAsync();

            if (servisler.Any())
            {
                _context.Servisler.RemoveRange(servisler);
                await _context.SaveChangesAsync();
            }

            var egitmenler = await _context.Egitmenler
                .Where(t => t.GymId == id)
                .ToListAsync();

            if (egitmenler.Any())
            {
                _context.Egitmenler.RemoveRange(egitmenler);
                await _context.SaveChangesAsync();
            }

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
