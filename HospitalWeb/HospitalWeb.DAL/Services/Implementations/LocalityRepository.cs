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

        public void Create(Locality item)
        {
            _db.Localities.Add(item);
            _db.SaveChanges();
        }

        public Locality GetOrCreate(string? locality)
        {
            if (GetAll().Any(l => l.LocalityName == locality))
            {
                return GetAll()?.FirstOrDefault(l => l.LocalityName == locality);
            }
            else
            {
                var obj = new Locality { LocalityName = locality };
                _db.Localities.Add(obj);
                _db.SaveChanges();

                return obj;
            }
        }

        public void Delete(Locality item)
        {
            _db.Localities.Remove(item);
            _db.SaveChanges();
        }

        public Locality Get(int id)
        {
            return _db.Localities.Find(id);
        }

        public IEnumerable<Locality> GetAll()
        {
            return _db.Localities
                .Include(a => a.Addresses);
        }

        public void Update(Locality item)
        {
            _db.Localities.Update(item);
            _db.SaveChanges();
        }
    }
}
