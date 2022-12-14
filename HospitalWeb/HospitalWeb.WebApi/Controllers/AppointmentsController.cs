using AutoMapper;
using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Hospital Appointments
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IUnitOfWork _uow;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Returns a filtered list of Appointments
        /// </summary>
        /// <param name="searchString">String for searching through Specialty or Diagnosis</param>
        /// <param name="userId">Appointment owner's id</param>
        /// <param name="state">Appointment state</param>
        /// <param name="locality">Appointment locality</param>
        /// <param name="fromDate">Starting date for searching</param>
        /// <param name="toDate">Ending date for searching</param>
        /// <param name="sortOrder">Sorting order of the filtered list</param>
        /// <param name="pageSize">Count of the result on one page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns>Filtered list of Appointments</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> Get(
            string searchString = null,
            string userId = null,
            int? state = null,
            int? locality = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            AppointmentSortState sortOrder = AppointmentSortState.DateAsc,
            int pageSize = 10,
            int pageNumber = 1)
        {
            try
            {
                int totalCount = 0;

                Func<Appointment, bool> filter = (a) =>
                {
                    bool result = true;

                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        result = a.Diagnosis.DiagnosisName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        a.Doctor.Specialty.SpecialtyName.Contains(searchString, StringComparison.OrdinalIgnoreCase);
                    }

                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        result = result && (a.Doctor.Id == userId || a.Patient.Id == userId);
                    }

                    if (state != null && state != 0)
                    {
                        result = result && (int)a.State == state;
                    }

                    if (locality != null && locality != 0)
                    {
                        result = result && (int)a.Patient.Address.LocalityId == locality;
                    }

                    if (fromDate != null)
                    {
                        result = result && DateTime.Compare((DateTime)fromDate, a.AppointmentDate) <= 0;
                    }

                    if (toDate != null)
                    {
                        result = result && DateTime.Compare((DateTime)toDate, a.AppointmentDate) >= 0;
                    }

                    return result;
                };

                Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>> orderBy = (appointments) =>
                {
                    totalCount = appointments.Count();

                    switch (sortOrder)
                    {
                        case AppointmentSortState.DateDesc:
                            appointments = appointments.OrderByDescending(a => a.AppointmentDate);
                            break;
                        case AppointmentSortState.DiagnosisAsc:
                            appointments = appointments.OrderBy(a => a.Diagnosis.DiagnosisName);
                            break;
                        case AppointmentSortState.DiagnosisDesc:
                            appointments = appointments.OrderByDescending(a => a.Diagnosis.DiagnosisName);
                            break;
                        case AppointmentSortState.StateAsc:
                            appointments = appointments.OrderBy(a => a.State);
                            break;
                        case AppointmentSortState.StateDesc:
                            appointments = appointments.OrderByDescending(a => a.State);
                            break;
                        case AppointmentSortState.DoctorAsc:
                            appointments = appointments.OrderBy(a => a.Doctor.ToString());
                            break;
                        case AppointmentSortState.DoctorDesc:
                            appointments = appointments.OrderByDescending(a => a.Doctor.ToString());
                            break;
                        case AppointmentSortState.PatientAsc:
                            appointments = appointments.OrderBy(a => a.Patient.ToString());
                            break;
                        case AppointmentSortState.PatientDesc:
                            appointments = appointments.OrderByDescending(a => a.Patient.ToString());
                            break;
                        default:
                            appointments = appointments.OrderBy(a => a.AppointmentDate);
                            break;
                    }

                    return (IOrderedQueryable<Appointment>)appointments;
                };

                var appointments = await _uow.Appointments
                    .GetAllAsync(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize,
                    include: a => a
                    .Include(a => a.Diagnosis)
                    .Include(a => a.Meetings)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.Specialty)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.Hospital)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.Address)
                            .ThenInclude(a => a.Locality));

                Response.Headers.Add("TotalCount", totalCount.ToString());
                Response.Headers.Add("Count", appointments.Count().ToString());
                Response.Headers.Add("PageSize", pageSize.ToString());
                Response.Headers.Add("PageNumber", pageNumber.ToString());
                Response.Headers.Add("TotalPages", ((int)Math.Ceiling(totalCount / (double)pageSize)).ToString());

                return new ObjectResult(appointments);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns Appointment found by Id
        /// </summary>
        /// <param name="id">Appointment Id</param>
        /// <returns>Appointment object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> Get(int id)
        {
            try
            {
                var appointment = await _uow.Appointments.GetAsync(a => a.AppointmentId == id,
                include: a => a
                .Include(a => a.Diagnosis)
                .Include(a => a.Meetings)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Hospital)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality));

                if (appointment == null)
                {
                    return NotFound("The appointment object wasn't found");
                }

                return new ObjectResult(appointment);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Get(id): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns Appointment found by DoctorId and its Date
        /// </summary>
        /// <param name="doctor">DoctorId</param>
        /// <param name="date">Date</param>
        /// <returns>Appointment object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Appointment>> Get(string doctor, DateTime date)
        {
            try
            {
                var appointment = await _uow.Appointments
                .GetAsync(a => a.Doctor.Id == doctor && DateTime.Compare(a.AppointmentDate, date) == 0,
                include: a => a
                .Include(a => a.Diagnosis)
                .Include(a => a.Meetings)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Hospital)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Address)
                        .ThenInclude(a => a.Locality));

                if (appointment == null)
                {
                    return NotFound();
                }

                return new ObjectResult(appointment);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Get(doctor, date): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates new Appointment
        /// </summary>
        /// <param name="appointment">Appointment to create</param>
        /// <returns>Appointment object</returns>
        [HttpPost]
        [Authorize(Policy = "DoctorsPatientsOnly")]
        public async Task<ActionResult<Appointment>> Post(AppointmentResourceModel appointment)
        {
            try
            {
                if (appointment == null)
                {
                    return BadRequest("Passing null object to the AppointmentsController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<AppointmentResourceModel, Appointment>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<AppointmentResourceModel, Appointment>(appointment);

                await _uow.Appointments.CreateAsync(entity);

                _logger.LogDebug($"Updated appointment with id {entity.AppointmentId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates Appointment's data
        /// </summary>
        /// <param name="appointment">Appointment to update</param>
        /// <returns>Appointment object</returns>
        [HttpPut]
        [Authorize(Policy = "DoctorsPatientsOnly")]
        public async Task<ActionResult<Appointment>> Put(Appointment appointment)
        {
            try
            {
                if (appointment == null)
                {
                    return BadRequest("Passing null object to the AppointmentsController.Put method");
                }

                await _uow.Appointments.UpdateAsync(appointment);

                _logger.LogDebug($"Updated appointment with id {appointment.AppointmentId}");

                return Ok(appointment);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes Appointment
        /// </summary>
        /// <param name="id">Appointment's id</param>
        /// <returns>Appointment object</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Appointment>> Delete(int id)
        {
            try
            {
                var appointment = await _uow.Appointments.GetAsync(a => a.AppointmentId == id);

                if (appointment == null)
                {
                    return NotFound("The appointment object wasn't found");
                }

                await _uow.Appointments.DeleteAsync(appointment);

                _logger.LogDebug($"Updated appointment with id {appointment.AppointmentId}");

                return Ok(appointment);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
