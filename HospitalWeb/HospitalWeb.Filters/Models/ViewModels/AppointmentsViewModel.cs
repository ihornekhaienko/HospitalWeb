using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;

namespace HospitalWeb.Filters.Models.ViewModels
{
    public class AppointmentsViewModel
    {
        public IEnumerable<AppointmentDTO> Appointments { get; set; }
        public PageModel PageModel { get; set; }
        public AppointmentFilterModel FilterModel { get; set; }
        public AppointmentSortModel SortModel { get; set; }
    }
}
