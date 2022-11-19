using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly ILogger<SchedulesController> _logger;
        private readonly UnitOfWork _uow;

        public SchedulesController(
            ILogger<SchedulesController> logger,
            UnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        [HttpGet]
        public async Task<IEnumerable<Schedule>> Get()
        {
            return await _uow.Schedules.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> Get(int id)
        {
            var schedule = await _uow.Schedules.GetAsync(s => s.ScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return new ObjectResult(schedule);
        }

        [HttpGet("details")]
        public async Task<ActionResult<Schedule>> Get(string doctor, string day)
        {
            var schedule = await _uow.Schedules.GetAsync(s => s.Doctor.Id == doctor && s.DayOfWeek.ToString() == day);

            if (schedule == null)
            {
                return NotFound();
            }

            return new ObjectResult(schedule);
        }

        [HttpPost]
        public async Task<ActionResult<Schedule>> Post(Schedule schedule)
        {
            if (schedule == null)
            {
                return BadRequest();
            }

            await _uow.Schedules.CreateAsync(schedule);

            return Ok(schedule);
        }

        [HttpPut]
        public async Task<ActionResult<Schedule>> Put(Schedule schedule)
        {
            if (schedule == null)
            {
                return BadRequest();
            }

            await _uow.Schedules.UpdateAsync(schedule);

            return Ok(schedule);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Schedule>> Delete(int id)
        {
            var schedule = await _uow.Schedules.GetAsync(s => s.ScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            await _uow.Schedules.DeleteAsync(schedule);

            return Ok(schedule);
        }

        [HttpDelete("{Schedule}")]
        public async Task<ActionResult<Schedule>> Delete(Schedule schedule)
        {
            if (schedule == null)
            {
                return NotFound();
            }

            await _uow.Schedules.DeleteAsync(schedule);

            return Ok(schedule);
        }
    }
}
