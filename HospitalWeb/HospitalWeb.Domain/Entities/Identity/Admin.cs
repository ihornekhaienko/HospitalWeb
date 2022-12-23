using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWeb.Domain.Entities.Identity
{
    [Table("Admins")]
    public class Admin : AppUser
    {
        public bool IsSuperAdmin { get; set; }
    }
}
