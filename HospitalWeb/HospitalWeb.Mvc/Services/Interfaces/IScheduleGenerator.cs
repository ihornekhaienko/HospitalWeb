using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.ViewModels.Doctors;

namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface IScheduleGenerator
    {
        public Task<IEnumerable<ScheduleViewModel>> GenerateWeekSchedule(string doctorId, DateTime startDate);

        public Task<ScheduleViewModel> GenerateDaySchedule(Doctor doctor, DateTime date);
    }
}
