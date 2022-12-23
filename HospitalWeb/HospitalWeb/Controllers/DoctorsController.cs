using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Doctors;
using HospitalWeb.ViewModels.Error;
using HospitalWeb.Clients.Implementations;
using HospitalWeb.Models.ResourceModels;
using HospitalWeb.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ILogger<DoctorsController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ApiUnitOfWork _api;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenManager _tokenManager;
        private readonly ICalendarService _calendar;
        private readonly IFileManager _fileManager;
        private readonly IScheduleGenerator _scheduleGenerator;
        private readonly IMeetingService _meetingService;

        public DoctorsController(
            ILogger<DoctorsController> logger,
            IWebHostEnvironment environment,
            ApiUnitOfWork api,
            UserManager<AppUser> userManager,
            ITokenManager tokenManager,
            ICalendarService calendar,
            IFileManager fileManager,
            IScheduleGenerator scheduleGenerator,
            IMeetingService meetingService
            )
        {
            _logger = logger;
            _environment = environment;
            _api = api;
            _userManager = userManager;
            _tokenManager = tokenManager;
            _calendar = calendar;
            _fileManager = fileManager;
            _scheduleGenerator = scheduleGenerator;
            _meetingService = meetingService;
        }
         
        [HttpGet]
        public async Task<IActionResult> Search(
            string searchString,
            int? specialty,
            int? hospital,
            int? locality,
            int page = 1,
            DoctorSortState sortOrder = DoctorSortState.Id)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

                var builder = new DoctorsViewModelBuilder(_api, page, searchString, sortOrder, specialty, hospital, locality);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var viewModel = builder.GetViewModel();

                return View(viewModel);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DoctorsController.Search.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

                var response = _api.Doctors.Get(id, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Doctors.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var doctor = _api.Doctors.Read(id);

                var model = new DoctorDetailsViewModel
                {
                    Id = doctor.Id,
                    FullName = doctor.ToString(),
                    Email = doctor.Email,
                    Phone = doctor.PhoneNumber,
                    Image = doctor.Image,
                    Specialty = doctor.Specialty.SpecialtyName,
                    Hospital = doctor.Hospital.HospitalName,
                    Rating = doctor.Rating,
                    ServicePrice = doctor.ServicePrice
                };

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    ViewBag.Author = user.Id;

                    response = _api.Grades.Filter(user.Id, doctor.Id);

                    if (!response.IsSuccessStatusCode)
                    {
                        var statusCode = response.StatusCode;
                        var message = _api.Grades.ReadError<string>(response);

                        return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                    }

                    var grade = _api.Grades.ReadMany(response).FirstOrDefault();

                    model.MyGrade = grade != null ? grade.Stars : 0;
                }

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DoctorsController.Details.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpGet]
        public IActionResult Schedule(string id, DateTime date)
        {
            var response = _api.Doctors.Get(id, null, null);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Doctors.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            var doctor = _api.Doctors.Read(id);

            var model = _scheduleGenerator.GenerateWeekSchedule(doctor, date);

            return PartialView("_SchedulePartial", model);
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SignUpForAppointment(string doctorId, DateTime date)
        {
            try
            {
                var response = _api.Patients.Get(User.Identity.Name, null, null);
                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Patients.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }
                var patient = _api.Patients.Read(response);

                response = _api.Doctors.Get(doctorId, null, null);
                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Doctors.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }
                var doctor = _api.Doctors.Read(response);

                var appointment = new AppointmentResourceModel
                {
                    AppointmentDate = date,
                    State = State.Planned,
                    DoctorId = doctor.Id,
                    PatientId = patient.Id,
                    Price = doctor.ServicePrice
                };

                var tokenResult = await _tokenManager.GetToken(patient);

                response = _api.Appointments.Post(appointment, tokenResult.Token, tokenResult.Provider);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Appointments.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var entity = _api.Appointments.Read(response);

                var meeting = _meetingService.CreateMeeting(entity);
                _api.Meetings.Post(meeting, tokenResult.Token, tokenResult.Provider);

                await _calendar.CreateEvent(doctor, entity);
                await _calendar.CreateEvent(patient, entity);

                if (entity.Price != 0)
                {
                    await RunPaymentChecker(entity.AppointmentId);

                    return RedirectToAction("PayOff", "Treatment", new { id = entity.AppointmentId });
                }

                return RedirectToAction("Details", "Doctors", new { id = doctorId });
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DoctorsController.SignUpForAppointment.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        private async Task RunPaymentChecker(int id)
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://localhost:7271/jobs/removeUnpaid?{id}");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Access-Control-Allow-Origin", "*");

            var response = await client.SendAsync(request);
        }
    }
}