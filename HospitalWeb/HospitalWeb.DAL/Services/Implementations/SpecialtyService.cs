using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly AppDbContext _db;

        public SpecialtyService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Specialty?> Create(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (_db.Specialties.Any(s => s.SpecialtyName == name))
            {
                return await _db.Specialties.FirstOrDefaultAsync(s => s.SpecialtyName == name);
            }
            else
            {
                var specialty = new Specialty
                {
                    SpecialtyName = name
                };
                _db.Specialties.Add(specialty);
                await _db.SaveChangesAsync();
                return specialty;
            }
        }

        public async Task Delete(Specialty specialty)
        {
            _db.Specialties?.Remove(specialty);
            await _db.SaveChangesAsync();
        }

        public Specialty? Get(int id)
        {
            return _db.Specialties?.FirstOrDefault(s => s.SpecialtyId == id);
        }

        public IEnumerable<Specialty>? GetAll()
        {
            return _db.Specialties?.Include(s => s.Doctors);
        }

        public async Task Update(Specialty specialty)
        {
            _db.Specialties?.Update(specialty);
            await _db.SaveChangesAsync();
        }
    }
}
