using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Meetings
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class MeetingsController : ControllerBase
    {
        private readonly ILogger<MeetingsController> _logger;
        private readonly IUnitOfWork _uow;

        public MeetingsController(
            ILogger<MeetingsController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Return a list of Meetings
        /// </summary>
        /// <returns>List of Meetings</returns>
        [HttpGet]
        public async Task<IEnumerable<Meeting>> Get()
        {
            return await _uow.Meetings.GetAllAsync(include: m => m.Include(m => m.Appointment));
        }

        /// <summary>
        /// Returns the Meeting found by Id
        /// </summary>
        /// <param name="id">Meeting's id</param>
        /// <returns>The Meeting object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Meeting>> Get(int id)
        {
            var meeting = await _uow.Meetings.GetAsync(m => m.MeetingId == id, include: m => m.Include(m => m.Appointment));

            if (meeting == null)
            {
                return NotFound();
            }

            return new ObjectResult(meeting);
        }

        /// <summary>
        /// Returns the Meeting found by AppointmentId
        /// </summary>
        /// <param name="appointmentId">Appointment's id</param>
        /// <returns>The Meeting object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Meeting>> GetByAppointment(int appointmentId)
        {
            var meeting = await _uow.Meetings.GetAsync(m => m.AppointmentId == appointmentId, include: m => m.Include(m => m.Appointment));

            if (meeting == null)
            {
                return NotFound();
            }

            return new ObjectResult(meeting);
        }

        /// <summary>
        /// Creates the new Meeting
        /// </summary>
        /// <param name="Meeting">Meeting to create</param>
        /// <returns>The Meeting object</returns>
        [HttpPost]
        [Authorize(Policy = "DoctorsPatientsOnly")]
        public async Task<ActionResult<Meeting>> Post(MeetingResourceModel Meeting)
        {
            if (Meeting == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<MeetingResourceModel, Meeting>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<MeetingResourceModel, Meeting>(Meeting);

            await _uow.Meetings.CreateAsync(entity);

            return Ok(entity);
        }

        /// <summary>
        /// Updates the Meeting object data
        /// </summary>
        /// <param name="meeting">The Meeting to update</param>
        /// <returns>The Meeting object</returns>
        [HttpPut]
        [Authorize(Policy = "DoctorsPatientsOnly")]
        public async Task<ActionResult<Meeting>> Put(Meeting meeting)
        {
            if (meeting == null)
            {
                return BadRequest();
            }

            await _uow.Meetings.UpdateAsync(meeting);

            return Ok(meeting);
        }

        /// <summary>
        /// Deletes the Meeting found by Id
        /// </summary>
        /// <param name="id">Meeting's id</param>
        /// <returns>The Meeting object</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "DoctorsPatientsOnly")]
        public async Task<ActionResult<Meeting>> Delete(int id)
        {
            var meeting = await _uow.Meetings.GetAsync(m => m.MeetingId == id);

            if (meeting == null)
            {
                return NotFound();
            }

            await _uow.Meetings.DeleteAsync(meeting);

            return Ok(meeting);
        }
    }
}
