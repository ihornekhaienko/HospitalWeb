using HospitalWeb.DAL.Entities;
using HospitalWeb.Filters.Models.DTO;

namespace HospitalWeb.Filters.Models.FilterModels
{
    public class HospitalFilterModel
    {
        public string SearchString { get; private set; }

        public List<LocalityDTO> Localities { get; private set; }
        public int? SelectedLocality { get; private set; }

        public List<HospitalTypeDTO> HospitalTypes { get; private set; }
        public int? SelectedType { get; private set; }

        public HospitalFilterModel(string searchString, List<LocalityDTO> localities, int? locality, int? type)
        {
            localities.Insert(0, new LocalityDTO { LocalityId = 0, LocalityName = "All" });
            Localities = localities;
            SelectedLocality = locality;

            HospitalTypes = Enum.GetValues(typeof(HospitalType))
                .Cast<HospitalType>()
                .Select(t => new HospitalTypeDTO { Value = (int)t, Name = t.ToString() })
                .ToList();
            HospitalTypes.Insert(0, new HospitalTypeDTO { Value = 0, Name = "All" });
            SelectedType = type;

            SearchString = searchString;
        }
    }
}
