using HospitalWeb.Filters.Models.DTO;

namespace HospitalWeb.Filters.Models.DTO
{
    public class AdminDTO : UserDTO
    {
        public bool IsSuperAdmin { get; set; }
    }
}
