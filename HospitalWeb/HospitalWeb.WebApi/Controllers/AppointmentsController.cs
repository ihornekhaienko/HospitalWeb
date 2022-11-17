using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Filters.Models.ViewModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HospitalWeb.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly UnitOfWork _uow;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            UnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        [HttpGet]
        public IEnumerable<Appointment> Get(
            string searchString,
            string userId,
            int? state,
            DateTime? fromDate,
            DateTime? toDate,
            AppointmentSortState sortOrder = AppointmentSortState.DateAsc,
            int pageSize = 10,
            int pageNumber = 1)
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

            var appointments = _uow.Appointments
                .GetAll(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize);

            var metadata = new
            {
                TotalCount = totalCount,
                Count = appointments.Count(),
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            Response.Headers.Add("Pagination", JsonConvert.SerializeObject(metadata));

            return appointments;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> Get(int id)
        {
            return await _uow.Appointments.GetAsync(a => a.AppointmentId == id);
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> Post(Appointment appointment)
        {
            if (appointment == null)
            {
                return BadRequest();
            }

            await _uow.Appointments.CreateAsync(appointment);

            return Ok(appointment);
        }

        [HttpPut]
        public async Task<ActionResult<Appointment>> Put(Appointment appointment)
        {
            if (appointment == null)
            {
                return BadRequest();
            }

            await _uow.Appointments.UpdateAsync(appointment);

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Appointment>> Delete(int id)
        {
            var appointment = await _uow.Appointments.GetAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            await _uow.Appointments.DeleteAsync(appointment);

            return Ok(appointment);
        }
    }
}
