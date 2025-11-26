using System.ComponentModel.DataAnnotations;

namespace Sportify.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Required]
        [Display(Name = "Randevu Baþlangýcý")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "Randevu Bitiþi")]
        public DateTime EndTime { get; set; }
    }

}
