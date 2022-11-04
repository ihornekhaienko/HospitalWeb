using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class ScheduleRepository : IRepository<Schedule>
    {
        private readonly AppDbContext _db;

        public ScheduleRepository(AppDbContext db)
        {
            _db = db;
        }

        public void Create(Schedule item)
        {
            _db.Schedules.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Schedule item)
        {
            _db.Schedules.Remove(item);
            _db.SaveChanges();
        }

        public Schedule Get(int id)
        {
            return _db.Schedules.Find(id);
        }

        public IEnumerable<Schedule> GetAll()
        {
            return _db.Schedules;
        }

        public void Update(Schedule item)
        {
            _db.Schedules.Update(item);
            _db.SaveChanges();
        }
    }
}
