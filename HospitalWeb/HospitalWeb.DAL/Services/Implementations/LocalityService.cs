using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class LocalityService : ILocalityService
    {
        private readonly AppDbContext _db;

        public LocalityService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Locality?> Create(string? locality)
        {
            if (GetAll().Any(l => l.LocalityName == locality))
            {
                return GetAll()?.FirstOrDefault(l => l.LocalityName == locality);
            }
            else
            {
                var obj = new Locality { LocalityName = locality };
                _db.Localities?.Add(obj);
                await _db.SaveChangesAsync();
                return obj;
            }
        }

        public async Task Delete(Locality locality)
        {
            _db.Localities?.Remove(locality);
            await _db.SaveChangesAsync();
        }

        public Locality? Get(int id)
        {
            return GetAll()?.FirstOrDefault(l => l.LocalityId == id);
        }

        public IEnumerable<Locality>? GetAll()
        {
            return _db.Localities?.ToList();
        }

        public async Task Update(Locality locality)
        {
            _db.Localities?.Update(locality);
            await _db.SaveChangesAsync();
        }
    }
}
