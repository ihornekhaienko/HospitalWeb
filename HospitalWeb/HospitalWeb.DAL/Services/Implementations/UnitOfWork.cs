﻿using HospitalWeb.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext _db;
        private AddressRepository addressRepository;
        private LocalityRepository localityRepository;
        private RecordRepository recordRepository;
        private SpecialtyRepository specialtyRepository;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
        }

        public AddressRepository Addresses
        {
            get
            {
                if (addressRepository == null)
                    addressRepository = new AddressRepository(_db);
                return addressRepository;
            }
        }

        public LocalityRepository Localities
        {
            get
            {
                if (localityRepository == null)
                    localityRepository = new LocalityRepository(_db);
                return localityRepository;
            }
        }

        public RecordRepository Records
        {
            get
            {
                if (recordRepository == null)
                    recordRepository = new RecordRepository(_db);
                return recordRepository;
            }
        }

        public SpecialtyRepository Specialties
        {
            get
            {
                if (specialtyRepository == null)
                    specialtyRepository = new SpecialtyRepository(_db);
                return specialtyRepository;
            }
        }

        private bool _disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
