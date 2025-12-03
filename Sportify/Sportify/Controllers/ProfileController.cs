using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportify.Data;
using Sportify.Models;
using Sportify.ViewModels;

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

        var userAppointments = await _context.Randevular
            .Include(x=>x.trainer)
            .Include(x=>x.service)
            .Include(x=>x.gym)
                                          .Where(t => t.UserName == userFullName)
                                          .ToListAsync();

        var userTrainings = await _context.Egitmenler
                                          .Where(t => t.TrainerName == userFullName)
                                          .ToListAsync();

        var userGyms = await _context.Salonlar
                                        .Where(t => t.Name == userFullName)
                                        .ToListAsync();

        var userServices = await _context.Servisler
                                        .Where(t => t.UserName == userFullName)
                                        .ToListAsync();

        var viewModel = new ProfileViewModel
        {
            MyAppointments = userAppointments,
            MyTrainings = userTrainings,
            MyGyms = userGyms,
            MyServices = userServices
        };

        return View(viewModel);
    }
}