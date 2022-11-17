using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class LocalityRepository : IRepository<Locality>
    {
        private readonly AppDbContext _db;

        public LocalityRepository(AppDbContext db)
        {
            _db = db;
        }

        public Locality Get(Expression<Func<Locality, bool>> filter)
        {
            return _db.Localities
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients)
                .FirstOrDefault(filter);
        }

        public async Task<Locality> GetAsync(Expression<Func<Locality, bool>> filter)
        {
            return await _db.Localities
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients)
                .FirstOrDefaultAsync(filter);
        }

        public IEnumerable<Locality> GetAll(
            Func<Locality, bool> filter = null,
            Func<IQueryable<Locality>, IOrderedQueryable<Locality>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Locality> localities = _db.Localities
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients);

            if (filter != null)
            {
                localities = localities.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                localities = orderBy(localities);
            }

            if (offset > 0)
            {
                localities = localities.Skip(offset);
            }

            if (first > 0)
            {
                localities = localities.Take(first);
            }

            return localities
                .ToList();
        }

        public async Task<IEnumerable<Locality>> GetAllAsync(
            Func<Locality, bool> filter = null,
            Func<IQueryable<Locality>, IOrderedQueryable<Locality>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Locality> localities = _db.Localities
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients);

            if (filter != null)
            {
                localities = localities.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                localities = orderBy(localities);
            }

            if (offset > 0)
            {
                localities = localities.Skip(offset);
            }

            if (first > 0)
            {
                localities = localities.Take(first);
            }

            return await localities.ToListAsync();
        }

        public bool Contains(Expression<Func<Locality, bool>> query)
        {
            return _db.Localities
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients)
                .Any(query);
        }

        public async Task<bool> ContainsAsync(Expression<Func<Locality, bool>> query)
        {
            return await _db.Localities
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients)
                .AnyAsync(query);
        }

        public void Create(Locality item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public async Task CreateAsync(Locality item)
        {
            _db.Add(item);
            await _db.SaveChangesAsync();
        }

        public void Delete(Locality item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(Locality item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();
        }

        public void Update(Locality item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public async Task UpdateAsync(Locality item)
        {
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public Locality GetOrCreate(string locality)
        {
            if (Contains(l => l.LocalityName == locality))
            {
                return Get(l => l.LocalityName == locality);
            }
            else
            {
                var obj = new Locality { LocalityName = locality };
                Create(obj);

                return obj;
            }
        }

        public async Task<Locality> GetOrCreateAsync(string locality)
        {
            if (await ContainsAsync(l => l.LocalityName == locality))
            {
                return await GetAsync(l => l.LocalityName == locality);
            }
            else
            {
                var obj = new Locality { LocalityName = locality };
                await CreateAsync(obj);

                return obj;
            }
        }
    }
}
