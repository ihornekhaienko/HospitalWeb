using HospitalWeb.DAL.Entities;

namespace HospitalWeb.Filters.Models.FilterModels
{
    public class DoctorFilterModel
    {
        public string? SearchString { get; private set; }
        public List<Specialty> Specialties { get; private set; }
        public int? SelectedSpecialty { get; private set; }

        public DoctorFilterModel(string? searchString, List<Specialty> specialties, int? specialty)
        {
            specialties.Insert(0, new Specialty { SpecialtyId = 0, SpecialtyName = "All"});
            Specialties = specialties;
            SelectedSpecialty = specialty;
            SearchString = searchString;
        }
    }
}
