using HospitalWeb.DAL.Entities;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Appointments;
using HospitalWeb.ViewModels.Error;
using HospitalWeb.WebApi.Clients.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ApiUnitOfWork _api;
        private readonly IFileManager _fileManager;
        private readonly IPdfPrinter _pdfPrinter;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IWebHostEnvironment environment,
            ApiUnitOfWork api,
            IFileManager fileManager,
            IPdfPrinter pdfPrinter
            )
        {
            _logger = logger;
            _environment = environment;
            _api = api;
            _fileManager = fileManager;
            _pdfPrinter = pdfPrinter;
        }

        [Authorize(Roles = "Doctor, Patient")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var response = _api.Appointments.Get(id);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var appointment = _api.Appointments.Read(response);

            var model = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                Diagnosis = appointment.Diagnosis.DiagnosisName,
                Prescription = appointment.Prescription,
                AppointmentDate = appointment.AppointmentDate,
                DoctorId = appointment.Doctor.Id,
                Doctor = appointment.Doctor.ToString(),
                DoctorSpecialty = appointment.Doctor.Specialty.SpecialtyName,
                Patient = appointment.Patient.ToString()
            };

            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> History(
            string searchString,
            int? state,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            AppointmentSortState sortOrder = AppointmentSortState.DateDesc)
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

            var response = _api.Doctors.Get(User.Identity.Name);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var userId = _api.Doctors.Read(response).Id;

            var builder = new AppointmentsViewModelBuilder(_api,
                page, searchString, sortOrder, userId, state: state, fromTime: fromDate, toTime: toDate);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Today(
                    string searchString,
                    int? state,
                    int page = 1,
                    AppointmentSortState sortOrder = AppointmentSortState.DateAsc)
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

            var response = _api.Doctors.Get(User.Identity.Name);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var userId = _api.Doctors.Read(response).Id;

            var start = DateTime.Today;
            var end = start.AddDays(1).AddTicks(-1);

            var builder = new AppointmentsViewModelBuilder(_api,
                page, searchString, sortOrder, userId, state: state, fromTime: start, toTime: end);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public IActionResult Cancel(int id)
        {
            try
            {
                var response = _api.Appointments.Get(id);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var appointment = _api.Appointments.Read(response);

                if (appointment.State == State.Planned)
                {
                    appointment.State = State.Canceled;
                    _api.Appointments.Put(appointment);

                    return RedirectToAction("History", "Appointments");
                }
                else
                {
                    throw new Exception($"Appointment has to planned to cancel it, whereas it is {appointment.State}");
                }
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public IActionResult Fill(int id)
        {
            var response = _api.Diagnoses.Get();

            ViewBag.Diagnoses = _api.Diagnoses
                .ReadMany(response)
                .OrderBy(d => d.DiagnosisName)
                .Select(d => d.DiagnosisName);

            response = _api.Appointments.Get(id);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var appointment = _api.Appointments.Read(response);

            var model = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                DoctorId = appointment.Doctor.Id,
                Doctor = appointment.Doctor.ToString(),
                DoctorSpecialty = appointment.Doctor.Specialty.SpecialtyName,
                Patient = appointment.Patient.ToString(),
                PatientId = appointment.PatientId
            };

            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public IActionResult Fill(AppointmentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _api.Appointments.Get(model.AppointmentId);

                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound();
                    }

                    var appointment = _api.Appointments.Read(response);

                    if (appointment.State == State.Planned)
                    {
                        response = _api.Diagnoses.Get(model.Diagnosis);
                        var diagnosis = _api.Diagnoses.GetOrCreate(model.Diagnosis);

                        appointment.DiagnosisId = diagnosis.DiagnosisId;
                        appointment.Prescription = model.Prescription;
                        appointment.State = State.Completed;
                        _api.Appointments.Put(appointment);

                        return RedirectToAction("Today", "Appointments");
                    }
                    else
                    {
                        throw new Exception($"Appointment has to be planned to submit it, whereas it is {appointment.State}");
                    }
                }

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpGet]
        public IActionResult Print(int id)
        {
            var response = _api.Appointments.Get(id);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var appointment = _api.Appointments.Read(response);

            string filePath = Path.Combine(_environment.WebRootPath, $"files/docs/{id}.pdf");
            _pdfPrinter.PrintAppointment(appointment, filePath);

            return PhysicalFile(filePath, "application/json", $"{id}.pdf");
        }
    }
}
