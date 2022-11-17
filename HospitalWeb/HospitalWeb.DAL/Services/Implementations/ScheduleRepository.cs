using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalWeb.DAL.Services.Implementations
{
    public class ScheduleRepository : IRepository<Schedule>
    {
        private readonly AppDbContext _db;

        public ScheduleRepository(AppDbContext db)
        {
            _db = db;
        }

        public Schedule Get(Expression<Func<Schedule, bool>> filter)
        {
            return _db.Schedules
                .Include(d => d.Doctor)
                .FirstOrDefault(filter);
        }

        public async Task<Schedule> GetAsync(Expression<Func<Schedule, bool>> filter)
        {
            return await _db.Schedules
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(filter);
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

        public async Task<IEnumerable<Schedule>> GetAllAsync(
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

            return await schedules.ToListAsync();
        }

        public bool Contains(Expression<Func<Schedule, bool>> query)
        {
            return _db.Schedules
                .Include(d => d.Doctor)
                .Any(query);
        }

        public async Task<bool> ContainsAsync(Expression<Func<Schedule, bool>> query)
        {
            return await _db.Schedules
                .Include(d => d.Doctor)
                .AnyAsync(query);
        }

        public void Create(Schedule item)
        {
            _db.Add(item);
            _db.SaveChanges();
        }

        public async Task CreateAsync(Schedule item)
        {
            _db.Add(item);
            await _db.SaveChangesAsync();
        }

        public void Delete(Schedule item)
        {
            _db.Remove(item);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(Schedule item)
        {
            _db.Remove(item);
            await _db.SaveChangesAsync();
        }

        public void Update(Schedule item)
        {
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public async Task UpdateAsync(Schedule item)
        {
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
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
