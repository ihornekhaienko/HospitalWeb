using HospitalWeb.Models.Identity;

namespace HospitalWeb.Models
{
    public class Specialty
    {
        public Specialty()
        {
            Doctors = new List<Doctor>();
        }

        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }
}
