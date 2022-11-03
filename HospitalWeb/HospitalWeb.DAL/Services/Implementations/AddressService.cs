﻿using HospitalWeb.DAL.Data;
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

        public async Task<Address?> Create(string? address, Locality? locality)
        {
            if (GetAll().Any(a => a.FullAddress == address && a.Locality == locality))
            {
                return GetAll()?.FirstOrDefault(a => a.FullAddress == address && a.Locality == locality);
            }
            else
            {
                var obj = new Address { FullAddress = address, Locality = locality };
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
            return _db.Addresses?.Include(a => a.Locality).ToList();
        }

        public async Task Update(Address address)
        {
            _db.Addresses?.Update(address);
            await _db.SaveChangesAsync();
        }
    }
}
