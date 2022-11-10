using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;

namespace HospitalWeb.Filters.Models.ViewModels
{
    public class PatientsViewModel
    {
        public IEnumerable<PatientDTO> Patients { get; set; }
        public PageModel PageModel { get; set; }
        public PatientFilterModel FilterModel { get; set; }
        public PatientSortModel SortModel { get; set; }
    }
}
