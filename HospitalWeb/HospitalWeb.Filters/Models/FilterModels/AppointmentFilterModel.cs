using HospitalWeb.Filters.Models.DTO;

namespace HospitalWeb.Filters.Models.FilterModels
{
    public class AppointmentFilterModel
    {
        public string SearchString { get; private set; }
        public List<StateDTO> States { get; private set; }
        public int? SelectedState { get; private set; }
        public DateTime? FromDate { get; private set; }
        public DateTime? ToDate { get; private set; }

        public AppointmentFilterModel(string searchString, 
            List<StateDTO> states, 
            int? state,
            DateTime? fromDate,
            DateTime? toDate)
        {
            states.Insert(0, new StateDTO { Value = 0, Name = "All" });
            States = states;
            SelectedState = state;
            SearchString = searchString;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}
