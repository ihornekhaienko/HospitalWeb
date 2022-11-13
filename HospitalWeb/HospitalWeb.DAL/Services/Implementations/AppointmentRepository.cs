using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class AppointmentRepository : IRepository<Appointment>
    {
        private readonly AppDbContext _db;

        public AppointmentRepository(AppDbContext db)
        {
            _db = db;
        }

        public Appointment Get(Func<Appointment, bool> filter)
        {
            return _db.Appointments.FirstOrDefault(filter);
        }

        public IEnumerable<Appointment> GetAll(
            Func<Appointment, bool> filter = null,
            Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Appointment> appointments = _db.Appointments
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(r => r.Patient)
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

        public bool Contains(Func<Appointment, bool> query)
        {
            return _db.Appointments
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality)
                .Any(query);
        }

        public void Create(Appointment item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Appointment item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Appointment item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public bool IsDateFree(Doctor doctor, DateTime date)
        {
            return !_db.Appointments
                .Any(r => DateTime.Compare(r.AppointmentDate, date) == 0 &&
                     r.Doctor == doctor &&
                     (r.State == State.Active || r.State == State.Planned || r.State == State.Completed)
                );
        }
    }
}
