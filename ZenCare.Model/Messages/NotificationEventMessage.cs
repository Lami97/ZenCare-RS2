namespace ZenCare.Model.Messages
{
    public class NotificationEventMessage
    {
        public int UserId { get; set; }

        public string EventKey { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; }
    }
}
