using HospitalWeb.Mvc.Filters.Models.DTO;

namespace HospitalWeb.Mvc.Filters.Models.FilterModels
{
    public class PatientFilterModel
    {
        public string SearchString { get; private set; }
        public List<LocalityDTO> Localities { get; private set; }
        public int? SelectedLocality { get; private set; }

        public PatientFilterModel(string searchString, List<LocalityDTO> localities, int? locality)
        {
            localities.Insert(0, new LocalityDTO { LocalityId = 0, LocalityName = "All" });
            Localities = localities;
            SelectedLocality = locality;
            SearchString = searchString;
        }
    }
}
