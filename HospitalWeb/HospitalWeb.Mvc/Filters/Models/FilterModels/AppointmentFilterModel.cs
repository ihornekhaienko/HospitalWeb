using HospitalWeb.Mvc.Filters.Models.DTO;

namespace HospitalWeb.Mvc.Filters.Models.FilterModels
{
    public class AppointmentFilterModel
    {
        public string SearchString { get; private set; }
        public List<StateDTO> States { get; private set; }
        public int? SelectedState { get; private set; }
        public List<LocalityDTO> Localities { get; private set; }
        public int? SelectedLocality { get; private set; }
        public DateTime? FromDate { get; private set; }
        public DateTime? ToDate { get; private set; }

        public AppointmentFilterModel(string searchString, 
            List<StateDTO> states, 
            int? state,
            List<LocalityDTO> localities, 
            int? locality,
            DateTime? fromDate,
            DateTime? toDate)
        {
            states.Insert(0, new StateDTO { Value = 0, Name = "All" });
            States = states;
            SelectedState = state;

            localities.Insert(0, new LocalityDTO { LocalityId = 0, LocalityName = "All" });
            Localities = localities;
            SelectedLocality = locality;

            SearchString = searchString;

            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}
