using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class AppUserRepository : IRepository<AppUser>
    {
        private readonly AppDbContext _db;

        public AppUserRepository(AppDbContext db)
        {
            _db = db;
        }

        public AppUser Get(Expression<Func<AppUser, bool>> filter)
        {
            return _db.AppUsers
                .FirstOrDefault(filter);
        }

        public async Task<AppUser> GetAsync(Expression<Func<AppUser, bool>> filter)
        {
            return await _db.AppUsers
                .FirstOrDefaultAsync(filter);
        }

        public IEnumerable<AppUser> GetAll(
            Func<AppUser, bool> filter = null,
            Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<AppUser> localities = _db.AppUsers;

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

        public async Task<IEnumerable<AppUser>> GetAllAsync(
            Func<AppUser, bool> filter = null,
            Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<AppUser> localities = _db.AppUsers;

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

        public bool Contains(Expression<Func<AppUser, bool>> query)
        {
            return _db.AppUsers
                .Any(query);
        }

        public async Task<bool> ContainsAsync(Expression<Func<AppUser, bool>> query)
        {
            return await _db.AppUsers
                .AnyAsync(query);
        }

        public void Create(AppUser item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public async Task CreateAsync(AppUser item)
        {
            _db.Add(item);
            await _db.SaveChangesAsync();
        }

        public void Delete(AppUser item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(AppUser item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();
        }

        public void Update(AppUser item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public async Task UpdateAsync(AppUser item)
        {
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
