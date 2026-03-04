namespace BookShelf.Models.ViewModels
{
    public class AboutViewModel
    {
        // Hero Section
        public string Title { get; set; }
        public string Subtitle { get; set; }

        // Story
        public string StoryDescription { get; set; }

        // Mission & Vision
        public string Mission { get; set; }
        public string Vision { get; set; }

        // Team Info
        public List<TeamMember> TeamMembers { get; set; }

        // Why Choose Us
        public List<string> WhyChooseUsPoints { get; set; }

        // What We Offer
        public List<OfferItem> OfferItems { get; set; }
    }

    public class TeamMember
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class OfferItem
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
    }
}