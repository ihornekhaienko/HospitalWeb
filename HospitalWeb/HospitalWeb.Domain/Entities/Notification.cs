using HospitalWeb.Domain.Entities.Identity;

namespace HospitalWeb.Domain.Entities
{
    public enum NotificationType
    {
        Secondary,
        Danger,
        Success,
        Primary
    };

    public class Notification
    {
        public int NotificationId { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public NotificationType Type { get; set; }


        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }
    }
}
