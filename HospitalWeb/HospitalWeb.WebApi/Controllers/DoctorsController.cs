using AutoMapper;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        private readonly UnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;

        public DoctorsController(
            ILogger<DoctorsController> logger,
            UnitOfWork uow,
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
        /// <param name="sortOrder">Sorting order of the filtered list</param>
        /// <param name="pageSize">Count of the result on one page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns>Filtered list of Doctors</returns>
        [HttpGet]
        public async Task<IEnumerable<Doctor>> Get(
            string searchString = null,
            int? specialty = null,
            DoctorSortState sortOrder = DoctorSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
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
                    default:
                        doctors = doctors.OrderBy(d => d.Id);
                        break;
                }

                return (IOrderedQueryable<Doctor>)doctors;
            };

            var doctors = await _uow.Doctors
                .GetAllAsync(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize);

            Response.Headers.Add("TotalCount", totalCount.ToString());
            Response.Headers.Add("Count", doctors.Count().ToString());
            Response.Headers.Add("PageSize", pageSize.ToString());
            Response.Headers.Add("PageNumber", pageNumber.ToString());
            Response.Headers.Add("TotalPages", ((int)Math.Ceiling(totalCount / (double)pageSize)).ToString());

            return doctors;
        }

        /// <summary>
        /// Returns the Doctor found by Id or Email
        /// </summary>
        /// <param name="searchString">Doctor's id or Email</param>
        /// <returns>The Doctor object</returns>
        [HttpGet("{searchString}")]
        public async Task<ActionResult<Doctor>> Get(string searchString)
        {
            var doctor = await _uow.Doctors.GetAsync(d => d.Id == searchString || d.Email == searchString);

            if (doctor == null)
            {
                return NotFound();
            }

            return new ObjectResult(doctor);
        }

        /// <summary>
        /// Creates the new Doctor
        /// </summary>
        /// <param name="doctor">Doctor to create</param>
        /// <returns>The Doctor object</returns>
        [HttpPost]
        public async Task<ActionResult<Doctor>> Post(DoctorResourceModel doctor)
        {
            if (doctor == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<DoctorResourceModel, Doctor>());
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

                return Ok(entity);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Updates the Doctor object data
        /// </summary>
        /// <param name="doctor">The Doctor to update</param>
        /// <returns>The Doctor object</returns>
        [HttpPut]
        public async Task<ActionResult<Doctor>> Put(DoctorResourceModel doctor)
        {
            if (doctor == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<DoctorResourceModel, Doctor>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<DoctorResourceModel, Doctor>(doctor);

            var result = await _userManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                return Ok(entity);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Deletes the Doctor found by Id or Email
        /// </summary>
        /// <param name="searchString">Doctor's id or Email</param>
        /// <returns>The Doctor object</returns>
        [HttpDelete("{searchString}")]
        public async Task<ActionResult<Doctor>> Delete(string searchString)
        {
            var doctor = await _uow.Doctors.GetAsync(d => d.Id == searchString || d.Email == searchString);

            if (doctor == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(doctor);

            return Ok(doctor);
        }
    }
}
