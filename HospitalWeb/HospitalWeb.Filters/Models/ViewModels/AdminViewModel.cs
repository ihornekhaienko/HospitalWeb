using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;

namespace HospitalWeb.Filters.Models.ViewModels
{
    public class AdminViewModel
    {
        public IEnumerable<Admin>? Admins { get; set; }
        public PageModel? PageModel { get; set; }
        public AdminFilterModel? FilterModel { get; set; }
        public AdminSortModel? SortModel { get; set; } 
    }
}
