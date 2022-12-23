using HospitalWeb.Mvc.Models.SortStates;

namespace HospitalWeb.Mvc.Filters.Models.SortModels
{
    public class PatientSortModel
    {
        public PatientSortState IdSort { get; private set; }
        public PatientSortState NameSort { get; private set; }
        public PatientSortState SurnameSort { get; private set; }
        public PatientSortState EmailSort { get; private set; }
        public PatientSortState PhoneSort { get; protected set; }
        public PatientSortState AddressSort { get; private set; }
        public PatientSortState BirthDateSort { get; private set; }

        public PatientSortState Current { get; private set; }

        public PatientSortModel(PatientSortState sortOrder)
        {
            IdSort = PatientSortState.Id;
            NameSort = sortOrder == PatientSortState.NameAsc ? PatientSortState.NameDesc : PatientSortState.NameAsc;
            SurnameSort = sortOrder == PatientSortState.SurnameAsc ? PatientSortState.SurnameDesc : PatientSortState.SurnameAsc;
            EmailSort = sortOrder == PatientSortState.EmailAsc ? PatientSortState.EmailDesc : PatientSortState.EmailAsc;
            PhoneSort = sortOrder == PatientSortState.PhoneAsc ? PatientSortState.PhoneDesc : PatientSortState.PhoneAsc;
            AddressSort = sortOrder == PatientSortState.AddressAsc ? PatientSortState.AddressDesc : PatientSortState.AddressAsc;
            BirthDateSort = sortOrder == PatientSortState.BirthDateAsc ? PatientSortState.BirthDateDesc : PatientSortState.BirthDateAsc;
            Current = sortOrder;
        }
    }
}
