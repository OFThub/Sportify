using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportify.Data;
using Sportify.Models;

[Authorize(Roles = "User")]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Challenge();
        }

        var userFullName = currentUser.FullName;

        var userAppointments = await (from appointment in _context.Randevular
                                      join trainer in _context.Egitmenler
                                      on appointment.TrainerId equals trainer.TrainerId
                                      where appointment.UserName == userFullName
                                      select new AppointmentDetailViewModel
                                      {
                                          AppointmentId = appointment.AppointmentId,
                                          UserName = appointment.UserName,
                                          TrainerName = trainer.TrainerName,
                                          ServiceName = trainer.ServiceName,
                                          ServiceTime = trainer.ServiceTime
                                      }).ToListAsync();

        var userTrainings = await _context.Egitmenler
                                          .Where(t => t.TrainerName == userFullName)
                                          .ToListAsync();

        var viewModel = new ProfileViewModel
        {
            MyAppointments = userAppointments,
            MyTrainings = userTrainings
        };

        return View(viewModel);
    }
}