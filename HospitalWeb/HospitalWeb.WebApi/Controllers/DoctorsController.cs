using AutoMapper;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Doctors
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;

        public DoctorsController(
            ILogger<DoctorsController> logger,
            IUnitOfWork uow,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _uow = uow;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns a filtered list of Doctors
        /// </summary>
        /// <param name="searchString">Search string that identifies Admin</param>
        /// <param name="specialty">Doctor's specialty Id</param>
        /// <param name="hospital">Doctor's hospital Id</param>
        /// <param name="locality">Doctor's locality Id</param>
        /// <param name="sortOrder">Sorting order of the filtered list</param>
        /// <param name="pageSize">Count of the result on one page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns>Filtered list of Doctors</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> Get(
            string searchString = null,
            int? specialty = null,
            int? hospital = null,
            int? locality = null,
            DoctorSortState sortOrder = DoctorSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            try
            {
                int totalCount = 0;

                Func<Doctor, bool> filter = (d) =>
                {
                    bool result = true;

                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        result = d.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        d.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        d.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        d.Specialty.SpecialtyName.Contains(searchString, StringComparison.OrdinalIgnoreCase);
                    }

                    if (specialty != null && specialty != 0)
                    {
                        result = result && d.Specialty.SpecialtyId == specialty;
                    }

                    if (hospital != null && hospital != 0)
                    {
                        result = result && d.Hospital.HospitalId == hospital;
                    }

                    if (locality != null && locality != 0)
                    {
                        result = result && d.Hospital.Address.Locality.LocalityId == locality;
                    }

                    return result;
                };

                Func<IQueryable<Doctor>, IOrderedQueryable<Doctor>> orderBy = (doctors) =>
                {
                    totalCount = doctors.Count();

                    switch (sortOrder)
                    {
                        case DoctorSortState.NameAsc:
                            doctors = doctors.OrderBy(d => d.Name);
                            break;
                        case DoctorSortState.NameDesc:
                            doctors = doctors.OrderByDescending(d => d.Name);
                            break;
                        case DoctorSortState.SurnameAsc:
                            doctors = doctors.OrderBy(d => d.Surname);
                            break;
                        case DoctorSortState.SurnameDesc:
                            doctors = doctors.OrderByDescending(d => d.Surname);
                            break;
                        case DoctorSortState.EmailAsc:
                            doctors = doctors.OrderBy(d => d.Email);
                            break;
                        case DoctorSortState.EmailDesc:
                            doctors = doctors.OrderByDescending(d => d.Email);
                            break;
                        case DoctorSortState.PhoneAsc:
                            doctors = doctors.OrderBy(d => d.PhoneNumber);
                            break;
                        case DoctorSortState.PhoneDesc:
                            doctors = doctors.OrderByDescending(d => d.PhoneNumber);
                            break;
                        case DoctorSortState.SpecialtyAsc:
                            doctors = doctors.OrderBy(d => d.Specialty.SpecialtyName);
                            break;
                        case DoctorSortState.SpecialtyDesc:
                            doctors = doctors.OrderByDescending(d => d.Specialty.SpecialtyName);
                            break;
                        case DoctorSortState.HospitalAsc:
                            doctors = doctors.OrderBy(d => d.Hospital.HospitalName);
                            break;
                        case DoctorSortState.HospitalDesc:
                            doctors = doctors.OrderByDescending(d => d.Hospital.HospitalName);
                            break;
                        default:
                            doctors = doctors.OrderBy(d => d.Id);
                            break;
                    }

                    return (IOrderedQueryable<Doctor>)doctors;
                };

                var doctors = await _uow.Doctors
                    .GetAllAsync(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize,
                    include: d => d
                    .Include(d => d.Appointments)
                        .ThenInclude(a => a.Diagnosis)
                    .Include(d => d.Schedules)
                    .Include(d => d.Hospital)
                        .ThenInclude(h => h.Address)
                                .ThenInclude(a => a.Locality)
                    .Include(d => d.Specialty));

                Response.Headers.Add("TotalCount", totalCount.ToString());
                Response.Headers.Add("Count", doctors.Count().ToString());
                Response.Headers.Add("PageSize", pageSize.ToString());
                Response.Headers.Add("PageNumber", pageNumber.ToString());
                Response.Headers.Add("TotalPages", ((int)Math.Ceiling(totalCount / (double)pageSize)).ToString());

                return new ObjectResult(doctors);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DoctorsController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Doctor found by Id or Email
        /// </summary>
        /// <param name="searchString">Doctor's id or Email</param>
        /// <returns>The Doctor object</returns>
        [HttpGet("{searchString}")]
        public async Task<ActionResult<Doctor>> Get(string searchString)
        {
            try
            {
                var doctor = await _uow.Doctors.GetAsync(d => d.Id == searchString || d.Email == searchString,
                include: d => d
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Diagnosis)
                .Include(d => d.Schedules)
                .Include(d => d.Hospital)
                    .ThenInclude(h => h.Address)
                            .ThenInclude(a => a.Locality)
                .Include(d => d.Specialty));

                if (doctor == null)
                {
                    return NotFound("The doctor object wasn't found");
                }

                return new ObjectResult(doctor);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DoctorsController.Get(searchString): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new Doctor
        /// </summary>
        /// <param name="doctor">Doctor to create</param>
        /// <returns>The Doctor object</returns>
        [HttpPost]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Doctor>> Post(DoctorResourceModel doctor)
        {
            try
            {
                if (doctor == null)
                {
                    return BadRequest("Passing null object to the DoctorsController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<DoctorResourceModel, Doctor>()
                    .ForMember(d => d.Image, o => o.Ignore()));
                var mapper = new Mapper(config);

                var entity = mapper.Map<DoctorResourceModel, Doctor>(doctor);

                IdentityResult result;

                if (string.IsNullOrWhiteSpace(doctor.Password))
                {
                    result = await _userManager.CreateAsync(entity);
                }
                else
                {
                    result = await _userManager.CreateAsync(entity, doctor.Password);
                }

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(entity, "Doctor");

                    _logger.LogDebug($"Created doctor with id {entity.Id}");

                    return Ok(entity);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DoctorsController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Doctor object data
        /// </summary>
        /// <param name="doctor">The Doctor to update</param>
        /// <returns>The Doctor object</returns>
        [HttpPut]
        [Authorize(Policy = "AdminsDoctorsOnly")]
        public async Task<ActionResult<Doctor>> Put(Doctor doctor)
        {
            try
            {
                if (doctor == null)
                {
                    return BadRequest("Passing null object to the DoctorsController.Put method");
                }

                var result = await _userManager.UpdateAsync(doctor);

                if (result.Succeeded)
                {
                    _logger.LogDebug($"Updated doctor with id {doctor.Id}");

                    return Ok(doctor);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DoctorsController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Doctor found by Id or Email
        /// </summary>
        /// <param name="searchString">Doctor's id or Email</param>
        /// <returns>The Doctor object</returns>
        [HttpDelete("{searchString}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Doctor>> Delete(string searchString)
        {
            try
            {
                var doctor = await _uow.Doctors.GetAsync(d => d.Id == searchString || d.Email == searchString);

                if (doctor == null)
                {
                    return NotFound("The doctor object wasn't found");
                }

                await _userManager.DeleteAsync(doctor);

                _logger.LogDebug($"Deleteed doctor with id {doctor.Id}");

                return Ok(doctor);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DoctorsController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
