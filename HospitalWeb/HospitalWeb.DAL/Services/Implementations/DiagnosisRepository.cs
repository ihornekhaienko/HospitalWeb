using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class DiagnosisRepository : IRepository<Diagnosis>
    {
        private readonly AppDbContext _db;

        public DiagnosisRepository(AppDbContext db)
        {
            _db = db;
        }

        public Diagnosis Get(Func<Diagnosis, bool> filter)
        {
            return _db.Diagnoses
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefault(filter);
        }

        public IEnumerable<Diagnosis> GetAll(
           Func<Diagnosis, bool> filter = null,
           Func<IQueryable<Diagnosis>, IOrderedQueryable<Diagnosis>> orderBy = null,
           int first = 0,
           int offset = 0)
        {
            IQueryable<Diagnosis> diagnoses = _db.Diagnoses
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient);

            if (filter != null)
            {
                diagnoses = diagnoses.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                diagnoses = orderBy(diagnoses);
            }

            if (offset > 0)
            {
                diagnoses = diagnoses.Skip(offset);
            }

            if (first > 0)
            {
                diagnoses = diagnoses.Take(first);
            }

            return diagnoses
                .ToList();
        }

        public bool Contains(Func<Diagnosis, bool> query)
        {
            return _db.Diagnoses
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient)
                .Any(query);
        }

        public void Create(Diagnosis item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Diagnosis item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Diagnosis item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public Diagnosis GetOrCreate(string diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
            {
                return null;
            }

            if (Contains(d => d.DiagnosisName == diagnosis))
            {
                return Get(d => d.DiagnosisName == diagnosis);
            }
            else
            {
                var obj = new Diagnosis
                {
                    DiagnosisName = diagnosis
                };
                Create(obj);

                return obj;
            }
        }
    }
}
