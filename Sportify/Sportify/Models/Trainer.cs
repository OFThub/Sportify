using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore; 

namespace Sportify.Models
{
    [Index(nameof(GymId))]
    public class Trainer
    {
        public Trainer()
        {
            service = null;
        }
        [Key]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Eðitmen adý zorunludur.")]
        [StringLength(100, ErrorMessage = "Eðitmen adý 100 karakteri geçemez.")]
        [Display(Name = "Eðitmen Adý")]
        public string TrainerName { get; set; }

        [Required(ErrorMessage = "Çalýþma baþlangýç saati zorunludur.")]
        [DataType(DataType.Time)]
        [Display(Name = "Çalýþma Baþlangýç Saati")]
        public TimeSpan WorkStartTime { get; set; }

        [Required(ErrorMessage = "Çalýþma bitiþ saati zorunludur.")]
        [DataType(DataType.Time)]
        [Display(Name = "Çalýþma Bitiþ Saati")]
        public TimeSpan WorkEndTime { get; set; }
        public int GymId { get; set; }
        public Gym gym { get; set; } 
        public ICollection<Service> ?service { get; set; } 
    }
}