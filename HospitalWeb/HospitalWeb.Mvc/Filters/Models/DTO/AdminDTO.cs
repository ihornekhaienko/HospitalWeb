using HospitalWeb.Mvc.Filters.Models.DTO;

namespace HospitalWeb.Mvc.Filters.Models.DTO
{
    public class AdminDTO : UserDTO
    {
        public bool IsSuperAdmin { get; set; }
    }
}
