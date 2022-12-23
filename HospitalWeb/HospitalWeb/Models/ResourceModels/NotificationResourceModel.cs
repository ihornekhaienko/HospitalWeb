using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Models.ResourceModels
{
    public class NotificationResourceModel
    {
        public int NotificationId { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public NotificationType Type { get; set; }


        public string AppUserId { get; set; }
    }
}
