using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalWeb.Domain.Entities.Identity
{
    [Table("Doctors")]
    public class Doctor : AppUser
    {
        public Doctor()
        {
            Appointments = new List<Appointment>();
            Schedules = new List<Schedule>();
            Grades = new List<Grade>();
        }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ServicePrice { get; set; }

        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }

        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public ICollection<Schedule> Schedules { get; set; }

        public ICollection<Grade> Grades { get; set; }

        public double Rating { 
            get
            {
                if (Grades.Count == 0)
                {
                    return 0;
                }

                return Grades.Average(a => (double)a.Stars);
            }
        }
    }
}
