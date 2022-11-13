using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Doctors;

namespace HospitalWeb.Services.Implementations
{
    public class ScheduleGenerator : IScheduleGenerator
    {
        private readonly UnitOfWork _uow;
        private DateTime _today;

        public ScheduleGenerator(UnitOfWork uow)
        {
            _uow = uow;
            _today = DateTime.Now;
        }

        public ScheduleViewModel GenerateDaySchedule(Doctor doctor, DateTime date)
        {
            var schedule = _uow.Schedules
                .Get(s => s.DayOfWeek == date.DayOfWeek && s.Doctor.Id == doctor.Id);

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
                    if (time.Hour - _today.Hour >= 1)
                    {
                        slots.Add(new SlotViewModel { Time = time, IsFree = _uow.Appointments.IsDateFree(doctor, time) });
                    }
                }
                else
                {
                    slots.Add(new SlotViewModel { Time = time, IsFree = _uow.Appointments.IsDateFree(doctor, time) });
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

        public IEnumerable<ScheduleViewModel> GenerateWeekSchedule(Doctor doctor, DateTime startDate)
        {
            _today = startDate;
            var date = startDate;

            var schedules = new List<ScheduleViewModel>();

            for (int i = 0; i < 7; i++)
            {
                var schedule = GenerateDaySchedule(doctor, date);
                schedules.Add(schedule);

                date = date.AddDays(1);
            }

            return schedules;
        }
    }
}
