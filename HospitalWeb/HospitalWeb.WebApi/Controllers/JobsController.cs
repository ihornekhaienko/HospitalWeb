using Hangfire;
using HospitalWeb.WebApi.Clients.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    public class JobsController : ControllerBase
    {
        private readonly ApiUnitOfWork _api;

        public JobsController(ApiUnitOfWork api)
        {
            _api = api;
        }

        [HttpPost]
        [Route("updateStates")]
        public IActionResult UpdateStates()
        {
            RecurringJob.AddOrUpdate(() => _api.Appointments.UpdateStates(null, null), Cron.Daily);

            return Ok();
        }
    }
}
