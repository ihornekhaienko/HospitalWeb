using AutoMapper;
using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Services.Implementations;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Doctor Schedules
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("/")]
    public class SchedulesController : ControllerBase
    {
        private readonly ILogger<SchedulesController> _logger;
        private readonly IUnitOfWork _uow;

        public SchedulesController(
            ILogger<SchedulesController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Returns a list of Schedules
        /// </summary>
        /// <returns>List of Schedules</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> Get()
        {
            try
            {
                var schedules = await _uow.Schedules.GetAllAsync(include: s => s.Include(d => d.Doctor));

                return new ObjectResult(schedules);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SchedulesController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Schedule found by Id
        /// </summary>
        /// <param name="id">Schedule's id</param>
        /// <returns>The Schedule object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> Get(int id)
        {
            try
            {
                var schedule = await _uow.Schedules.GetAsync(s => s.ScheduleId == id, include: s => s.Include(d => d.Doctor));

                if (schedule == null)
                {
                    return NotFound("The schedule object wasn't found");
                }

                return new ObjectResult(schedule);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SchedulesController.Get(id): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
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
            try
            {
                DayOfWeek dayOfWeek;
                var result = Enum.TryParse(day, out dayOfWeek);

                if (!result)
                {
                    return BadRequest("Cannot convert day to the DayOfWeek");
                }

                var schedule = await _uow.Schedules.GetAsync(s => s.Doctor.Id == doctor && s.DayOfWeek == dayOfWeek,
                    include:
                    s => s.Include(d => d.Doctor));

                if (schedule == null)
                {
                    return NotFound();
                }

                return new ObjectResult(schedule);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SchedulesController.Get(doctor, day): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new Schedule
        /// </summary>
        /// <param name="schedule">Schedule to create</param>
        /// <returns>The Schedule object</returns>
        [HttpPost]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Schedule>> Post(ScheduleResourceModel schedule)
        {
            try
            {
                if (schedule == null)
                {
                    return BadRequest("Passing null object to the SchedulesController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<ScheduleResourceModel, Schedule>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<ScheduleResourceModel, Schedule>(schedule);

                await _uow.Schedules.CreateAsync(entity);

                _logger.LogDebug($"Created schedule with id {entity.ScheduleId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SchedulesController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Schedule object data
        /// </summary>
        /// <param name="schedule">The Schedule to update</param>
        /// <returns>The Schedule object</returns>
        [HttpPut]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Schedule>> Put(Schedule schedule)
        {
            try
            {
                if (schedule == null)
                {
                    return BadRequest("Passing null object to the SchedulesController.Put method");
                }

                await _uow.Schedules.UpdateAsync(schedule);

                _logger.LogDebug($"Updated schedule with id {schedule.ScheduleId}");

                return Ok(schedule);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SchedulesController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Schedule found by Id
        /// </summary>
        /// <param name="id">Schedule's id</param>
        /// <returns>The Schedule object</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Schedule>> Delete(int id)
        {
            try
            {
                var schedule = await _uow.Schedules.GetAsync(s => s.ScheduleId == id);

                if (schedule == null)
                {
                    return NotFound("The schedule object wasn't found");
                }

                await _uow.Schedules.DeleteAsync(schedule);

                _logger.LogDebug($"Deleted schedule with id {schedule.ScheduleId}");

                return Ok(schedule);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SchedulesController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
