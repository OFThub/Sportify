using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Sportify.Models
{
    [Index(nameof(GymId))]
    [Index(nameof(ServiceId))]
    [Index(nameof(TrainerId))]
    public class Appointment
    {
        public Appointment()
        {
            trainer = null;
            TrainerId = null;
            gym = null;
            GymId = null;
            service = null;
            ServiceId = null;
        }
        public int AppointmentId { get; set; }

        public int ?TrainerId { get; set; }
        public Trainer ?trainer { get; set; }
        public int ?GymId { get; set; }
        public Gym ?gym { get; set; }
        public int ?ServiceId { get; set; }
        public Service ?service { get; set; }
        public string UserName { get; set; }
    }
}
