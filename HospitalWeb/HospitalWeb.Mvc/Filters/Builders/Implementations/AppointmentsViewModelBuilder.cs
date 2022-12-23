using HospitalWeb.Domain.Entities;
using HospitalWeb.Mvc.Filters.Builders.Interfaces;
using HospitalWeb.Mvc.Filters.Models;
using HospitalWeb.Mvc.Filters.Models.DTO;
using HospitalWeb.Mvc.Filters.Models.FilterModels;
using HospitalWeb.Mvc.Filters.Models.SortModels;
using HospitalWeb.Mvc.Filters.Models.ViewModels;
using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Mvc.Models.SortStates;

namespace HospitalWeb.Mvc.Filters.Builders.Implementations
{
    public class AppointmentsViewModelBuilder : ViewModelBuilder<AppointmentsViewModel>
    {
        private readonly ApiUnitOfWork _api;
        private readonly string _userId;
        private readonly int? _state;
        private readonly int? _locality;
        private readonly DateTime? _fromTime;
        private readonly DateTime? _toTime;
        private readonly AppointmentSortState _sortOrder;
        private IEnumerable<AppointmentDTO> _appointments;
        private PageModel _pageModel;
        private AppointmentFilterModel _filterModel;
        private AppointmentSortModel _sortModel;
        private int _count = 0;

        public AppointmentsViewModelBuilder(
           ApiUnitOfWork api,
           int pageNumber = 1,
           string searchString = null,
           AppointmentSortState sortOrder = AppointmentSortState.DateAsc,
           string userId = null,
           int? state = null,
           int? locality = null,
           DateTime? fromTime = null,
           DateTime? toTime = null,
           int pageSize = 10
           ) : base(pageNumber, pageSize, searchString)
        {
            _api = api;
            _sortOrder = sortOrder;
            _userId = userId;
            _state = state;
            _locality = locality;
            _fromTime = fromTime;
            _toTime = toTime;
        }

        public override void BuildEntityModel()
        {
            var response = _api.Appointments.Filter(_searchString, _userId, _state, _locality, _fromTime, _toTime, _sortOrder, _pageSize, _pageNumber);

            if (response.IsSuccessStatusCode)
            {
                _appointments = _api.Appointments.ReadMany(response)
                    .Select(a => new AppointmentDTO
                    {
                        AppointmentId = a.AppointmentId,
                        Diagnosis = a.Diagnosis?.DiagnosisName,
                        Prescription = a.Prescription,
                        AppointmentDate = a.AppointmentDate,
                        State = a.State.ToString(),
                        DoctorId = a.Doctor.Id,
                        Doctor = a.Doctor.ToString(),
                        DoctorSpecialty = a.Doctor.Specialty.SpecialtyName,
                        DoctorImage = a.Doctor.Image,
                        PatientId = a.Patient.Id,
                        Patient = a.Patient.ToString(),
                        PatientBirthDate = a.Patient.BirthDate.ToShortDateString(),
                        PatientSex = a.Patient.Sex.ToString(),
                        PatientImage = a.Patient.Image,
                        Hospital = a.Doctor.Hospital.HospitalName,
                        MeetingStartLink = a.Meetings.FirstOrDefault()?.StartLink,
                        MeetingJoinLink = a.Meetings.FirstOrDefault()?.JoinLink,
                        Price = a.Price,
                        IsPaid = a.IsPaid
                    });

                _count = Convert.ToInt32(response.Headers.GetValues("TotalCount").FirstOrDefault());
            }
            else
            {
                throw new Exception("Failed loading appointments");
            }
        }

        public override void BuildFilterModel()
        {
            var states = Enum.GetValues(typeof(State))
                .Cast<State>()
                .Select(s => new StateDTO { Value = (int)s, Name = s.ToString() })
                .ToList();

            var response = _api.Localities.Get();
            var localities = _api.Localities
                .ReadMany(response)
                .OrderBy(l => l.LocalityName)
                .Select(l => new LocalityDTO { LocalityId = l.LocalityId, LocalityName = l.LocalityName })
                .ToList();

            _filterModel = new AppointmentFilterModel(_searchString, states, _state, localities, _locality, _fromTime, _toTime);
        }

        public override void BuildPageModel()
        {
            _pageModel = new PageModel(_count, _pageNumber, _pageSize);
        }

        public override void BuildSortModel()
        {
            _sortModel = new AppointmentSortModel(_sortOrder);
        }

        public override AppointmentsViewModel GetViewModel()
        {
            if (_pageModel == null || _sortModel == null || _filterModel == null)
            {
                throw new Exception("Failed building view model, some of models is null");
            }

            return new AppointmentsViewModel
            {
                Appointments = _appointments,
                PageModel = _pageModel,
                SortModel = _sortModel,
                FilterModel = _filterModel
            };
        }
    }
}
