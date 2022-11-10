using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class AdminRepository : IRepository<Admin>
    {
        private readonly AppDbContext _db;

        public AdminRepository(AppDbContext db)
        {
            _db = db;
        }

        public Admin Get(Func<Admin, bool> filter)
        {
            return _db.Admins.FirstOrDefault(filter);
        }

        public IEnumerable<Admin> GetAll(
            Func<Admin, bool> filter = null,
            Func<IQueryable<Admin>, IOrderedQueryable<Admin>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Admin> admins = _db.Admins;

            if (filter != null)
            {
                admins = admins.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                admins = orderBy(admins);
            }

            if (offset > 0)
            {
                admins = admins.Skip(offset);
            }

            if (first > 0)
            {
                admins = admins.Take(first);
            }

            return admins
                .ToList();
        }

        public bool Contains(Func<Admin, bool> query)
        {
            return _db.Admins.Any(query);
        }

        public void Create(Admin item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Admin item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Admin item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
