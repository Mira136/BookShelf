using System.ComponentModel.DataAnnotations;

namespace BookShelf.Models.ViewModels
{
    public class AdminProfile1ViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [Phone]
        public string Number { get; set; }
    }
}