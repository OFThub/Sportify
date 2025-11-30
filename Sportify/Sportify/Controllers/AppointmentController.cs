using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportify.Data;
using Sportify.Models;

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
        var trainer = await _context.Egitmenler.ToListAsync();

        if (trainer == null)
            return NotFound();

        return View(trainer);
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
    public async Task<IActionResult> CreateAppointment(int trainerId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var UserName = currentUser.FullName;

        var appointment = new Appointment
        {
            TrainerId = trainerId,
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
        var query = from appointment in _context.Randevular
                    join trainer in _context.Egitmenler 
                    on appointment.TrainerId equals trainer.TrainerId
                    select new AppointmentDetailViewModel 
                    {
                        AppointmentId = appointment.AppointmentId,
                        UserName = appointment.UserName, 
                        TrainerName = trainer.TrainerName,
                        ServiceName = trainer.ServiceName,
                        ServiceTime = trainer.ServiceTime
                    };

        var appointmentDetails = await query.ToListAsync();

        if (appointmentDetails == null || appointmentDetails.Count == 0)
            return View(new List<AppointmentDetailViewModel>());

        var allUsers = await _userManager.Users.ToListAsync();

        return View(appointmentDetails);
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
            TrainerName = trainer?.TrainerName, 
            ServiceName = trainer?.ServiceName,
            ServiceTime = trainer?.ServiceTime ?? 0
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
