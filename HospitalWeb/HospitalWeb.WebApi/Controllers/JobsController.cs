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
        [Route("jobs/removeUnpaid")]
        public IActionResult RemoveUnpaid(int id)
        {
            BackgroundJob.Schedule(() => RemoveUnpaidAppointment(id), TimeSpan.FromHours(1));

            return Ok();
        }

        [HttpPost]
        [Route("jobs/updateStates")]
        public IActionResult UpdateStates()
        {
            RecurringJob.AddOrUpdate(() => SetMissed(), Cron.Daily);

            return Ok();
        }

        public async Task RemoveUnpaidAppointment(int id)
        {
            var appointment = await _uow.Appointments.GetAsync(a => a.AppointmentId == id);

            if (appointment.IsPaid)
            {
                return;
            }

            await _uow.Appointments.DeleteAsync(appointment);
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
