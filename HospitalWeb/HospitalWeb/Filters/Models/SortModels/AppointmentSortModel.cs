using HospitalWeb.Models.SortStates;

namespace HospitalWeb.Filters.Models.SortModels
{
    public class AppointmentSortModel
    {
        public AppointmentSortState DateSort { get; private set; }
        public AppointmentSortState DiagnosisSort { get; private set; }
        public AppointmentSortState StateSort { get; private set; }
        public AppointmentSortState PatientSort { get; private set; }
        public AppointmentSortState DoctorSort { get; private set; }

        public AppointmentSortState Current { get; private set; }

        public AppointmentSortModel(AppointmentSortState sortOrder)
        {
            DateSort = sortOrder == AppointmentSortState.DateAsc ? AppointmentSortState.DateDesc : AppointmentSortState.DateAsc;
            DiagnosisSort = sortOrder == AppointmentSortState.DiagnosisAsc ? AppointmentSortState.DiagnosisDesc : AppointmentSortState.DiagnosisAsc;
            StateSort = sortOrder == AppointmentSortState.StateAsc ? AppointmentSortState.StateDesc : AppointmentSortState.StateAsc;
            PatientSort = sortOrder == AppointmentSortState.PatientAsc ? AppointmentSortState.PatientDesc : AppointmentSortState.PatientAsc;
            DoctorSort = sortOrder == AppointmentSortState.DoctorAsc ? AppointmentSortState.DoctorDesc : AppointmentSortState.DoctorAsc;
            Current = sortOrder;
        }
    }
}
