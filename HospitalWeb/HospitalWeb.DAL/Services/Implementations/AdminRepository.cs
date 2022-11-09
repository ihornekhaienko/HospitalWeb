using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class AdminRepository : IRepository<Admin>
    {
        private readonly AppDbContext _db;

        public AdminRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Create(Admin item)
        {
            _db.Admins.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Admin item)
        {
            _db.Admins.Remove(item);
            _db.SaveChanges();
        }

        public Admin Get(int id)
        {
            return _db.Admins.Find(id);
        }

        public IEnumerable<Admin> GetAll()
        {
            return _db.Admins.ToList();
        }

        public void Update(Admin item)
        {
            _db.Admins.Update(item);
            _db.SaveChanges();
        }
    }
}
