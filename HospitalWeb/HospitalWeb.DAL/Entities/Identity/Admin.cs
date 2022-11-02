using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWeb.DAL.Entities.Identity
{
    [Table("Admins")]
    public class Admin : AppUser
    {
    }
}
