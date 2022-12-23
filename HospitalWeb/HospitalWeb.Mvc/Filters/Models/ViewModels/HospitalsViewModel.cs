using HospitalWeb.Mvc.Filters.Models.DTO;
using HospitalWeb.Mvc.Filters.Models.FilterModels;
using HospitalWeb.Mvc.Filters.Models.SortModels;

namespace HospitalWeb.Mvc.Filters.Models.ViewModels
{
    public class HospitalsViewModel
    {
        public IEnumerable<HospitalDTO> Hospitals { get; set; }
        public PageModel PageModel { get; set; }
        public HospitalFilterModel FilterModel { get; set; }
        public HospitalSortModel SortModel { get; set; }
    }
}
