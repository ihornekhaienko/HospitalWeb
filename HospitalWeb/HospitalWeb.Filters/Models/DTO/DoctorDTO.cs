using HospitalWeb.Filters.Models.DTO;

namespace HospitalWeb.Filters.Models.DTO
{
    public class DoctorDTO : UserDTO
    {
        public string Specialty{ get; set; }

        public string Hospital { get; set; }

        public string Locality { get; set; }
    }
}
