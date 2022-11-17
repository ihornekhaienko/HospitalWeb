using System.Linq.Expressions;

namespace HospitalWeb.DAL.Services.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Func<T, bool> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int first, int offset);
        Task<IEnumerable<T>> GetAllAsync(Func<T, bool> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int first, int offset);
        T Get(Expression<Func<T, bool>> filter);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        bool Contains(Expression<Func<T, bool>> query);
        Task<bool> ContainsAsync(Expression<Func<T, bool>> query);
        void Create(T item);
        Task CreateAsync(T item);
        void Update(T item);
        Task UpdateAsync(T item);
        void Delete(T item);
        Task DeleteAsync(T item);
    }
}
