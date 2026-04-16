namespace BookShelf.Models
{
    public class ScoreboardEntry
    {
        public int Id { get; set; }

        public string UserId { get; set; }   // FK
        public ApplicationUser User { get; set; }

        public int Credits { get; set; }     // +1, -1 etc

        public string Action { get; set; }   // "Completed 10 Uploads"

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}