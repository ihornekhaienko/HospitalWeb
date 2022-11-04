using HospitalWeb.DAL.Data;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext _db;
        private AddressRepository _addressRepository;
        private LocalityRepository _localityRepository;
        private RecordRepository _recordRepository;
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

        public LocalityRepository Localities
        {
            get
            {
                if (_localityRepository == null)
                    _localityRepository = new LocalityRepository(_db);
                return _localityRepository;
            }
        }

        public RecordRepository Records
        {
            get
            {
                if (_recordRepository == null)
                    _recordRepository = new RecordRepository(_db);
                return _recordRepository;
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