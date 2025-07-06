namespace TP4SCS.Library.Models.Response.Notification
{
    public class OrderNotificationResponse
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public DateTime NotificationTime { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}
