using HospitalWeb.Mvc.Filters.Models.DTO;
using HospitalWeb.Mvc.Filters.Models.FilterModels;
using HospitalWeb.Mvc.Filters.Models.SortModels;

namespace HospitalWeb.Mvc.Filters.Models.ViewModels
{
    public class DoctorsViewModel
    {
        public IEnumerable<DoctorDTO> Doctors { get; set; }
        public PageModel PageModel { get; set; }
        public DoctorFilterModel FilterModel { get; set; }
        public DoctorSortModel SortModel { get; set; }
    }
}
