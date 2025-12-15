using Microsoft.EntityFrameworkCore;
using Sportify.Models;
using System.ComponentModel.DataAnnotations;

namespace Sportify.ViewModels
{
    public class APIViewModel
    {
        public int ?AppointmentId { get; set; }
        public int TrainerId { get; set; }
        public string ?TrainerName { get; set; }
        public int GymId { get; set; }
        public string ?GymName { get; set; }
        public int ServiceId { get; set; }
        public string ?ServiceName { get; set; }
        public int ServiceTime { get; set; }
        public int ServicePrice { get; set; }
        public string UserName { get; set; }
    }
}