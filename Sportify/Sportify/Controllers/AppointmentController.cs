using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportify.Data;
using Sportify.Models;
using Sportify.ViewModels;

public class AppointmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public AppointmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "User")]
    [HttpGet]
    public async Task<IActionResult> BookAppointment()
    {
        var services = await _context.Servisler
            .Include(s => s.gym)      
            .Include(s => s.trainer)  
            .ToListAsync();

        if (services == null || !services.Any())
            return NotFound();

        return View(services);
    }


    //CREATE

    [Authorize(Roles = "User")]
    [HttpGet]
    public IActionResult CreateAppointment()
    {
        return View();
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> CreateAppointment(Appointment model)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var UserName = currentUser.FullName;

        bool exists = await _context.Randevular
        .AnyAsync(x => x.UserName == UserName &&
                   x.ServiceId == model.ServiceId &&
                   x.TrainerId == model.TrainerId &&
                   x.GymId == model.GymId);

        if (exists)
        {
            ModelState.AddModelError("hata", "Bu randevu zaten alınmıştır!");
            return RedirectToAction("BookAppointment");
        }

        

        var appointment = new Appointment
        {
            TrainerId = model.TrainerId,
            trainer = model.trainer,
            GymId = model.GymId,
            gym=model.gym,
            ServiceId = model.ServiceId,
            service=model.service,
            UserName = UserName
        };

        _context.Randevular.Add(appointment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    //READ

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ListAppointment()
    {
        var appointment = await _context.Randevular.Include(x => x.gym).Include(x => x.service).Include(x=>x.trainer).ToListAsync();
        return View(appointment);
    }

    //UPDATE

    

    //DELETE

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> DeleteAppointment(int? id)
    {
        if (id == null)
            return NotFound();

        var appointment = await _context.Randevular.FindAsync(id);

        if (appointment == null)
            return NotFound();

        var trainer = await _context.Egitmenler.FindAsync(appointment.TrainerId);

        var model = new AppointmentDetailViewModel
        {
            AppointmentId = appointment.AppointmentId,
            UserName = appointment.UserName,
            TrainerName = trainer?.TrainerName
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var appointment = await _context.Randevular.FindAsync(id);
        if (appointment != null)
        {
            _context.Randevular.Remove(appointment);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index", "Home");
    }
}
