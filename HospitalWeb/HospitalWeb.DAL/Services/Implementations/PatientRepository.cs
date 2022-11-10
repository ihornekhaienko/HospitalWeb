using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class PatientRepository : IRepository<Patient>
    {
        private readonly AppDbContext _db;

        public PatientRepository(AppDbContext db)
        {
            _db = db;
        }

        public Patient Get(Func<Patient, bool> filter)
        {
            return _db.Patients
                .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                .Include(p => p.Records)
                .FirstOrDefault(filter);
        }

        public IEnumerable<Patient> GetAll(
            Func<Patient, bool> filter = null,
            Func<IQueryable<Patient>, IOrderedQueryable<Patient>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Patient> patients = _db.Patients
                .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                .Include(p => p.Records);

            if (filter != null)
            {
                patients = patients.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                patients = orderBy(patients);
            }

            if (offset > 0)
            {
                patients = patients.Skip(offset);
            }

            if (first > 0)
            {
                patients = patients.Take(first);
            }

            return patients
                .ToList();
        }

        public bool Contains(Func<Patient, bool> query)
        {
            return _db.Patients
                .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                .Include(p => p.Records)
                .Any(query);
        }

        public void Create(Patient item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Patient item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Patient item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
