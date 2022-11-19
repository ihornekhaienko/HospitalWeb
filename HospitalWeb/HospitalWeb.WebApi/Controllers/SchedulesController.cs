using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Doctor Schedules
    /// </summary>
    [Produces("application/json")]
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

        /// <summary>
        /// Returns a list of Schedules
        /// </summary>
        /// <returns>List of Schedules</returns>
        [HttpGet]
        public async Task<IEnumerable<Schedule>> Get()
        {
            return await _uow.Schedules.GetAllAsync();
        }

        /// <summary>
        /// Returns the Schedule found by Id
        /// </summary>
        /// <param name="id">Schedule's id</param>
        /// <returns>The Schedule object</returns>
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

        /// <summary>
        /// Returns the Schedule found by Doctor and Day
        /// </summary>
        /// <param name="doctor">Doctor</param>
        /// <param name="day">Day of week</param>
        /// <returns>The Schedule object</returns>
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

        /// <summary>
        /// Creates the new Schedule
        /// </summary>
        /// <param name="schedule">Schedule to create</param>
        /// <returns>The Schedule object</returns>
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

        /// <summary>
        /// Updates the Schedule object data
        /// </summary>
        /// <param name="schedule">The Schedule to update</param>
        /// <returns>The Schedule object</returns>
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

        /// <summary>
        /// Deletes the Schedule found by Id
        /// </summary>
        /// <param name="id">Schedule's id</param>
        /// <returns>The Schedule object</returns>
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

        /// <summary>
        /// Deletes the Schedule 
        /// </summary>
        /// <param name="schedule">The Schedule object</param>
        /// <returns>The Schedule object</returns>
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
