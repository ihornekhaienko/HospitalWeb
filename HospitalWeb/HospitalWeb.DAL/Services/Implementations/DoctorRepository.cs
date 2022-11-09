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

        public void Create(Doctor item)
        {
            _db.Doctors.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Doctor item)
        {
            _db.Doctors.Remove(item);
            _db.SaveChanges();
        }

        public Doctor Get(int id)
        {
            return _db.Doctors.Find(id);
        }

        public IEnumerable<Doctor> GetAll()
        {
            return _db.Doctors
                .Include(d => d.Specialty)
                .Include(d => d.Records)
                .ToList();
        }

        public void Update(Doctor item)
        {
            _db.Doctors.Update(item);
            _db.SaveChanges();
        }
    }
}
