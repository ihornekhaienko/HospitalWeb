using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class AddressRepository : IRepository<Address>
    {
        private readonly AppDbContext _db;

        public AddressRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Create(Address item)
        {
            _db.Addresses.Add(item);
            _db.SaveChanges();
        }

        public Address GetOrCreate(string address, Locality locality)
        {
            if (GetAll().Any(a => a.FullAddress == address && a.Locality == locality))
            {
                return GetAll()?.FirstOrDefault(a => a.FullAddress == address && a.Locality == locality);
            }
            else
            {
                var obj = new Address { FullAddress = address, Locality = locality };
                _db.Addresses.Add(obj);
                _db.SaveChanges();

                return obj;
            }
        }

        public void Delete(Address item)
        {
            _db.Addresses.Remove(item);
            _db.SaveChanges();
        }

        public Address Get(int id)
        {
            return _db.Addresses.Find(id);
        }

        public IEnumerable<Address> GetAll()
        {
            return _db.Addresses;
        }

        public void Update(Address item)
        {
            _db.Addresses.Update(item);
            _db.SaveChanges();
        }
    }
}
