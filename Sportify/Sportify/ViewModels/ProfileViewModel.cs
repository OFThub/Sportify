using Sportify.Models;

namespace Sportify.ViewModels
{
    public class ProfileViewModel
    {
        public List<Appointment> MyAppointments { get; set; }
        public List<Trainer> MyTrainings { get; set; }
        public List<Gym> MyGyms { get; set; }
        public List<Service> MyServices { get; set; }
    }
}