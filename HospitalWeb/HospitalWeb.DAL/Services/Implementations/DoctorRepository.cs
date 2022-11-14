using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class DoctorRepository : IRepository<Doctor>
    {
        private readonly AppDbContext _db;

        public DoctorRepository(AppDbContext db)
        {
            _db = db;
        }

        public Doctor Get(Func<Doctor, bool> filter)
        {
            return _db.Doctors
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .Include(d => d.Schedules)
                .Include(d => d.Specialty)
                .FirstOrDefault(filter);
        }

        public IEnumerable<Doctor> GetAll(
            Func<Doctor, bool> filter = null,
            Func<IQueryable<Doctor>, IOrderedQueryable<Doctor>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Doctor> doctors = _db.Doctors
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .Include(d => d.Schedules)
                .Include(d => d.Specialty);

            if (filter != null)
            {
                doctors = doctors.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                doctors = orderBy(doctors);
            }

            if (offset > 0)
            {
                doctors = doctors.Skip(offset);
            }

            if (first > 0)
            {
                doctors = doctors.Take(first);
            }

            return doctors
                .ToList();
        }

        public bool Contains(Func<Doctor, bool> query)
        {
            return _db.Doctors
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .Include(d => d.Schedules)
                .Include(d => d.Specialty)
                .Any(query);
        }

        public void Create(Doctor item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Doctor item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Doctor item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
