using System.ComponentModel.DataAnnotations;

namespace BookShelf.Models.ViewModels
{
    public class AboutUsViewModel
    {
        [StringLength(500)]
        public string OurStory { get; set; }

        [StringLength(500)]
        public string OurMission { get; set; }

        [StringLength(500)]
        public string OurVision { get; set; }

        [StringLength(500)]
        public string WhyChooseUs { get; set; }
    }
}