namespace BookShelf.Models.ViewModels
{
    public class NotificationItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "system"; // order / offer / system
        public string TimeAgo { get; set; } = "Just now";
    }

    public class AdminNotificationsViewModel
    {
        public List<NotificationItemViewModel> Notifications { get; set; } = new();
        public NotificationItemViewModel NewNotification { get; set; } = new();
    }
}