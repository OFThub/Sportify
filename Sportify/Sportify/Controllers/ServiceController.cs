using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportify.Data;
using Sportify.Models;

namespace Sportify.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ServiceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Create

        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateService()
        {
            

            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateService(Service model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userName = currentUser.FullName;
            
            model.UserName = userName;

            ModelState.Remove(nameof(model.UserName));
            ModelState.Remove(nameof(model.GymId));
            ModelState.Remove(nameof(model.gym));
            ModelState.Remove(nameof(model.TrainerId)); 
            ModelState.Remove(nameof(model.trainer));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingTrainer = await _context.Egitmenler
                .Include(t => t.gym)
                .FirstOrDefaultAsync(t => t.TrainerName == userName);

            if (existingTrainer == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcınızla eşleşen bir eğitmen kaydı bulunamadı.");
                return View(model);
            }

            var service = new Service
            {
                UserName = userName,
                ServiceName = model.ServiceName,
                ServicePrice = model.ServicePrice,
                ServiceTime = model.ServiceTime,

                TrainerId = existingTrainer.TrainerId, 
                trainer = existingTrainer,             

                GymId = existingTrainer.GymId,        
                gym = existingTrainer.gym             
            };

            _context.Servisler.Add(service);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        //READ

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListService()
        {
            var service = await _context.Servisler.Include(x=>x.gym).Include(x => x.trainer).ToListAsync();
            return View(service);
        }

        //UPDATE

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditService(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Servisler.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(Service model)
        {
            ModelState.Remove(nameof(model.ServiceId));
            ModelState.Remove(nameof(model.UserName));
            ModelState.Remove(nameof(model.GymId));
            ModelState.Remove(nameof(model.TrainerId));
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        //Delete

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteService(int? id)
        {
            if (id == null)
                return NotFound();

            var service = await _context.Servisler
                .Include(s => s.trainer)
                .FirstOrDefaultAsync(m => m.ServiceId == id);

            if (service == null)
                return NotFound();

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Servisler
                .FirstOrDefaultAsync(x => x.ServiceId == id);

            if (service == null)
                return NotFound();

            var relatedAppointments = await _context.Randevular
                .Where(a => a.ServiceId == id) 
                .ToListAsync();

            if (relatedAppointments != null && relatedAppointments.Count > 0)
            {
                _context.Randevular.RemoveRange(relatedAppointments);
            }

            _context.Servisler.Remove(service);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
