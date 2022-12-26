using HospitalWeb.Domain.Entities.Identity;

namespace HospitalWeb.Domain.Entities
{
    public class Message
    {
        public int MessageId { get; set; }

        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
