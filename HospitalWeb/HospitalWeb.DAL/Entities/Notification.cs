using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.DAL.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }


        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }
    }
}
