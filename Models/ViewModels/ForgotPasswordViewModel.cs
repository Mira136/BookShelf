using System.ComponentModel.DataAnnotations;

namespace BookShelf.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}