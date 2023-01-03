using AutoMapper;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Patients
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;

        public PatientsController(
            ILogger<PatientsController> logger,
            IUnitOfWork uow,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _uow = uow;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns a filtered list of Patients
        /// </summary>
        /// <param name="searchString">Search string that identifies Admin</param>
        /// <param name="locality">Locality's Id</param>
        /// <param name="sortOrder">Sorting order of the filtered list</param>
        /// <param name="pageSize">Count of the result on one page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns>Fltered list of Patients</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> Get(
            string searchString,
            int? locality,
            PatientSortState sortOrder = PatientSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            try
            {
                int totalCount = 0;

                Func<Patient, bool> filter = (p) =>
                {
                    bool result = true;

                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        result = p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        p.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase);
                    }

                    if (locality != null && locality != 0)
                    {
                        result = result && p.Address.Locality.LocalityId == locality;
                    }

                    return result;
                };

                Func<IQueryable<Patient>, IOrderedQueryable<Patient>> orderBy = (patients) =>
                {
                    totalCount = patients.Count();

                    switch (sortOrder)
                    {
                        case PatientSortState.NameAsc:
                            patients = patients.OrderBy(p => p.Name);
                            break;
                        case PatientSortState.NameDesc:
                            patients = patients.OrderByDescending(p => p.Name);
                            break;
                        case PatientSortState.SurnameAsc:
                            patients = patients.OrderBy(p => p.Surname);
                            break;
                        case PatientSortState.SurnameDesc:
                            patients = patients.OrderByDescending(p => p.Surname);
                            break;
                        case PatientSortState.EmailAsc:
                            patients = patients.OrderBy(p => p.Email);
                            break;
                        case PatientSortState.EmailDesc:
                            patients = patients.OrderByDescending(p => p.Email);
                            break;
                        case PatientSortState.PhoneAsc:
                            patients = patients.OrderBy(p => p.PhoneNumber);
                            break;
                        case PatientSortState.PhoneDesc:
                            patients = patients.OrderByDescending(p => p.PhoneNumber);
                            break;
                        case PatientSortState.BirthDateAsc:
                            patients = patients.OrderBy(p => p.BirthDate);
                            break;
                        case PatientSortState.BirthDateDesc:
                            patients = patients.OrderByDescending(p => p.BirthDate);
                            break;
                        case PatientSortState.AddressAsc:
                            patients = patients
                                .OrderBy(p => p.Address.FullAddress)
                                .ThenBy(a => a.Address.Locality.LocalityName);
                            break;
                        case PatientSortState.AddressDesc:
                            patients = patients
                                .OrderByDescending(p => p.Address.FullAddress)
                                .ThenByDescending(a => a.Address.Locality.LocalityName);
                            break;
                        default:
                            patients = patients.OrderBy(d => d.Id);
                            break;
                    }

                    return (IOrderedQueryable<Patient>)patients;
                };

                var patients = await _uow.Patients
                    .GetAllAsync(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize,
                    include: p => p
                     .Include(p => p.Address)
                        .ThenInclude(a => a.Locality)
                    .Include(p => p.Appointments)
                        .ThenInclude(a => a.Diagnosis));

                Response.Headers.Add("TotalCount", totalCount.ToString());
                Response.Headers.Add("Count", patients.Count().ToString());
                Response.Headers.Add("PageSize", pageSize.ToString());
                Response.Headers.Add("PageNumber", pageNumber.ToString());
                Response.Headers.Add("TotalPages", ((int)Math.Ceiling(totalCount / (double)pageSize)).ToString());

                return new ObjectResult(patients);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in PatientController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Patient found by Id or Email
        /// </summary>
        /// <param name="searchString">Patient's id or Email</param>
        /// <returns>The Patient object</returns>
        [HttpGet("{searchString}")]
        public async Task<ActionResult<Patient>> Get(string searchString)
        {
            try
            {
                var patient = await _uow.Patients.GetAsync(p => p.Id == searchString || p.Email == searchString,
                 include: p => p
                 .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                 .Include(p => p.Appointments)
                    .ThenInclude(a => a.Diagnosis));

                if (patient == null)
                {
                    return NotFound("The patient object wasn't found");
                }

                return new ObjectResult(patient);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in PatientController.Get(searchString): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new Patient
        /// </summary>
        /// <param name="patient">Patient to create</param>
        /// <returns>The Patient object</returns>
        [HttpPost]
        public async Task<ActionResult<Patient>> Post(PatientResourceModel patient)
        {
            try
            {
                if (patient == null)
                {
                    return BadRequest("Passing null object to the PatientsController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<PatientResourceModel, Patient>()
                    .ForMember(d => d.Image, o => o.Ignore()));
                var mapper = new Mapper(config);

                var entity = mapper.Map<PatientResourceModel, Patient>(patient);

                IdentityResult result;

                if (string.IsNullOrWhiteSpace(patient.Password))
                {
                    result = await _userManager.CreateAsync(entity);
                }
                else
                {
                    result = await _userManager.CreateAsync(entity, patient.Password);
                }

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(entity, "Patient");

                    _logger.LogDebug($"Created patient with id {entity.Id}");

                    return Ok(entity);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in PatientController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Patient object data
        /// </summary>
        /// <param name="patient">The Patient to update</param>
        /// <returns>The Patient object</returns>
        [HttpPut]
        [Authorize(Policy = "PatientsOnly")]
        public async Task<ActionResult<Patient>> Put(Patient patient)
        {
            try
            {
                if (patient == null)
                {
                    return BadRequest("Passing null object to the PatientsController.Put method");
                }

                var result = await _userManager.UpdateAsync(patient);

                if (result.Succeeded)
                {
                    _logger.LogDebug($"Updated patient with id {patient.Id}");

                    return Ok(patient);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in PatientController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Patient found by Id or Email
        /// </summary>
        /// <param name="searchString">Patient's id or Email</param>
        /// <returns>The Patient object</returns>
        [HttpDelete("{searchString}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Patient>> Delete(string searchString)
        {
            try
            {
                var patient = await _uow.Patients.GetAsync(p => p.Id == searchString || p.Email == searchString);

                if (patient == null)
                {
                    return NotFound("The patient object wasn't found");
                }

                await _userManager.DeleteAsync(patient);

                _logger.LogDebug($"Deleted patient with id {patient.Id}");

                return Ok(patient);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in PatientController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
