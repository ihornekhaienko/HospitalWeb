using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;

namespace HospitalWeb.Filters.Models.ViewModels
{
    public class DoctorsViewModel
    {
        public IEnumerable<DoctorDTO> Doctors { get; set; }
        public PageModel PageModel { get; set; }
        public DoctorFilterModel FilterModel { get; set; }
        public DoctorSortModel SortModel { get; set; }
    }
}
