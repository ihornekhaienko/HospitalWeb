using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class RecordRepository : IRepository<Record>
    {
        private readonly AppDbContext _db;

        public RecordRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Create(Record item)
        {
            _db.Records.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Record item)
        {
            _db.Records.Remove(item);
            _db.SaveChanges();
        }

        public Record Get(int id)
        {
            return _db.Records.Find(id);
        }

        public IEnumerable<Record> GetAll()
        {
            return _db.Records;
        }

        public void Update(Record item)
        {
            _db.Records.Update(item);
            _db.SaveChanges();
        }
    }
}
