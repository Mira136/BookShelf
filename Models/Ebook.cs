using System.ComponentModel.DataAnnotations;

namespace BookShelf.Models
{
    public class Ebook
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        // English, Hindi, Gujarati etc.
        public string? Language { get; set; }

        // PDF file path e.g. let-them.pdf
        public string? PdfFilePath { get; set; }

        // Likes & Dislikes count
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Category
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Who uploaded this ebook
        public string? UploaderId { get; set; }
        public ApplicationUser? Uploader { get; set; }

        public string? Email { get; set; }
    }
}