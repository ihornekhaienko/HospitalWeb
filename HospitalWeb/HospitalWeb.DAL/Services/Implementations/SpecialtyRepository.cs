using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class SpecialtyRepository : IRepository<Specialty>
    {
        private readonly AppDbContext _db;

        public SpecialtyRepository(AppDbContext db)
        {
            _db = db;
        }

        public Specialty Get(Expression<Func<Specialty, bool>> filter)
        {
            return _db.Specialties
                .Include(s => s.Doctors)
                .FirstOrDefault(filter);
        }

        public async Task<Specialty> GetAsync(Expression<Func<Specialty, bool>> filter)
        {
            return await _db.Specialties
               .Include(s => s.Doctors)
               .FirstOrDefaultAsync(filter);
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

        public async Task<IEnumerable<Specialty>> GetAllAsync(
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

            return await specialties.ToListAsync();
        }

        public bool Contains(Expression<Func<Specialty, bool>> query)
        {
            return _db.Specialties
                .Include(s => s.Doctors)
                .Any(query);
        }

        public async Task<bool> ContainsAsync(Expression<Func<Specialty, bool>> query)
        {
            return await _db.Specialties
                .Include(s => s.Doctors)
                .AnyAsync(query);
        }

        public void Create(Specialty item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public async Task CreateAsync(Specialty item)
        {
            _db.Add(item);
            await _db.SaveChangesAsync();
        }

        public void Delete(Specialty item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(Specialty item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();
        }

        public void Update(Specialty item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public async Task UpdateAsync(Specialty item)
        {
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
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

        public async Task<Specialty> GetOrCreateAsync(string specialty)
        {
            if (string.IsNullOrWhiteSpace(specialty))
            {
                return null;
            }

            if (await ContainsAsync(s => s.SpecialtyName == specialty))
            {
                return await GetAsync(s => s.SpecialtyName == specialty);
            }
            else
            {
                var obj = new Specialty
                {
                    SpecialtyName = specialty
                };
                await CreateAsync(obj);

                return obj;
            }
        }
    }
}
