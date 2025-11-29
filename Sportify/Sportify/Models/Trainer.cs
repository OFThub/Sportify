using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportify.Models
{
    public class Trainer
    {
        [Key]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Spor Salonu ID'si zorunludur.")]
        [ForeignKey("Gym")]
        public int GymId { get; set; }

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

        [Required(ErrorMessage = "Hizmet adý zorunludur.")]
        [StringLength(150, ErrorMessage = "Hizmet adý 150 karakteri geçemez.")]
        [Display(Name = "Hizmet Adý")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Hizmet süresi zorunludur.")]
        [Range(15, 120, ErrorMessage = "Hizmet süresi 15 ile 120 dakika arasýnda olmalýdýr.")] 
        [Display(Name = "Hizmet Süresi (Dakika)")]
        public int ServiceTime { get; set; }

        [Required(ErrorMessage = "Hizmet fiyatý zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat pozitif bir deðer olmalýdýr.")]
        [Display(Name = "Hizmet Fiyatý")]
        public int ServicePrice { get; set; }
    }
}