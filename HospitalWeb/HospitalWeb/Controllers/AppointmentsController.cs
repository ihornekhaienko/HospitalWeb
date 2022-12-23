using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Appointments;
using HospitalWeb.ViewModels.Error;
using HospitalWeb.Clients.Implementations;
using HospitalWeb.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HospitalWeb.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IStringLocalizer<ExceptionResource> _errLocalizer;
        private readonly IWebHostEnvironment _environment;
        private readonly ApiUnitOfWork _api;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenManager _tokenManager;
        private readonly ICalendarService _calendar;
        private readonly IFileManager _fileManager;
        private readonly IPdfPrinter _pdfPrinter;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IStringLocalizer<ExceptionResource> errLocalizer,
            IWebHostEnvironment environment,
            ApiUnitOfWork api,
            UserManager<AppUser> userManager,
            ITokenManager tokenManager,
            ICalendarService calendar,
            IFileManager fileManager,
            IPdfPrinter pdfPrinter
            )
        {
            _logger = logger;
            _errLocalizer = errLocalizer;
            _environment = environment;
            _api = api;
            _userManager = userManager;
            _tokenManager = tokenManager;
            _calendar = calendar;
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
                var statusCode = response.StatusCode;
                var message = _api.Appointments.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

                var response = _api.Doctors.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Doctors.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var user = _api.Doctors.Read(response);

                ViewBag.Calendar = _calendar.GetCalendar(user);

                var builder = new AppointmentsViewModelBuilder(_api,
                    page, searchString, sortOrder, user.Id, state: state, fromTime: fromDate, toTime: toDate);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var viewModel = builder.GetViewModel();

                return View(viewModel);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.History.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Today(
                    string searchString,
                    int? state,
                    int page = 1,
                    AppointmentSortState sortOrder = AppointmentSortState.DateAsc)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

                var response = _api.Doctors.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Doctors.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var user = _api.Doctors.Read(response);

                ViewBag.Calendar = _calendar.GetCalendar(user);

                var start = DateTime.Today;
                var end = start.AddDays(1).AddTicks(-1);

                var builder = new AppointmentsViewModelBuilder(_api,
                    page, searchString, sortOrder, user.Id, state: state, fromTime: start, toTime: end);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var viewModel = builder.GetViewModel();

                return View(viewModel);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Today.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var response = _api.Appointments.Get(id);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Appointments.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var appointment = _api.Appointments.Read(response);

                if (appointment.State != State.Planned)
                {
                    throw new Exception(string.Format(_errLocalizer["MustBePlannedToCancel"].Value, appointment.State));
                }

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                appointment.State = State.Canceled;

                _api.Appointments.Put(appointment, tokenResult.Token, tokenResult.Provider);

                var meeting = appointment.Meetings.FirstOrDefault();
                if (meeting != null)
                {
                    _api.Meetings.Delete(meeting.MeetingId, tokenResult.Token, tokenResult.Provider);
                }

                await _calendar.CancelEvent(appointment.Doctor, appointment);
                await _calendar.CancelEvent(appointment.Patient, appointment);

                return RedirectToAction("History", "Appointments");
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Cancel.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

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
                var statusCode = response.StatusCode;
                var message = _api.Appointments.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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
        public async Task<IActionResult> Fill(AppointmentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _api.Appointments.Get(model.AppointmentId);

                    if (!response.IsSuccessStatusCode)
                    {
                        var statusCode = response.StatusCode;
                        var message = _api.Appointments.ReadError<string>(response);

                        return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                    }

                    var appointment = _api.Appointments.Read(response);

                    if (appointment.State != State.Planned)
                    {
                        throw new Exception(string.Format(_errLocalizer["MustBePlannedToSubmit"].Value, appointment.State));
                    }

                    response = _api.Diagnoses.Get(model.Diagnosis);

                    var user = await _userManager.GetUserAsync(User);
                    var tokenResult = await _tokenManager.GetToken(user);

                    var diagnosis = _api.Diagnoses.GetOrCreate(model.Diagnosis, tokenResult.Token, tokenResult.Provider);

                    appointment.DiagnosisId = diagnosis.DiagnosisId;
                    appointment.Prescription = model.Prescription;
                    appointment.State = State.Completed;
                    _api.Appointments.Put(appointment, tokenResult.Token, tokenResult.Provider);

                    var meeting = appointment.Meetings.FirstOrDefault();
                    if (meeting != null)
                    {
                        _api.Meetings.Delete(meeting.MeetingId, tokenResult.Token, tokenResult.Provider);
                    }

                    await _calendar.ConfirmEvent(appointment.Doctor, appointment);
                    await _calendar.ConfirmEvent(appointment.Patient, appointment);

                    return RedirectToAction("Today", "Appointments");
                }

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Fill.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpGet]
        public IActionResult Print(int id)
        {
            try
            {
                var response = _api.Appointments.Get(id);
                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Appointments.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }
                var appointment = _api.Appointments.Read(response);

                string filePath = Path.Combine(_environment.WebRootPath, $"files/docs/{id}.pdf");
                _pdfPrinter.PrintAppointment(appointment, filePath);

                return PhysicalFile(filePath, "application/json", $"{id}.pdf");
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppointmentsController.Print.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
    }
}
