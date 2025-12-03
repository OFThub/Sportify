using System.ComponentModel.DataAnnotations;

namespace Sportify.Models
{
    public class Gym
    {
        public Gym()
        {
            Trainers = null;
            Services = null;
        }
        public int GymId { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Salon adý zorunludur.")]
        [StringLength(100, ErrorMessage = "Salon adý 100 karakterden uzun olamaz.")]
        [Display(Name = "Salon Adý")]
        public string GymName { get; set; }

        [Required(ErrorMessage = "Açýlýþ saati zorunludur.")]
        [DataType(DataType.Time)]
        [Display(Name = "Açýlýþ Saati")]
        public TimeSpan OpenTime { get; set; }

        [Required(ErrorMessage = "Kapanýþ saati zorunludur.")]
        [DataType(DataType.Time)]
        [Display(Name = "Kapanýþ Saati")]
        public TimeSpan CloseTime { get; set; }
        public ICollection<Trainer> ?Trainers { get; set; }
        public ICollection<Service> ?Services { get; set; }
    }
}