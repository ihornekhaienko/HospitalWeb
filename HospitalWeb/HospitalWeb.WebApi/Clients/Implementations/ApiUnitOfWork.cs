﻿using HospitalWeb.WebApi.Clients.Implementations;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class ApiUnitOfWork
    {
        private readonly IConfiguration _config;
        private AddressesApiClient _addressesApiClient;
        private AdminsApiClient _adminsApiClient;
        private AppointmentsApiClient _appointmentsApiClient;
        private DiagnosesApiClient _diagnosesApiClient;
        private DoctorsApiClient _doctorsApiClient;
        private LocalitiesApiClient _localitiesApiClient;
        private PatientsApiClient _patientsApiClient;
        private SchedulesApiClient _schedulesApiClient;
        private SpecialtiesApiClient _specialtiesApiClient;

        public ApiUnitOfWork(IConfiguration config)
        {
            _config = config;
        }

        public AddressesApiClient Addresses
        {
            get
            {
                if (_addressesApiClient == null)
                    _addressesApiClient = new AddressesApiClient(_config);
                return _addressesApiClient;
            }
        }

        public AdminsApiClient Admins
        {
            get
            {
                if (_adminsApiClient == null)
                    _adminsApiClient = new AdminsApiClient(_config);
                return _adminsApiClient;
            }
        }

        public AppointmentsApiClient Appointments
        {
            get
            {
                if (_appointmentsApiClient == null)
                    _appointmentsApiClient = new AppointmentsApiClient(_config);
                return _appointmentsApiClient;
            }
        }

        public DiagnosesApiClient Diagnoses
        {
            get
            {
                if (_diagnosesApiClient == null)
                    _diagnosesApiClient = new DiagnosesApiClient(_config);
                return _diagnosesApiClient;
            }
        }

        public DoctorsApiClient Doctors
        {
            get
            {
                if (_doctorsApiClient == null)
                    _doctorsApiClient = new DoctorsApiClient(_config);
                return _doctorsApiClient;
            }
        }

        public LocalitiesApiClient Localities
        {
            get
            {
                if (_localitiesApiClient == null)
                    _localitiesApiClient = new LocalitiesApiClient(_config);
                return _localitiesApiClient;
            }
        }

        public PatientsApiClient Patients
        {
            get
            {
                if (_patientsApiClient == null)
                    _patientsApiClient = new PatientsApiClient(_config);
                return _patientsApiClient;
            }
        }

        public SchedulesApiClient Schedules
        {
            get
            {
                if (_schedulesApiClient == null)
                    _schedulesApiClient = new SchedulesApiClient(_config);
                return _schedulesApiClient;
            }
        }

        public SpecialtiesApiClient Specialties
        {
            get
            {
                if (_specialtiesApiClient == null)
                    _specialtiesApiClient = new SpecialtiesApiClient(_config);
                return _specialtiesApiClient;
            }
        }
    }
}