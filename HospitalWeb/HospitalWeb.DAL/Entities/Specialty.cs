using HospitalWeb.DAL.Entities.Identity;

namespace HospitalWeb.DAL.Entities
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
