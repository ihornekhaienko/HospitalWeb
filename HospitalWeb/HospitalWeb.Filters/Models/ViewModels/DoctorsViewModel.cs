using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;

namespace HospitalWeb.Filters.Models.ViewModels
{
    public class DoctorsViewModel
    {
        public IEnumerable<Doctor> Doctors { get; set; }
        public PageModel PageModel { get; set; }
        public DoctorFilterModel FilterModel { get; set; }
        public DoctorSortModel SortModel { get; set; }
    }
}
