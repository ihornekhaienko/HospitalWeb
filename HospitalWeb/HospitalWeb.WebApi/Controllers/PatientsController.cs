using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HospitalWeb.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger;
        private readonly UnitOfWork _uow;

        public PatientsController(
            ILogger<PatientsController> logger,
            UnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        [HttpGet]
        public IEnumerable<Patient> Get(
            string searchString,
            int? locality,
            PatientSortState sortOrder = PatientSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
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

            var patients = _uow.Patients
                .GetAll(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize);

            var metadata = new
            {
                TotalCount = totalCount,
                Count = patients.Count(),
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            Response.Headers.Add("Pagination", JsonConvert.SerializeObject(metadata));

            return patients;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> Get(string id)
        {
            return await _uow.Patients.GetAsync(p => p.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> Post(Patient patient)
        {
            if (patient == null)
            {
                return BadRequest();
            }

            await _uow.Patients.CreateAsync(patient);

            return Ok(patient);
        }

        [HttpPut]
        public async Task<ActionResult<Patient>> Put(Patient patient)
        {
            if (patient == null)
            {
                return BadRequest();
            }

            await _uow.Patients.UpdateAsync(patient);

            return Ok(patient);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Patient>> Delete(string id)
        {
            var patient = await _uow.Patients.GetAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            await _uow.Patients.DeleteAsync(patient);

            return Ok(patient);
        }
    }
}
