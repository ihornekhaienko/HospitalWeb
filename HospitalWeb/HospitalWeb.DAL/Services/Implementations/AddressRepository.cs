using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class AddressRepository : IRepository<Address>
    {
        private readonly AppDbContext _db;

        public AddressRepository(AppDbContext db)
        {
            _db = db;
        }

        public Address Get(Func<Address, bool> filter)
        {
            return _db.Addresses
                .Include(a => a.Locality)
                .Include(a => a.Patients)
                .FirstOrDefault(filter);
        }

        public IEnumerable<Address> GetAll(
            Func<Address, bool> filter = null,
            Func<IQueryable<Address>, IOrderedQueryable<Address>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Address> addresses = _db.Addresses
                .Include(a => a.Locality)
                .Include(a => a.Patients);

            if (filter != null)
            {
                addresses = addresses.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                addresses = orderBy(addresses);
            }

            if (offset > 0)
            {
                addresses = addresses.Skip(offset);
            }

            if (first > 0)
            {
                addresses = addresses.Take(first);
            }

            return addresses
                .ToList();
        }

        public bool Contains(Func<Address, bool> query)
        {
            return _db.Addresses
                .Include(a => a.Locality)
                .Include(a => a.Patients)
                .Any(query);
        }

        public void Create(Address item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Address item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Address item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public Address GetOrCreate(string address, Locality locality)
        {
            if (Contains(a => a.FullAddress == address && a.Locality == locality))
            {
                return Get(a => a.FullAddress == address && a.Locality == locality);
            }
            else
            {
                var obj = new Address { FullAddress = address, Locality = locality };
                Create(obj);

                return obj;
            }
        }
    }
}
