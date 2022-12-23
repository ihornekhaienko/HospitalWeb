using HospitalWeb.Mvc.Filters.Models.DTO;

namespace HospitalWeb.Mvc.Filters.Models.FilterModels
{
    public class DoctorFilterModel
    {
        public string SearchString { get; private set; }

        public List<SpecialtyDTO> Specialties { get; private set; }
        public int? SelectedSpecialty { get; private set; }

        public List<HospitalDTO> Hospitals { get; private set; }
        public int? SelectedHospital { get; private set; }

        public List<LocalityDTO> Localities { get; private set; }
        public int? SelectedLocality { get; private set; }

        public DoctorFilterModel(
            string searchString, 
            List<SpecialtyDTO> specialties,
            int? specialty,
            List<HospitalDTO> hospitals,
            int? hospital,
            List<LocalityDTO> localities,
            int? locality
            )
        {
            SearchString = searchString;

            specialties.Insert(0, new SpecialtyDTO { SpecialtyId = 0, SpecialtyName = "All"});
            Specialties = specialties;
            SelectedSpecialty = specialty;

            hospitals.Insert(0, new HospitalDTO { HospitalId = 0, HospitalName = "All" });
            Hospitals = hospitals;
            SelectedHospital = hospital;

            localities.Insert(0, new LocalityDTO { LocalityId = 0, LocalityName = "All" });
            Localities = localities;
            SelectedLocality = locality;
        }
    }
}
