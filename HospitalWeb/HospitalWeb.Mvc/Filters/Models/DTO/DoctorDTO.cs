using HospitalWeb.Mvc.Filters.Models.DTO;

namespace HospitalWeb.Mvc.Filters.Models.DTO
{
    public class DoctorDTO : UserDTO
    {
        public string Specialty{ get; set; }

        public string Hospital { get; set; }

        public string Locality { get; set; }

        public double Rating { get; set; }

        public decimal ServicePrice { get; set; }
    }
}
