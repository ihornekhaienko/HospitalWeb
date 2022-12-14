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
        public async Task<ActionResult<IEnumerable<Meeting>>> Get()
        {
            try
            {
                var meetings = await _uow.Meetings.GetAllAsync(include: m => m.Include(m => m.Appointment));

                return new ObjectResult(meetings);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MeetingsController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Meeting found by Id
        /// </summary>
        /// <param name="id">Meeting's id</param>
        /// <returns>The Meeting object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Meeting>> Get(int id)
        {
            try
            {
                var meeting = await _uow.Meetings.GetAsync(m => m.MeetingId == id, include: m => m.Include(m => m.Appointment));

                if (meeting == null)
                {
                    return NotFound("The meeting object wasn't found");
                }

                return new ObjectResult(meeting);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MeetingsController.Get(id): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Meeting found by AppointmentId
        /// </summary>
        /// <param name="appointmentId">Appointment's id</param>
        /// <returns>The Meeting object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Meeting>> GetByAppointment(int appointmentId)
        {
            try
            {
                var meeting = await _uow.Meetings.GetAsync(m => m.AppointmentId == appointmentId, include: m => m.Include(m => m.Appointment));

                if (meeting == null)
                {
                    return NotFound("The meeting object wasn't found");
                }

                return new ObjectResult(meeting);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MeetingsController.Get(appointmentId): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
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
            try
            {
                if (Meeting == null)
                {
                    return BadRequest("Passing null object to the MeetingsController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<MeetingResourceModel, Meeting>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<MeetingResourceModel, Meeting>(Meeting);

                await _uow.Meetings.CreateAsync(entity);

                _logger.LogDebug($"Created meeting with id {entity.MeetingId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MeetingsController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
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
            try
            {
                if (meeting == null)
                {
                    return BadRequest("Passing null object to the MeetingsController.Put method");
                }

                await _uow.Meetings.UpdateAsync(meeting);

                _logger.LogDebug($"Updated meeting with id {meeting.MeetingId}");

                return Ok(meeting);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MeetingsController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
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
            try
            {
                var meeting = await _uow.Meetings.GetAsync(m => m.MeetingId == id);

                if (meeting == null)
                {
                    return NotFound("The meeting object wasn't found");
                }

                await _uow.Meetings.DeleteAsync(meeting);

                _logger.LogDebug($"Deleted meeting with id {meeting.MeetingId}");

                return Ok(meeting);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MeetingsController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
