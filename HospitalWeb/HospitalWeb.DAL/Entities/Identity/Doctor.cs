using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWeb.DAL.Entities.Identity
{
    [Table("Doctors")]
    public class Doctor : AppUser
    {
        public Doctor()
        {
            Records = new List<Record>();
            Schedules = new List<Schedule>();
        }

        public Specialty? Specialty { get; set; }
        public ICollection<Record>? Records { get; set; }
        public ICollection<Schedule>? Schedules { get; set; }
    }
}
