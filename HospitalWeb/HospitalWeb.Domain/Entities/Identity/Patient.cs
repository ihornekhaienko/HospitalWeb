using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWeb.Domain.Entities.Identity
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
            Appointments = new List<Appointment>();
            Grades = new List<Grade>();
        }

        public Sex Sex { get; set; }

        public DateTime BirthDate { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public ICollection<Grade> Grades { get; set; }
    }
}
