using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class SpecialtyRepository : IRepository<Specialty>
    {
        private readonly AppDbContext _db;

        public SpecialtyRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Create(Specialty item)
        {
            _db.Specialties.Add(item);
            _db.SaveChanges();
        }

        public Specialty GetOrCreate(string? specialty)
        {
            if (string.IsNullOrWhiteSpace(specialty))
            {
                return null;
            }

            if (_db.Specialties.Any(s => s.SpecialtyName == specialty))
            {
                return _db.Specialties.FirstOrDefault(s => s.SpecialtyName == specialty);
            }
            else
            {
                var obj = new Specialty
                {
                    SpecialtyName = specialty
                };
                _db.Specialties.Add(obj);
                _db.SaveChanges();

                return obj;
            }
        }

        public void Delete(Specialty item)
        {
            _db.Specialties.Remove(item);
            _db.SaveChanges();
        }

        public Specialty Get(int id)
        {
            return _db.Specialties.Find(id);
        }

        public IEnumerable<Specialty> GetAll()
        {
            return _db.Specialties;
        }

        public void Update(Specialty item)
        {
            _db.Specialties.Update(item);
            _db.SaveChanges();
        }
    }
}
