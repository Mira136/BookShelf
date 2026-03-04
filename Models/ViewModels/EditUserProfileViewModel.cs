using System.ComponentModel.DataAnnotations;

namespace BookShelf.Models.ViewModels
{
    public class EditUserProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Phone]
        public string Number { get; set; }

        public int Credit { get; set; }

        public string? ExistingPhotoPath { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
