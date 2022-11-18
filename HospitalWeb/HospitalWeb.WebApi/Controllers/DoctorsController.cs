using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HospitalWeb.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger;
        private readonly ApiUnitOfWork _uow;

        public DoctorsController(
            ILogger<DoctorsController> logger,
            ApiUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

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

            var metadata = new
            {
                TotalCount = totalCount,
                Count = doctors.Count(),
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            Response.Headers.Add("Pagination", JsonConvert.SerializeObject(metadata));

            return doctors;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> Get(string id)
        {
            var doctor = await _uow.Doctors.GetAsync(d => d.Id == id || d.Email == id);

            if (doctor == null)
            {
                return NotFound();
            }

            return new ObjectResult(doctor);
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> Post(Doctor doctor)
        {
            if (doctor == null)
            {
                return BadRequest();
            }

            await _uow.Doctors.CreateAsync(doctor);

            return Ok(doctor);
        }

        [HttpPut]
        public async Task<ActionResult<Doctor>> Put(Doctor doctor)
        {
            if (doctor == null)
            {
                return BadRequest();
            }

            await _uow.Doctors.UpdateAsync(doctor);

            return Ok(doctor);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Doctor>> Delete(string id)
        {
            var doctor = await _uow.Doctors.GetAsync(d => d.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            await _uow.Doctors.DeleteAsync(doctor);

            return Ok(doctor);
        }

        [HttpDelete("{Doctor}")]
        public async Task<ActionResult<Doctor>> Delete(Doctor doctor)
        {
            if (doctor == null)
            {
                return NotFound();
            }

            await _uow.Doctors.DeleteAsync(doctor);

            return Ok(doctor);
        }
    }
}
