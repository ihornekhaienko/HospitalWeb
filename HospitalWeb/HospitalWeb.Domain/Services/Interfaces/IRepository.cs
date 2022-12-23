using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace HospitalWeb.Domain.Services.Interfaces
{
    public interface IRepository<T> where T : class
    {
        int Count { get; }

        IEnumerable<T> GetAll(Func<T, bool> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, int first = 0, int offset = 0);
        Task<IEnumerable<T>> GetAllAsync(Func<T, bool> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, int first = 0, int offset = 0);
        T Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        bool Contains(Expression<Func<T, bool>> query);
        Task<bool> ContainsAsync(Expression<Func<T, bool>> query);
        void Create(T item);
        Task CreateAsync(T item);
        void CreateMany(IEnumerable<T> items);
        Task CreateManyAsync(IEnumerable<T> items);
        void Update(T item);
        Task UpdateAsync(T item);
        void UpdateMany(IEnumerable<T> items);
        Task UpdateManyAsync(IEnumerable<T> items);
        void Delete(T item);
        Task DeleteAsync(T item);
        void Attach(T item);
        void Detach(T item);
    }
}
