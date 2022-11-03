using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _db;

        public AddressService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Address?> Create(string? address)
        {
            if (GetAll().Any(a => a.FullAddress == address))
            {
                return GetAll()?.FirstOrDefault(a => a.FullAddress == address);
            }
            else
            {
                var obj = new Address { FullAddress = address };
                _db.Addresses?.Add(obj);
                await _db.SaveChangesAsync();
                return obj;
            }
        }

        public async Task Delete(Address address)
        {
            _db.Addresses?.Remove(address);
            await _db.SaveChangesAsync();
        }

        public Address? Get(int id)
        {
            return GetAll()?.FirstOrDefault(a => a.AddressId == id);
        }

        public IEnumerable<Address>? GetAll()
        {
            return _db.Addresses?.Include(a => a.Patients).ToList();
        }

        public async Task Update(Address address)
        {
            _db.Addresses?.Update(address);
            await _db.SaveChangesAsync();
        }
    }
}
