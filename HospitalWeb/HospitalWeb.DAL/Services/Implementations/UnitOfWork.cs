using HospitalWeb.DAL.Data;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext _db;
        private AddressRepository _addressRepository;
        private AdminRepository _adminRepository;
        private AppointmentRepository _appointmentRepository;
        private DoctorRepository _doctorRepository;
        private LocalityRepository _localityRepository;
        private PatientRepository _patientRepository;
        private ScheduleRepository _scheduleRepository;
        private SpecialtyRepository _specialtyRepository;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
        }

        public AddressRepository Addresses
        {
            get
            {
                if (_addressRepository == null)
                    _addressRepository = new AddressRepository(_db);
                return _addressRepository;
            }
        }

        public AdminRepository Admins
        {
            get
            {
                if (_adminRepository == null)
                    _adminRepository = new AdminRepository(_db);
                return _adminRepository;
            }
        }

        public AppointmentRepository Appointments
        {
            get
            {
                if (_appointmentRepository == null)
                    _appointmentRepository = new AppointmentRepository(_db);
                return _appointmentRepository;
            }
        }

        public DoctorRepository Doctors
        {
            get
            {
                if (_doctorRepository == null)
                    _doctorRepository = new DoctorRepository(_db);
                return _doctorRepository;
            }
        }

        public LocalityRepository Localities
        {
            get
            {
                if (_localityRepository == null)
                    _localityRepository = new LocalityRepository(_db);
                return _localityRepository;
            }
        }

        public PatientRepository Patients
        {
            get
            {
                if (_patientRepository == null)
                    _patientRepository = new PatientRepository(_db);
                return _patientRepository;
            }
        }

        public ScheduleRepository Schedules
        {
            get
            {
                if (_scheduleRepository == null)
                    _scheduleRepository = new ScheduleRepository(_db);
                return _scheduleRepository;
            }
        }

        public SpecialtyRepository Specialties
        {
            get
            {
                if (_specialtyRepository == null)
                    _specialtyRepository = new SpecialtyRepository(_db);
                return _specialtyRepository;
            }
        }

        private bool _disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}