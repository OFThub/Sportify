using System.ComponentModel.DataAnnotations;

namespace Sportify.Models
{
    public class Gym
    {
        public int GymId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Salon Adý")]
        public string GymName { get; set; }

        [Required]
        [Display(Name = "Açýlýþ Saati")]
        public TimeSpan OpenTime { get; set; }

        [Required]
        [Display(Name = "Kapanýþ Saati")]
        public TimeSpan CloseTime { get; set; }

        public ICollection<Service> GymServices { get; set; } = new List<Service>();
        public ICollection<Trainer> GymTrainers { get; set; } = new List<Trainer>();
    }
}
