using System.ComponentModel.DataAnnotations;

namespace Sportify.Models
{
    public class VerifyEmail
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

    }
}