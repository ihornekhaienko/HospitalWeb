using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;

        public PatientsController(
            ILogger<PatientsController> logger,
            UnitOfWork uow,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _uow = uow;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IEnumerable<Patient>> Get(
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

            var patients = await _uow.Patients
                .GetAllAsync(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize);

            Response.Headers.Add("TotalCount", totalCount.ToString());
            Response.Headers.Add("Count", patients.Count().ToString());
            Response.Headers.Add("PageSize", pageSize.ToString());
            Response.Headers.Add("PageNumber", pageNumber.ToString());
            Response.Headers.Add("TotalPages", ((int)Math.Ceiling(totalCount / (double)pageSize)).ToString());

            return patients;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> Get(string id)
        {
            var patient = await _uow.Patients.GetAsync(p => p.Id == id || p.Email == id);

            if (patient == null)
            {
                return NotFound();
            }

            return new ObjectResult(patient);
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> Post(PatientResourceModel patient)
        {
            try
            {
                if (patient == null)
                {
                    return BadRequest();
                }

                var entity = new Patient
                {
                    Name = patient.Name,
                    Surname = patient.Surname,
                    Email = patient.Email,
                    UserName = patient.Email,
                    PhoneNumber = patient.PhoneNumber,
                    AddressId = patient.AddressId,
                    Sex = patient.Sex,
                    BirthDate = patient.BirthDate
                };

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

                    return Ok(entity);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.InnerException.Message);
                return BadRequest();
            }
        }

        [HttpPut]
        public async Task<ActionResult<Patient>> Put(Patient patient)
        {
            if (patient == null)
            {
                return BadRequest();
            }

            var result = await _userManager.UpdateAsync(patient);

            if (result.Succeeded)
            {
                return Ok(patient);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Patient>> Delete(string id)
        {
            var patient = await _uow.Patients.GetAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(patient);

            return Ok(patient);
        }

        [HttpDelete("{Patient}")]
        public async Task<ActionResult<Patient>> Delete(Patient patient)
        {
            if (patient == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(patient);

            return Ok(patient);
        }
    }
}
