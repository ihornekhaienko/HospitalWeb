using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public Patient Get(Expression<Func<Patient, bool>> filter)
        {
            return _db.Patients
                .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .FirstOrDefault(filter);
        }

        public async Task<Patient> GetAsync(Expression<Func<Patient, bool>> filter)
        {
            return await _db.Patients
                .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .FirstOrDefaultAsync(filter);
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
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Diagnosis);

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

        public async Task<IEnumerable<Patient>>  GetAllAsync(
            Func<Patient, bool> filter = null,
            Func<IQueryable<Patient>, IOrderedQueryable<Patient>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Patient> patients = _db.Patients
                .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Diagnosis);

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

            return await Task.FromResult(patients.ToList());
        }

        public bool Contains(Expression<Func<Patient, bool>> query)
        {
            return _db.Patients
                .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .Any(query);
        }

        public async Task<bool> ContainsAsync(Expression<Func<Patient, bool>> query)
        {
            return await _db.Patients
               .Include(p => p.Address)
                   .ThenInclude(a => a.Locality)
               .Include(p => p.Appointments)
                   .ThenInclude(a => a.Diagnosis)
               .AnyAsync(query);
        }

        public void Create(Patient item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public async Task CreateAsync(Patient item)
        {
            _db.Add(item);
            await _db.SaveChangesAsync();
        }

        public void Delete(Patient item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(Patient item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();
        }

        public void Update(Patient item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public async Task UpdateAsync(Patient item)
        {
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
