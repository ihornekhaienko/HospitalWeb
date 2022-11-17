using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class DiagnosisRepository : IRepository<Diagnosis>
    {
        private readonly AppDbContext _db;

        public DiagnosisRepository(AppDbContext db)
        {
            _db = db;
        }

        public Diagnosis Get(Expression<Func<Diagnosis, bool>> filter)
        {
            return _db.Diagnoses
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefault(filter);
        }

        public async Task<Diagnosis> GetAsync(Expression<Func<Diagnosis, bool>> filter)
        {
            return await _db.Diagnoses
               .Include(d => d.Appointments)
                   .ThenInclude(a => a.Doctor)
               .Include(d => d.Appointments)
                   .ThenInclude(a => a.Patient)
               .FirstOrDefaultAsync(filter);
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

        public async Task<IEnumerable<Diagnosis>> GetAllAsync(
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

            return await diagnoses.ToListAsync();
        }

        public bool Contains(Expression<Func<Diagnosis, bool>> query)
        {
            return _db.Diagnoses
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient)
                .Any(query);
        }

        public async Task<bool> ContainsAsync(Expression<Func<Diagnosis, bool>> query)
        {
            return await _db.Diagnoses
                 .Include(d => d.Appointments)
                     .ThenInclude(a => a.Doctor)
                 .Include(d => d.Appointments)
                     .ThenInclude(a => a.Patient)
                 .AnyAsync(query);
        }

        public void Create(Diagnosis item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public async Task CreateAsync(Diagnosis item)
        {
            _db.Add(item);
            await _db.SaveChangesAsync();
        }

        public void Delete(Diagnosis item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(Diagnosis item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();
        }

        public void Update(Diagnosis item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public async Task UpdateAsync(Diagnosis item)
        {
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
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

        public async Task<Diagnosis> GetOrCreateAsync(string diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
            {
                return null;
            }

            if (await ContainsAsync(d => d.DiagnosisName == diagnosis))
            {
                return await GetAsync(d => d.DiagnosisName == diagnosis);
            }
            else
            {
                var obj = new Diagnosis
                {
                    DiagnosisName = diagnosis
                };
                await CreateAsync(obj);

                return obj;
            }
        }
    }
}
