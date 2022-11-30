using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.DAL.Services.Interfaces;
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
    [Route("api/[controller]")]
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
        public async Task<IEnumerable<Schedule>> Get()
        {
            return await _uow.Schedules.GetAllAsync(include: s => s.Include(d => d.Doctor));
        }

        /// <summary>
        /// Returns the Schedule found by Id
        /// </summary>
        /// <param name="id">Schedule's id</param>
        /// <returns>The Schedule object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> Get(int id)
        {
            var schedule = await _uow.Schedules.GetAsync(s => s.ScheduleId == id, include: s => s.Include(d => d.Doctor));

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
            DayOfWeek dayOfWeek;
            var result = Enum.TryParse(day, out dayOfWeek);

            if (!result)
            {
                return BadRequest();
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

        /// <summary>
        /// Creates the new Schedule
        /// </summary>
        /// <param name="schedule">Schedule to create</param>
        /// <returns>The Schedule object</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Schedule>> Post(ScheduleResourceModel schedule)
        {
            if (schedule == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<ScheduleResourceModel, Schedule>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<ScheduleResourceModel, Schedule>(schedule);

            await _uow.Schedules.CreateAsync(entity);

            return Ok(entity);
        }

        /// <summary>
        /// Updates the Schedule object data
        /// </summary>
        /// <param name="schedule">The Schedule to update</param>
        /// <returns>The Schedule object</returns>
        [HttpPut]
        [Authorize]
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
        [Authorize]
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
    }
}
