using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class SpecialtyRepository : IRepository<Specialty>
    {
        private readonly AppDbContext _db;

        public SpecialtyRepository(AppDbContext db)
        {
            _db = db;
        }

        public Specialty Get(Func<Specialty, bool> filter)
        {
            return _db.Specialties
                .Include(s => s.Doctors)
                .FirstOrDefault(filter);
        }

        public IEnumerable<Specialty> GetAll(
            Func<Specialty, bool> filter = null,
            Func<IQueryable<Specialty>, IOrderedQueryable<Specialty>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Specialty> specialties = _db.Specialties
                .Include(s => s.Doctors);

            if (filter != null)
            {
                specialties = specialties.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                specialties = orderBy(specialties);
            }

            if (offset > 0)
            {
                specialties = specialties.Skip(offset);
            }

            if (first > 0)
            {
                specialties = specialties.Take(first);
            }

            return specialties
                .ToList();
        }

        public bool Contains(Func<Specialty, bool> query)
        {
            return _db.Specialties
                .Include(s => s.Doctors)
                .Any(query);
        }

        public void Create(Specialty item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Specialty item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Specialty item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public Specialty GetOrCreate(string specialty)
        {
            if (string.IsNullOrWhiteSpace(specialty))
            {
                return null;
            }

            if (Contains(s => s.SpecialtyName == specialty))
            {
                return Get(s => s.SpecialtyName == specialty);
            }
            else
            {
                var obj = new Specialty
                {
                    SpecialtyName = specialty
                };
                Create(obj);

                return obj;
            }
        }
    }
}
