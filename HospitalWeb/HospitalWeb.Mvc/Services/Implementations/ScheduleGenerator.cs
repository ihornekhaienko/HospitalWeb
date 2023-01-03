using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Services.Interfaces;
using HospitalWeb.Mvc.ViewModels.Doctors;
using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using HospitalWeb.Domain.Entities;

namespace HospitalWeb.Mvc.Services.Implementations
{
    public class ScheduleGenerator : IScheduleGenerator
    {
        private readonly ApiUnitOfWork _api;
        private readonly IUnitOfWork _uow;
        private DateTime _today;

        public ScheduleGenerator(ApiUnitOfWork api, IUnitOfWork uow)
        {
            _api = api;
            _uow = uow;
            _today = DateTime.Now;
        }

        public async Task<ScheduleViewModel> GenerateDaySchedule(Doctor doctor, DateTime date)
        {
            var schedule = await _uow.Schedules.GetAsync(s => s.Doctor.Id == doctor.Id && s.DayOfWeek == date.DayOfWeek,
                    include:
                    s => s.Include(d => d.Doctor));
            //var response = _api.Schedules.Get(doctor.Id, date.DayOfWeek.ToString());

            if (schedule == null || date < _today)
            {
                return new ScheduleViewModel
                {
                    Date = date,
                    Slots = new List<SlotViewModel>()
                };
            }

            List<SlotViewModel> slots = new List<SlotViewModel>();
            var startTime = new DateTime(date.Year, date.Month, date.Day, 
                schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
            var endTime = new DateTime(date.Year, date.Month, date.Day,
               schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second);

            for (var time = startTime; time < endTime; time = time.AddHours(1))
            {
                if (date.Date == _today.Date)
                {
                    if (time.Hour - _today.Hour > 2)
                    {
                        slots.Add(new SlotViewModel { Time = time, IsFree = await IsDateFree(doctor.Id, time) });
                    }
                }
                else
                {
                    slots.Add(new SlotViewModel { Time = time, IsFree = await IsDateFree(doctor.Id, time) });
                }    
            }

            var model = new ScheduleViewModel
            {
                DoctorId = doctor.Id,
                Date = date,
                Slots = slots
            };

            return model;
        }

        private async Task<bool> IsDateFree(string doctorId, DateTime time)
        {
            var appointment = await _uow.Appointments
                .GetAsync(a => a.Doctor.Id == doctorId && DateTime.Compare(a.AppointmentDate, time) == 0,
                include: a => a
                .Include(a => a.Diagnosis)
                .Include(a => a.Meetings)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Hospital)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality));

            if (appointment != null)
            {
                if (appointment.State == State.Planned || appointment.State == State.Active || appointment.State == State.Completed)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<IEnumerable<ScheduleViewModel>> GenerateWeekSchedule(string doctorId, DateTime startDate)
        {
            var doctor = await _uow.Doctors.GetAsync(d => d.Id == doctorId || d.Email == doctorId,
                include: d => d
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .Include(d => d.Schedules)
                .Include(d => d.Hospital)
                    .ThenInclude(h => h.Address)
                            .ThenInclude(a => a.Locality)
                .Include(d => d.Specialty)
                .Include(d => d.Grades));

            _today = startDate;
            var date = startDate;

            var schedules = new List<ScheduleViewModel>();

            for (int i = 0; i < 7; i++)
            {
                var schedule = await GenerateDaySchedule(doctor, date);
                schedules.Add(schedule);

                date = date.AddDays(1);
            }

            return schedules;
        }
    }
}
