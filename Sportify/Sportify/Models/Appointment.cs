using System.ComponentModel.DataAnnotations;

namespace Sportify.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int TrainerId { get; set; }
        public string UserName { get; set; }
    }
}
