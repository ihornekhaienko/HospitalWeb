using HospitalWeb.Mvc.Filters.Models.DTO;
using HospitalWeb.Mvc.Filters.Models.FilterModels;
using HospitalWeb.Mvc.Filters.Models.SortModels;

namespace HospitalWeb.Mvc.Filters.Models.ViewModels
{
    public class PatientsViewModel
    {
        public IEnumerable<PatientDTO> Patients { get; set; }
        public PageModel PageModel { get; set; }
        public PatientFilterModel FilterModel { get; set; }
        public PatientSortModel SortModel { get; set; }
    }
}
