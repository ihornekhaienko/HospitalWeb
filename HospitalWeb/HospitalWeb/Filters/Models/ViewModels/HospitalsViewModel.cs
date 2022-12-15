using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;

namespace HospitalWeb.Filters.Models.ViewModels
{
    public class HospitalsViewModel
    {
        public IEnumerable<HospitalDTO> Hospitals { get; set; }
        public PageModel PageModel { get; set; }
        public HospitalFilterModel FilterModel { get; set; }
        public HospitalSortModel SortModel { get; set; }
    }
}
