using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class AppointmentRepository : IRepository<Appointment>
    {
        private readonly AppDbContext _db;

        public AppointmentRepository(AppDbContext db)
        {
            _db = db;
        }

        public Appointment Get(Expression<Func<Appointment, bool>> filter)
        {
            return _db.Appointments
                .Include(a => a.Diagnosis)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality)
                .FirstOrDefault(filter);
        }

        public async Task<Appointment> GetAsync(Expression<Func<Appointment, bool>> filter)
        {
            return await _db.Appointments
                .Include(a => a.Diagnosis)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality)
                .FirstOrDefaultAsync(filter);
        }

        public IEnumerable<Appointment> GetAll(
            Func<Appointment, bool> filter = null,
            Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Appointment> appointments = _db.Appointments
                .Include(a => a.Diagnosis)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality);

            if (filter != null)
            {
                appointments = appointments.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                appointments = orderBy(appointments);
            }

            if (offset > 0)
            {
                appointments = appointments.Skip(offset);
            }

            if (first > 0)
            {
                appointments = appointments.Take(first);
            }

            return appointments
                .ToList();
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync(
            Func<Appointment, bool> filter = null,
            Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Appointment> appointments = _db.Appointments
                .Include(a => a.Diagnosis)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality);

            if (filter != null)
            {
                appointments = appointments.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                appointments = orderBy(appointments);
            }

            if (offset > 0)
            {
                appointments = appointments.Skip(offset);
            }

            if (first > 0)
            {
                appointments = appointments.Take(first);
            }

            return await appointments.ToListAsync();
        }

        public bool Contains(Expression<Func<Appointment, bool>> query)
        {
            return _db.Appointments
                .Include(a => a.Diagnosis)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality)
                .Any(query);
        }


        public async Task<bool> ContainsAsync(Expression<Func<Appointment, bool>> query)
        {
            return await _db.Appointments
               .Include(a => a.Diagnosis)
               .Include(a => a.Doctor)
                   .ThenInclude(d => d.Specialty)
               .Include(a => a.Patient)
                   .ThenInclude(p => p.Address)
                       .ThenInclude(a => a.Locality)
               .AnyAsync(query);
        }

        public void Create(Appointment item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public async Task CreateAsync(Appointment item)
        {
            _db.Add(item);
            await _db.SaveChangesAsync();
        }

        public void Delete(Appointment item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(Appointment item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();
        }

        public void Update(Appointment item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public async Task UpdateAsync(Appointment item)
        {
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public bool IsDateFree(Doctor doctor, DateTime date)
        {
            return !_db.Appointments
                .Any(a => DateTime.Compare(a.AppointmentDate, date) == 0 &&
                     a.Doctor == doctor &&
                     (a.State == State.Active || a.State == State.Planned || a.State == State.Completed)
                );
        }

        public void UpdateStates()
        {
            var date = DateTime.Today;
            var missed = _db.Appointments.Where(a => a.AppointmentDate < date && a.State == State.Planned);

            foreach (var a in missed)
            {
                a.State = State.Missed;
            }

            _db.Appointments.UpdateRange(missed);
            _db.SaveChanges();
        }
    }
}
