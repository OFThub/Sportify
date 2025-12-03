using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sportify.Data;
using Sportify.Models;
using Sportify.ViewModels;

namespace Sportify.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public TrainerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //CREATE

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> CreateTrainer()
        {
            var salonlar = await _context.Salonlar.ToListAsync();
            ViewBag.Salonlar = new SelectList(salonlar, "GymId", "GymName");

            var currentUser = await _userManager.GetUserAsync(User);
            var userName = currentUser.FullName;

            var model = new Trainer
            {
                TrainerName = userName
            };

            return View(model);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateTrainer(Trainer model)
        {
            async Task ReloadViewBag(int? selectedGymId = null)
            {
                var salonlar = await _context.Salonlar.ToListAsync();
                ViewBag.Salonlar = new SelectList(salonlar, "GymId", "GymName", selectedGymId);
            }

            bool exists = await _context.Egitmenler.AnyAsync(x =>x.TrainerName.ToLower() == model.TrainerName.ToLower());


            if (exists)
            {
                ModelState.AddModelError("GymId", "Bu kullanıcı zaten bir eğitmen!");
                return View(model);
            }

            ModelState.Remove(nameof(model.service));
            ModelState.Remove(nameof(model.gym));
            ModelState.Remove(nameof(model.GymId));

            if (!ModelState.IsValid)
            {
                await ReloadViewBag();
                return View(model);
            }

            var selectedGym = await _context.Salonlar
                                             .FirstOrDefaultAsync(s => s.GymId == model.GymId);

            if (selectedGym == null)
            {
                ModelState.AddModelError(string.Empty, "Seçilen spor salonu bulunamadı.");
                await ReloadViewBag();
                return View(model);
            }

            if (model.WorkStartTime < selectedGym.OpenTime || model.WorkEndTime > selectedGym.CloseTime)
            {
                ModelState.AddModelError(string.Empty,
                    $"Eğitmenin çalışma saatleri, spor salonunun saatleri dışında olamaz. Salon Açılış: {selectedGym.OpenTime.ToString(@"hh\:mm")}, Kapanış: {selectedGym.CloseTime.ToString(@"hh\:mm")}");

                await ReloadViewBag();

                return View(model);
            }
            else if (model.WorkStartTime >= model.WorkEndTime)
            {
                ModelState.AddModelError(nameof(model.WorkStartTime), "Başlangıç saati, bitiş saatinden önce olmalıdır.");
                await ReloadViewBag();
                return View(model);
            }

            var trainer = new Trainer
            {
                TrainerName = model.TrainerName,
                WorkEndTime = model.WorkEndTime,
                WorkStartTime = model.WorkStartTime,
                GymId = selectedGym.GymId,
                gym = selectedGym
            };

            _context.Egitmenler.Add(trainer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        //READ

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListTrainer()
        {
            var trainers = await _context.Egitmenler.Include(x=>x.gym).Include(x=>x.service).ToListAsync();
            return View(trainers);
        }

        //UPDATE

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditTrainer(int? id)
        {

            var trainer = await _context.Egitmenler.FindAsync(id);
            if (trainer == null) return NotFound();

            await PopulateGymsSelectList();
            return View(trainer);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainer(Trainer model)
        {
            ModelState.Remove(nameof(model.service));
            ModelState.Remove(nameof(model.gym));
            ModelState.Remove(nameof(model.GymId));
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(model.TrainerId)) return NotFound();
                    throw;
                }
                return RedirectToAction("Index", "Home");
            }

            await PopulateGymsSelectList();
            return RedirectToAction("Index", "Home");
        }

        //DELETE

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteTrainer(int? id)
        {
            if (id == null)
                return NotFound();

            var trainer = await _context.Egitmenler
                .FirstOrDefaultAsync(m => m.TrainerId == id);

            if (trainer == null)
                return NotFound();

            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Egitmenler
                .Include(x => x.service)
                .FirstOrDefaultAsync(x => x.TrainerId == id);

            if (trainer == null)
                return NotFound();

            var relatedAppointments = await _context.Randevular
                .Where(a => a.TrainerId == id) 
                .ToListAsync();

            if (relatedAppointments != null && relatedAppointments.Count > 0)
            {
                _context.Randevular.RemoveRange(relatedAppointments);
            }

            if (trainer.service != null && trainer.service.Count > 0)
            {
                _context.Servisler.RemoveRange(trainer.service);
            }

            _context.Egitmenler.Remove(trainer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        private async Task PopulateGymsSelectList()
        {
            var salonlar = await _context.Salonlar.ToListAsync();
            ViewBag.Salonlar = new SelectList(salonlar, "GymId", "GymName");
        }

        private bool TrainerExists(int id)
        {
            return _context.Egitmenler.Any(e => e.TrainerId == id);
        }
    }
}
