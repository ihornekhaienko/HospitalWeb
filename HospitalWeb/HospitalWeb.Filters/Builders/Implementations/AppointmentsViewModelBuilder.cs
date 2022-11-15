using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Filters.Builders.Interfaces;
using HospitalWeb.Filters.Models;
using HospitalWeb.Filters.Models.DTO;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;
using HospitalWeb.Filters.Models.SortStates;
using HospitalWeb.Filters.Models.ViewModels;

namespace HospitalWeb.Filters.Builders.Implementations
{
    public class AppointmentsViewModelBuilder : ViewModelBuilder<AppointmentsViewModel>
    {
        private readonly UnitOfWork _uow;
        private readonly string _patientId;
        private readonly string _doctorId;
        private readonly int? _state;
        private readonly DateTime? _fromTime;
        private readonly DateTime? _toTime;
        private readonly AppointmentSortState _sortOrder;
        private IEnumerable<AppointmentDTO> _appointments;
        private PageModel _pageModel;
        private AppointmentFilterModel _filterModel;
        private AppointmentSortModel _sortModel;
        private int _count = 0;

        public AppointmentsViewModelBuilder(
           UnitOfWork uow,
           int pageNumber,
           string searchString,
           AppointmentSortState sortOrder,
           int? state = null,
           DateTime? fromTime = null,
           DateTime? toTime = null,
           string doctorId = null,
           string patientId = null,
           int pageSize = 10
           ) : base(pageNumber, pageSize, searchString)
        {
            _uow = uow;
            _sortOrder = sortOrder;
            _state = state;
            _fromTime = fromTime;
            _toTime = toTime;
            _doctorId = doctorId;
            _patientId = patientId;
        }

        public override void BuildEntityModel()
        {
            Func<Appointment, bool> filter = (a) =>
            {
                bool result = true;

                if (!string.IsNullOrWhiteSpace(_searchString))
                {
                    result = a.Diagnosis.DiagnosisName.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
                    a.Doctor.Specialty.SpecialtyName.Contains(_searchString, StringComparison.OrdinalIgnoreCase);
                }

                if (!string.IsNullOrWhiteSpace(_doctorId))
                {
                    result = result && a.Doctor.Id == _doctorId;
                }

                if (!string.IsNullOrWhiteSpace(_patientId))
                {
                    result = result && a.Patient.Id == _patientId;
                }

                if (_state != null && _state != 0)
                {
                    result = result && (int)a.State == _state;
                }

                if (_fromTime != null)
                {
                    result = result && DateTime.Compare((DateTime)_fromTime, a.AppointmentDate) <= 0;
                }

                if (_toTime != null)
                {
                    result = result && DateTime.Compare((DateTime)_toTime, a.AppointmentDate) >= 0;
                }

                return result;
            };

            Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>> orderBy = (appointments) =>
            {
                _count = appointments.Count();

                switch (_sortModel.Current)
                {
                    case AppointmentSortState.DateDesc:
                        appointments = appointments.OrderByDescending(a => a.AppointmentDate);
                        break;
                    case AppointmentSortState.DiagnosisAsc:
                        appointments = appointments.OrderBy(a => a.Diagnosis.DiagnosisName);
                        break;
                    case AppointmentSortState.DiagnosisDesc:
                        appointments = appointments.OrderByDescending(a => a.Diagnosis.DiagnosisName);
                        break;
                    case AppointmentSortState.StateAsc:
                        appointments = appointments.OrderBy(a => a.State);
                        break;
                    case AppointmentSortState.StateDesc:
                        appointments = appointments.OrderByDescending(a => a.State);
                        break;
                    case AppointmentSortState.DoctorAsc:
                        appointments = appointments.OrderBy(a => a.Doctor.ToString());
                        break;
                    case AppointmentSortState.DoctorDesc:
                        appointments = appointments.OrderByDescending(a => a.Doctor.ToString());
                        break;
                    case AppointmentSortState.PatientAsc:
                        appointments = appointments.OrderBy(a => a.Patient.ToString());
                        break;
                    case AppointmentSortState.PatientDesc:
                        appointments = appointments.OrderByDescending(a => a.Patient.ToString());
                        break;
                    default:
                        appointments = appointments.OrderBy(a => a.AppointmentDate);
                        break;
                }

                return (IOrderedQueryable<Appointment>)appointments;
            };

            _appointments = _uow.Appointments
               .GetAll(filter: filter, orderBy: orderBy, first: _pageSize, offset: (_pageNumber - 1) * _pageSize)
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
                   PatientImage = a.Patient.Image
               });
        }

        public override void BuildFilterModel()
        {
            var states = Enum.GetValues(typeof(State))
                .Cast<State>()
                .Select(s => new StateDTO { Value = (int)s, Name = s.ToString() })
                .ToList();

            _filterModel = new AppointmentFilterModel(_searchString, states, _state, _fromTime, _toTime);
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
