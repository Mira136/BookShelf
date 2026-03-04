namespace BookShelf.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Number { get; set; }
        public int Credit { get; set; }

        public string? ProfileImage { get; set; }
    }
}
