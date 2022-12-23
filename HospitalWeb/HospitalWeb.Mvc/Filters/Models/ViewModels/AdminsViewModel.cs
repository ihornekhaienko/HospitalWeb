using HospitalWeb.Mvc.Filters.Models.DTO;
using HospitalWeb.Mvc.Filters.Models.FilterModels;
using HospitalWeb.Mvc.Filters.Models.SortModels;

namespace HospitalWeb.Mvc.Filters.Models.ViewModels
{
    public class AdminsViewModel
    {
        public IEnumerable<AdminDTO> Admins { get; set; }
        public PageModel PageModel { get; set; }
        public AdminFilterModel FilterModel { get; set; }
        public AdminSortModel SortModel { get; set; } 
    }
}
