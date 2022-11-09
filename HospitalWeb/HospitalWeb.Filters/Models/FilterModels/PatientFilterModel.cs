using HospitalWeb.DAL.Entities;

namespace HospitalWeb.Filters.Models.FilterModels
{
    public class PatientFilterModel
    {
        public string SearchString { get; private set; }
        public List<Locality> Localities { get; private set; }
        public int? SelectedLocality { get; private set; }

        public PatientFilterModel(string searchString, List<Locality> localities, int? localiity)
        {
            localities.Insert(0, new Locality { LocalityId = 0, LocalityName = "All" });
            Localities = localities;
            SelectedLocality = localiity;
            SearchString = searchString;
        }
    }
}
