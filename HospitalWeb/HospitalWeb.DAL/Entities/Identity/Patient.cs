using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWeb.DAL.Entities.Identity
{
    public enum Sex
    {
        Male,
        Female
    }

    [Table("Patients")]
    public class Patient : AppUser
    {
        public Patient()
        {
            Records = new List<Record>();
        }

        public Sex Sex { get; set; }
        public DateTime BirthDate { get; set; }

        public Address? Address { get; set; }
        public ICollection<Record>? Records { get; set; }
    }
}
