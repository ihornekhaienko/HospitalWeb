using HospitalWeb.Filters.Models.DTO;

namespace HospitalWeb.Filters.Models.FilterModels
{
    public class HospitalFilterModel
    {
        public string SearchString { get; private set; }
        public List<LocalityDTO> Localities { get; private set; }
        public int? SelectedLocality { get; private set; }

        public HospitalFilterModel(string searchString, List<LocalityDTO> localities, int? localiity)
        {
            localities.Insert(0, new LocalityDTO { LocalityId = 0, LocalityName = "All" });
            Localities = localities;
            SelectedLocality = localiity;
            SearchString = searchString;
        }
    }
}
