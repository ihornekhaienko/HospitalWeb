using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWeb.Models.Identity
{
    [Table("Admins")]
    public class Admin : AppUser
    {
    }
}
