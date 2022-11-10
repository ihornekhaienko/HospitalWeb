namespace HospitalWeb.DAL.Services.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Func<T, bool> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int first, int offset);
        T Get(Func<T, bool> filter);
        bool Contains(Func<T, bool> query);
        void Create(T item);
        void Update(T item);
        void Delete(T item);
    }
}
