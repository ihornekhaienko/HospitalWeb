using HospitalWeb.Domain.Data;
using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Interfaces;

namespace HospitalWeb.Domain.Services.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        private GenericRepository<AppUser> _appUserRepository;
        private GenericRepository<Address> _addressRepository;
        private GenericRepository<Admin> _adminRepository;
        private GenericRepository<Appointment> _appointmentRepository;
        private GenericRepository<Diagnosis> _diagnosisRepository;
        private GenericRepository<Doctor> _doctorRepository;
        private GenericRepository<Grade> _gradeRepository;
        private GenericRepository<Hospital> _hospitalRepository;
        private GenericRepository<Locality> _localityRepository;
        private GenericRepository<Meeting> _meetingRepository;
        private GenericRepository<Message> _messageRepository;
        private GenericRepository<Notification> _notificationRepository;
        private GenericRepository<Patient> _patientRepository;
        private GenericRepository<Schedule> _scheduleRepository;
        private GenericRepository<Specialty> _specialtyRepository;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
        }

        public virtual void Detach()
        {
            _db.ChangeTracker.Clear();
        }

        public virtual IRepository<AppUser> AppUsers
        {
            get
            {
                if (_appUserRepository == null)
                    _appUserRepository = new GenericRepository<AppUser>(_db);
                return _appUserRepository;
            }
        }

        public virtual IRepository<Address> Addresses
        {
            get
            {
                if (_addressRepository == null)
                    _addressRepository = new GenericRepository<Address>(_db);
                return _addressRepository;
            }
        }

        public virtual IRepository<Admin> Admins
        {
            get
            {
                if (_adminRepository == null)
                    _adminRepository = new GenericRepository<Admin>(_db);
                return _adminRepository;
            }
        }

        public virtual IRepository<Appointment> Appointments
        {
            get
            {
                if (_appointmentRepository == null)
                    _appointmentRepository = new GenericRepository<Appointment>(_db);
                return _appointmentRepository;
            }
        }

        public virtual IRepository<Diagnosis> Diagnoses
        {
            get
            {
                if (_diagnosisRepository == null)
                    _diagnosisRepository = new GenericRepository<Diagnosis>(_db);
                return _diagnosisRepository;
            }
        }

        public virtual IRepository<Doctor> Doctors
        {
            get
            {
                if (_doctorRepository == null)
                    _doctorRepository = new GenericRepository<Doctor>(_db);
                return _doctorRepository;
            }
        }

        public virtual IRepository<Grade> Grades
        {
            get
            {
                if (_gradeRepository == null)
                    _gradeRepository = new GenericRepository<Grade>(_db);
                return _gradeRepository;
            }
        }

        public virtual IRepository<Hospital> Hospitals
        {
            get
            {
                if (_hospitalRepository == null)
                    _hospitalRepository = new GenericRepository<Hospital>(_db);
                return _hospitalRepository;
            }
        }

        public virtual IRepository<Locality> Localities
        {
            get
            {
                if (_localityRepository == null)
                    _localityRepository = new GenericRepository<Locality>(_db);
                return _localityRepository;
            }
        }

        public virtual IRepository<Meeting> Meetings
        {
            get
            {
                if (_meetingRepository == null)
                    _meetingRepository = new GenericRepository<Meeting>(_db);
                return _meetingRepository;
            }
        }

        public virtual IRepository<Message> Messages
        {
            get
            {
                if (_messageRepository == null)
                    _messageRepository = new GenericRepository<Message>(_db);
                return _messageRepository;
            }
        }

        public virtual IRepository<Notification> Notifications
        {
            get
            {
                if (_notificationRepository == null)
                    _notificationRepository = new GenericRepository<Notification>(_db);
                return _notificationRepository;
            }
        }

        public virtual IRepository<Patient> Patients
        {
            get
            {
                if (_patientRepository == null)
                    _patientRepository = new GenericRepository<Patient>(_db);
                return _patientRepository;
            }
        }

        public virtual IRepository<Schedule> Schedules
        {
            get
            {
                if (_scheduleRepository == null)
                    _scheduleRepository = new GenericRepository<Schedule>(_db);
                return _scheduleRepository;
            }
        }

        public virtual IRepository<Specialty> Specialties
        {
            get
            {
                if (_specialtyRepository == null)
                    _specialtyRepository = new GenericRepository<Specialty>(_db);
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