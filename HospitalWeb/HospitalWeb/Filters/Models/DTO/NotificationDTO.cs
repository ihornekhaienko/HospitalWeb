using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Filters.Models.DTO
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public NotificationType Type { get; set; }
    }
}
