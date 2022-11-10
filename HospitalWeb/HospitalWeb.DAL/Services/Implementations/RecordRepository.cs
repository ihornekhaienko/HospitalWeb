using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class RecordRepository : IRepository<Record>
    {
        private readonly AppDbContext _db;

        public RecordRepository(AppDbContext db)
        {
            _db = db;
        }

        public Record Get(Func<Record, bool> filter)
        {
            return _db.Records.FirstOrDefault(filter);
        }

        public IEnumerable<Record> GetAll(
            Func<Record, bool> filter = null,
            Func<IQueryable<Record>, IOrderedQueryable<Record>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Record> records = _db.Records
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality);

            if (filter != null)
            {
                records = records.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                records = orderBy(records);
            }

            if (offset > 0)
            {
                records = records.Skip(offset);
            }

            if (first > 0)
            {
                records = records.Take(first);
            }

            return records
                .ToList();
        }

        public bool Contains(Func<Record, bool> query)
        {
            return _db.Records
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality)
                .Any(query);
        }

        public void Create(Record item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Record item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Record item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
