using HospitalWeb.Mvc.Filters.Models.DTO;

namespace HospitalWeb.Mvc.Filters.Models.DTO
{
    public class PatientDTO : UserDTO
    {
        public string Address { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
