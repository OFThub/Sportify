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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gym = await _context.Salonlar
                .Include(x => x.Trainers)
                .Include(x => x.Services)
                .FirstOrDefaultAsync(x => x.GymId == id);

            if (gym == null)
                return NotFound();

            if (gym.Trainers != null && gym.Trainers.Count > 0)
                _context.Egitmenler.RemoveRange(gym.Trainers);

            if (gym.Services != null && gym.Services.Count > 0)
                _context.Servisler.RemoveRange(gym.Services);

            _context.Salonlar.Remove(gym);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        private bool GymExists(int id)
        {
            return _context.Salonlar.Any(e => e.GymId == id);
        }
    }
}
