using HospitalWeb.Filters.Models.DTO;

namespace HospitalWeb.Filters.Models.FilterModels
{
    public class DoctorFilterModel
    {
        public string SearchString { get; private set; }
        public List<SpecialtyDTO> Specialties { get; private set; }
        public int? SelectedSpecialty { get; private set; }

        public DoctorFilterModel(string searchString, List<SpecialtyDTO> specialties, int? specialty)
        {
            specialties.Insert(0, new SpecialtyDTO { SpecialtyId = 0, SpecialtyName = "All"});
            Specialties = specialties;
            SelectedSpecialty = specialty;
            SearchString = searchString;
        }
    }
}
