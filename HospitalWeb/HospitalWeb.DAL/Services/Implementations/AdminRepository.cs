using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class AdminRepository : IRepository<Admin>
    {
        private readonly AppDbContext _db;

        public AdminRepository(AppDbContext db)
        {
            _db = db;
        }

        public Admin Get(Expression<Func<Admin, bool>> filter)
        {
            return _db.Admins.FirstOrDefault(filter);
        }


        public async Task<Admin> GetAsync(Expression<Func<Admin, bool>> filter)
        {
            return await _db.Admins.FirstOrDefaultAsync(filter);
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

        public async Task<IEnumerable<Admin>> GetAllAsync(
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

            return await admins.ToListAsync();
        }

        public bool Contains(Expression<Func<Admin, bool>> query)
        {
            return _db.Admins.Any(query);
        }

        public async Task<bool> ContainsAsync(Expression<Func<Admin, bool>> query)
        {
            return await _db.Admins.AnyAsync(query);
        }

        public void Create(Admin item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public async Task CreateAsync(Admin item)
        {
            _db.Add(item);
            await _db.SaveChangesAsync();
        }

        public void Delete(Admin item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }


        public async Task DeleteAsync(Admin item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();
        }

        public void Update(Admin item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public async Task UpdateAsync(Admin item)
        {
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
