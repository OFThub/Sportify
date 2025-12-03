using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportify.Models
{
    public class Service
    {
        public Service()
        {
            GymId = null;
            gym = null;
            trainer = null;
            TrainerId = null;
        }
        [Key]
        public int ServiceId { get; set; }
        public string UserName { get; set; }
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Hizmet süresi zorunludur.")]
        [Range(15, 120, ErrorMessage = "Hizmet süresi 15 ile 120 dakika arasýnda olmalýdýr.")] 
        [Display(Name = "Hizmet Süresi (Dakika)")]
        public int ServiceTime { get; set; }

        [Required(ErrorMessage = "Hizmet fiyatý zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat pozitif bir deðer olmalýdýr.")]
        [Display(Name = "Hizmet Fiyatý")]
        public int ServicePrice { get; set; }
        
        public int ?GymId { get; set; }
        public Gym ?gym { get; set; }
        public int? TrainerId { get; set; }
        public Trainer? trainer { get; set; }
    }
}