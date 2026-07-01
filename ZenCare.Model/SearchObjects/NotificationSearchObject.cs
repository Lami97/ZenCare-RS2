using ZenCare.Model.Enums;

namespace ZenCare.Model.SearchObjects
{
    public class NotificationSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }

        public string? NotificationType { get; set; }

        public NotificationStatus? Status { get; set; }
    }
}
