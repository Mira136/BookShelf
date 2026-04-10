using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShelf.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public string? Description { get; set; }

        // New price (discounted)
        public decimal SellingPrice { get; set; }

        // Old price (MRP)
        public decimal? ActualPrice { get; set; }

        public string? ImagePath { get; set; }

        // "New" or "Used"
        public string Condition { get; set; } = "New";

        // English, Hindi, Gujarati etc.
        public string? Language { get; set; }

        // Only filled when Category = Academic
        public string? EducationLevel { get; set; }

        // City e.g. Rajkot, Gujarat
        public string? Location { get; set; }

        // true = In Stock, false = Out of Stock
        public bool IsAvailable { get; set; } = true;

        // Average star rating
        public double Rating { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Which category this book belongs to
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Who uploaded/selling this book
        public string? UploaderId { get; set; }
        public ApplicationUser? Uploader { get; set; }

        // Customer reviews
        public List<Review> Reviews { get; set; } = new();

        [NotMapped]
        public bool IsInWishlist { get; set; }
    }
}