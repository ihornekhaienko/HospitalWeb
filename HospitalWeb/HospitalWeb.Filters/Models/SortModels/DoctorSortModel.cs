namespace HospitalWeb.Filters.Models.SortModels
{
    public enum DoctorSortState
    {
        Id,
        NameAsc,
        NameDesc,
        SurnameAsc,
        SurnameDesc,
        EmailAsc,
        EmailDesc,
        PhoneAsc,
        PhoneDesc,
        SpecialtyAsc,
        SpecialtyDesc,
    }

    public class DoctorSortModel
    {
        public DoctorSortState IdSort { get; private set; }
        public DoctorSortState NameSort { get; private set; }
        public DoctorSortState SurnameSort { get; private set; }
        public DoctorSortState EmailSort { get; private set; }
        public DoctorSortState PhoneSort { get; protected set; }
        public DoctorSortState SpecialtySort { get; private set; }

        public DoctorSortState Current { get; private set; }

        public DoctorSortModel(DoctorSortState sortOrder)
        {
            IdSort = DoctorSortState.Id;
            NameSort = sortOrder == DoctorSortState.NameAsc ? DoctorSortState.NameDesc : DoctorSortState.NameAsc;
            SurnameSort = sortOrder == DoctorSortState.SurnameAsc ? DoctorSortState.SurnameDesc : DoctorSortState.SurnameAsc;
            EmailSort = sortOrder == DoctorSortState.EmailAsc ? DoctorSortState.EmailDesc : DoctorSortState.EmailAsc;
            PhoneSort = sortOrder == DoctorSortState.PhoneAsc ? DoctorSortState.PhoneDesc : DoctorSortState.PhoneAsc;
            SpecialtySort = sortOrder == DoctorSortState.SpecialtyAsc ? DoctorSortState.SpecialtyDesc : DoctorSortState.SpecialtyAsc;
            Current = sortOrder;
        }
    }
}
