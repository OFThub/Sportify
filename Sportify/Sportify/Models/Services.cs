using System.ComponentModel.DataAnnotations;

namespace Sportify.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Hizmet adý zorunludur.")]
        [StringLength(50, ErrorMessage = "Hizmet adý maksimum 50 karakter olabilir.")]
        [Display(Name = "Hizmet Adý")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Hizmet süresi zorunludur.")]
        [Range(1, 300, ErrorMessage = "Hizmet süresi 1 ile 300 dakika arasýnda olmalýdýr.")]
        [Display(Name = "Hizmet Süresi (dakika)")]
        public int ServiceTime { get; set; }

        [Required(ErrorMessage = "Hizmet ücreti zorunludur.")]
        [Range(0, 20000, ErrorMessage = "Hizmet ücreti 0 ile 20.000 TL arasýnda olmalýdýr.")]
        [Display(Name = "Hizmet Ücreti (TL)")]
        public int ServicePrice { get; set; }
    }
}
