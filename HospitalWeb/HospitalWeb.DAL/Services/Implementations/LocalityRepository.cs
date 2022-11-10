using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class LocalityRepository : IRepository<Locality>
    {
        private readonly AppDbContext _db;

        public LocalityRepository(AppDbContext db)
        {
            _db = db;
        }

        public Locality Get(Func<Locality, bool> filter)
        {
            return _db.Localities
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients)
                .FirstOrDefault(filter);
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

        public bool Contains(Func<Locality, bool> query)
        {
            return _db.Localities
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients)
                .Any(query);
        }

        public void Create(Locality item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Locality item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Locality item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
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
    }
}
