using HospitalWeb.DAL.Services.Interfaces;

namespace HospitalWeb.Clients.Implementations
{
    public class ApiUnitOfWork
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        private AppUsersApiClient _appUsersApiClient;
        private AddressesApiClient _addressesApiClient;
        private AdminsApiClient _adminsApiClient;
        private AppointmentsApiClient _appointmentsApiClient;
        private DiagnosesApiClient _diagnosesApiClient;
        private DoctorsApiClient _doctorsApiClient;
        private HospitalsApiClient _hospitalsApiClient;
        private LocalitiesApiClient _localitiesApiClient;
        private MeetingsApiClient _meetingsApiClient;
        private NotificationsApiClient _notificationsApiClient;
        private PatientsApiClient _patientsApiClient;
        private SchedulesApiClient _schedulesApiClient;
        private SpecialtiesApiClient _specialtiesApiClient;

        public ApiUnitOfWork(IConfiguration config, IHttpClientFactory clientFactory)
        {
            _config = config;
            _clientFactory = clientFactory;
        }

        public AppUsersApiClient AppUsers
        {
            get
            {
                if (_appUsersApiClient == null)
                    _appUsersApiClient = new AppUsersApiClient(_config, _clientFactory);
                return _appUsersApiClient;
            }
        }

        public AddressesApiClient Addresses
        {
            get
            {
                if (_addressesApiClient == null)
                    _addressesApiClient = new AddressesApiClient(_config, _clientFactory);
                return _addressesApiClient;
            }
        }

        public AdminsApiClient Admins
        {
            get
            {
                if (_adminsApiClient == null)
                    _adminsApiClient = new AdminsApiClient(_config, _clientFactory);
                return _adminsApiClient;
            }
        }

        public AppointmentsApiClient Appointments
        {
            get
            {
                if (_appointmentsApiClient == null)
                    _appointmentsApiClient = new AppointmentsApiClient(_config, _clientFactory);
                return _appointmentsApiClient;
            }
        }

        public DiagnosesApiClient Diagnoses
        {
            get
            {
                if (_diagnosesApiClient == null)
                    _diagnosesApiClient = new DiagnosesApiClient(_config, _clientFactory);
                return _diagnosesApiClient;
            }
        }

        public DoctorsApiClient Doctors
        {
            get
            {
                if (_doctorsApiClient == null)
                    _doctorsApiClient = new DoctorsApiClient(_config, _clientFactory);
                return _doctorsApiClient;
            }
        }
        
        public HospitalsApiClient Hospitals
        {
            get
            {
                if (_hospitalsApiClient == null)
                    _hospitalsApiClient = new HospitalsApiClient(_config, _clientFactory);
                return _hospitalsApiClient;
            }
        }

        public LocalitiesApiClient Localities
        {
            get
            {
                if (_localitiesApiClient == null)
                    _localitiesApiClient = new LocalitiesApiClient(_config, _clientFactory);
                return _localitiesApiClient;
            }
        }

        public MeetingsApiClient Meetings
        {
            get
            {
                if (_meetingsApiClient == null)
                    _meetingsApiClient = new MeetingsApiClient(_config, _clientFactory);
                return _meetingsApiClient;
            }
        }

        public NotificationsApiClient Notifications
        {
            get
            {
                if (_notificationsApiClient == null)
                    _notificationsApiClient = new NotificationsApiClient(_config, _clientFactory);
                return _notificationsApiClient;
            }
        }

        public PatientsApiClient Patients
        {
            get
            {
                if (_patientsApiClient == null)
                    _patientsApiClient = new PatientsApiClient(_config, _clientFactory);
                return _patientsApiClient;
            }
        }

        public SchedulesApiClient Schedules
        {
            get
            {
                if (_schedulesApiClient == null)
                    _schedulesApiClient = new SchedulesApiClient(_config, _clientFactory);
                return _schedulesApiClient;
            }
        }

        public SpecialtiesApiClient Specialties
        {
            get
            {
                if (_specialtiesApiClient == null)
                    _specialtiesApiClient = new SpecialtiesApiClient(_config, _clientFactory);
                return _specialtiesApiClient;
            }
        }
    }
}