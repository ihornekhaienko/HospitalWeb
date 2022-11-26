using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public bool Contains(Expression<Func<T, bool>> query)
        {
            return _dbSet.Any(query);
        }

        public async Task<bool> ContainsAsync(Expression<Func<T, bool>> query)
        {
            return await _dbSet.AnyAsync(query);
        }

        public void Create(T item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public async Task CreateAsync(T item)
        {
            _db.Add(item);
            await _db.SaveChangesAsync();
        }

        public void Delete(T item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(T item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();
        }

        public T Get(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            return query.Where(filter)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public async Task<T> GetAsync(
            Expression<Func<T, bool>> filter, 
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            return await query.Where(filter)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public IEnumerable<T> GetAll(
            Func<T, bool> filter = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int first = 0, 
            int offset = 0)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (offset > 0)
            {
                query = query.Skip(offset);
            }

            if (first > 0)
            {
                query = query.Take(first);
            }

            return query.AsNoTracking().ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Func<T, bool> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (offset > 0)
            {
                query = query.Skip(offset);
            }

            if (first > 0)
            {
                query = query.Take(first);
            }

            return await Task.FromResult(query.AsNoTracking().ToList());
        }

        public void Update(T item)
        {
            Attach(item);
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public async Task UpdateAsync(T item)
        {
            Attach(item);
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public void Attach(T item)
        {
            _dbSet.Attach(item);
        }

        public void Detach(T item)
        {
            _db.Entry(item).State = EntityState.Detached;
        }
    }
}
