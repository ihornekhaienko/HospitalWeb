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

        public IEnumerable<Schedule>? GetDoctorSchedules(Doctor? doctor)
        {
            if (doctor == null)
            {
                return null;
            }    

            return GetAll()
                .Where(d => d.Doctor == doctor)
                .ToList();
        }

        public IEnumerable<Schedule>? GetDoctorSchedules(string doctorId)
        {
            if (string.IsNullOrWhiteSpace(doctorId))
            {
                return null;
            }

            return GetAll()
                .Where(d => d.Doctor.Id == doctorId)
                .ToList();
        }

        public Schedule? GetDoctorScheduleByDay(string doctorId, string day)
        {
            DayOfWeek dayOfWeek;
            var result = Enum.TryParse(day, out dayOfWeek);

            if (result)
            {
                var doctorSchedules = GetDoctorSchedules(doctorId);

                return doctorSchedules
                    .Where(s => s.DayOfWeek == dayOfWeek)
                    .FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Schedule> GetAll()
        {
            return _db.Schedules
                .Include(s => s.Doctor)
                .ToList();
        }

        public void Update(Schedule item)
        {
            _db.Schedules.Update(item);
            _db.SaveChanges();
        }
    }
}
