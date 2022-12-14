using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.Domain.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public byte[] Image { get; set; }
        public string CalendarId { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
