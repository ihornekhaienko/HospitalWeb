using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class ScheduleRepository : IRepository<Schedule>
    {
        private readonly AppDbContext _db;

        public ScheduleRepository(AppDbContext db)
        {
            _db = db;
        }

        public Schedule Get(Func<Schedule, bool> filter)
        {
            return _db.Schedules
                .Include(d => d.Doctor)
                .FirstOrDefault(filter);
        }

        public IEnumerable<Schedule> GetAll(
            Func<Schedule, bool> filter = null,
            Func<IQueryable<Schedule>, IOrderedQueryable<Schedule>> orderBy = null,
            int first = 0,
            int offset = 0)
        {
            IQueryable<Schedule> schedules = _db.Schedules
                .Include(d => d.Doctor);

            if (filter != null)
            {
                schedules = schedules.Where(filter).AsQueryable();
            }

            if (orderBy != null)
            {
                schedules = orderBy(schedules);
            }

            if (offset > 0)
            {
                schedules = schedules.Skip(offset);
            }

            if (first > 0)
            {
                schedules = schedules.Take(first);
            }

            return schedules
                .ToList();
        }

        public bool Contains(Func<Schedule, bool> query)
        {
            return _db.Schedules
                .Include(d => d.Doctor)
                .Any(query);
        }

        public void Create(Schedule item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public void Delete(Schedule item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public void Update(Schedule item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public IEnumerable<Schedule> GetDoctorSchedules(Doctor doctor)
        {
            if (doctor == null)
            {
                return null;
            }

            return GetAll(d => d.Doctor == doctor);
        }

        public IEnumerable<Schedule> GetDoctorSchedules(string doctorId)
        {
            if (string.IsNullOrWhiteSpace(doctorId))
            {
                return null;
            }

            return GetAll(d => d.Doctor.Id == doctorId);
        }

        public Schedule GetDoctorScheduleByDay(string doctorId, string day)
        {
            DayOfWeek dayOfWeek;
            var result = Enum.TryParse(day, out dayOfWeek);

            if (result)
            {
                var doctorSchedules = GetDoctorSchedules(doctorId);

                return Get(d => d.Doctor.Id == doctorId && d.DayOfWeek == dayOfWeek);
            }
            else
            {
                return null;
            }
        }
    }
}
