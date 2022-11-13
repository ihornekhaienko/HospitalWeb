using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.ViewModels.Doctors;

namespace HospitalWeb.Services.Interfaces
{
    public interface IScheduleGenerator
    {
        public IEnumerable<ScheduleViewModel> GenerateWeekSchedule(Doctor doctor, DateTime startDate);

        public ScheduleViewModel GenerateDaySchedule(Doctor doctor, DateTime date);
    }
}
