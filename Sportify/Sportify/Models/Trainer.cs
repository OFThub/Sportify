using System.ComponentModel.DataAnnotations;

namespace Sportify.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Antrenör Adý")]
        public string TrainerName { get; set; }

        [Required]
        [Display(Name = "Çalýþma Baþlangýç Saati")]
        public TimeSpan WorkStartTime { get; set; }

        [Required]
        [Display(Name = "Çalýþma Bitiþ Saati")]
        public TimeSpan WorkEndTime { get; set; }

        public ICollection<Service> TrainerServices { get; set; } = new List<Service>();
    }
}
