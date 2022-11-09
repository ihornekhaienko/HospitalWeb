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

        public void Create(Patient item)
        {
            _db.Patients.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Patient item)
        {
            _db.Patients.Remove(item);
            _db.SaveChanges();
        }

        public Patient Get(int id)
        {
            return _db.Patients.Find(id);
        }

        public IEnumerable<Patient> GetAll()
        {
            return _db.Patients
                .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                .Include(p => p.Records)
                .ToList();
        }

        public void Update(Patient item)
        {
            _db.Patients.Update(item);
            _db.SaveChanges();
        }
    }
}
