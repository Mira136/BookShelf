using System.ComponentModel.DataAnnotations;

namespace BookShelf.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }

        // Which book
        public int BookId { get; set; }
        public Book? Book { get; set; }

        // Which user
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}