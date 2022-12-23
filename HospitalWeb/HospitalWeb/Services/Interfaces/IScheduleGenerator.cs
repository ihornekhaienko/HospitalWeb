using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.ViewModels.Doctors;

namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface IScheduleGenerator
    {
        public IEnumerable<ScheduleViewModel> GenerateWeekSchedule(Doctor doctor, DateTime startDate);

        public ScheduleViewModel GenerateDaySchedule(Doctor doctor, DateTime date);
    }
}
