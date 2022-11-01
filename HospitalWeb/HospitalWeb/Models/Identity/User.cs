using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.Models.Identity
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ProfileImageUrl { get; set; }

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
