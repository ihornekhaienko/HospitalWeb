using Hangfire;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    public class JobsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public JobsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost]
        [Route("jobs/updateStates")]
        public IActionResult UpdateStates()
        {
            RecurringJob.AddOrUpdate(() => SetMissed(), Cron.Daily);

            return Ok();
        }

        public async Task SetMissed()
        {
            var today = DateTime.Today;
            var appointments = await _uow.Appointments.GetAllAsync(a => DateTime.Compare(a.AppointmentDate, today) < 0);

            foreach (var appointment in appointments)
            {
                if (appointment.State == State.Planned)
                {
                    appointment.State = State.Missed;
                }
            }

            await _uow.Appointments.UpdateManyAsync(appointments);
        }
    }
}
