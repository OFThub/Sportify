using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportify.Data;
using Sportify.Models;
using Sportify.ViewModels;

namespace Sportify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public APIController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("total-revenue")]
        public async Task<ActionResult<int>> GetTotalRevenue()
        {
            var totalRevenue = await _context.Randevular
                .Include(g => g.gym)
                .Include(t => t.trainer)
                .Include(s => s.service)
                .Where(a => a.ServiceId.HasValue)
                .Select(a => a.service.ServicePrice)
                .SumAsync();
            return Ok(totalRevenue);
        }

        [HttpGet("total-trainer")]
        public async Task<ActionResult<int>> GetTotalTrainer()
        {
            var totalTrainer = await _context.Egitmenler
                .CountAsync();                    

            return Ok(totalTrainer);
        }

        [HttpGet("total-member")]
        public async Task<ActionResult<int>> GetTotalMember()
        {
            var totalTrainer = await _context.Users
                .CountAsync();

            return Ok(totalTrainer);
        }

        [HttpGet]
        public List<APIViewModel> Get()
        {
            var appointments = _context.Randevular
                .Include(g => g.gym)
                .Include(t => t.trainer)
                .Include(s => s.service)
                .Select(n=>new APIViewModel 
                {
                    AppointmentId = n.AppointmentId,
                    TrainerId = (int)n.TrainerId,
                    TrainerName = n.trainer.TrainerName,
                    GymId = (int)n.GymId,
                    GymName = n.gym.GymName,
                    ServiceId = (int)n.ServiceId,
                    ServiceName =n.service.ServiceName,
                    ServiceTime = n.service.ServiceTime,
                    ServicePrice = n.service.ServicePrice,
                    UserName = n.UserName
                    
                })
                .ToList();
            return appointments;
        }

        [HttpGet("{id}")]
        public APIViewModel Get(int id) 
        {
            var appointment = _context.Randevular
                .Include(g => g.gym)
                .Include(t => t.trainer)
                .Include(s => s.service)
                .Where(n => n.AppointmentId == id)
                .Select(n => new APIViewModel
                {
                    AppointmentId = n.AppointmentId,
                    TrainerId = (int)n.TrainerId,
                    TrainerName = n.trainer.TrainerName,
                    GymId = (int)n.GymId,
                    GymName = n.gym.GymName,
                    ServiceId = (int)n.ServiceId,
                    ServiceName = n.service.ServiceName,
                    ServiceTime = n.service.ServiceTime,
                    ServicePrice = n.service.ServicePrice,
                    UserName = n.UserName

                })
                .FirstOrDefault();

            return appointment;
        }

        [HttpPost]
        public void Post([FromBody] APIViewModel value)
        {
            var newAppointment = new Appointment
            {
                TrainerId = value.TrainerId,
                GymId = value.GymId,
                ServiceId = value.ServiceId,

                UserName = value.UserName
            };

            _context.Randevular.Add(newAppointment);
            _context.SaveChanges();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] APIViewModel value) 
        {
            var appointment = _context.Randevular
                .Include(g => g.gym)
                .Include(t => t.trainer)
                .Include(s => s.service)
                .Where(n => n.AppointmentId == id)
                .FirstOrDefault();

            if(appointment == null)
            {
                return NotFound();
            }
            else
            {
                appointment.TrainerId = value.TrainerId;
                appointment.GymId = value.GymId;
                appointment.ServiceId = value.ServiceId;
                appointment.UserName = value.UserName;

                _context.SaveChanges();
                return Ok();
            }
            
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) 
        {
            var appointment = _context.Randevular
                .Include(g => g.gym)
                .Include(t => t.trainer)
                .Include(s => s.service)
                .Where(n => n.AppointmentId == id)
                .FirstOrDefault();

            if (appointment == null)
            {
                return NotFound();
            }
            else
            {
                _context.Randevular.Remove(appointment);
                _context.SaveChanges();
                _context.SaveChanges();
                return Ok();
            }
        }
    }
}
