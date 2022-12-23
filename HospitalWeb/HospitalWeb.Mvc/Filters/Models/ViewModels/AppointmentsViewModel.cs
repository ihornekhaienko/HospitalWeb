using HospitalWeb.Mvc.Filters.Models.DTO;
using HospitalWeb.Mvc.Filters.Models.FilterModels;
using HospitalWeb.Mvc.Filters.Models.SortModels;

namespace HospitalWeb.Mvc.Filters.Models.ViewModels
{
    public class AppointmentsViewModel
    {
        public IEnumerable<AppointmentDTO> Appointments { get; set; }
        public PageModel PageModel { get; set; }
        public AppointmentFilterModel FilterModel { get; set; }
        public AppointmentSortModel SortModel { get; set; }
    }
}
