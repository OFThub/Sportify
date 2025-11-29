using System.ComponentModel.DataAnnotations;

namespace Sportify.ViewModels
{
    public class VerifyEmail
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

    }
}