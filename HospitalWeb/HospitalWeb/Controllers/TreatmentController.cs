using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Error;
using HospitalWeb.WebApi.Clients.Implementations;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    [Authorize(Roles = "Patient")]
    public class TreatmentController : Controller
    {
        private readonly ILogger<TreatmentController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApiUnitOfWork _api;
        private readonly ICalendarService _calendar;
        private readonly IFileManager _fileManager;
        private readonly ITokenManager _tokenManager;

        public TreatmentController(
            ILogger<TreatmentController> logger,
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager,
            ApiUnitOfWork api,
            ICalendarService calendar,
            IFileManager fileManager,
            ITokenManager tokenManager)
        {
            _logger = logger;
            _environment = environment;
            _userManager = userManager;
            _api = api;
            _calendar = calendar;
            _fileManager = fileManager;
            _tokenManager = tokenManager;
        }

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

            var response = _api.Patients.Get(User.Identity.Name, null, null);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var user = _api.Patients.Read(response);

            ViewBag.Calendar = _calendar.GetCalendar(user);

            var builder = new AppointmentsViewModelBuilder(_api, 
                page, searchString, sortOrder, user.Id, state: state, fromTime: fromDate, toTime: toDate);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                var response = _api.Appointments.Get(id, tokenResult.Token, tokenResult.Provider);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var appointment = _api.Appointments.Read(response);

                if (appointment.State == State.Planned)
                {
                    appointment.State = State.Canceled;
                    _api.Appointments.Put(appointment, tokenResult.Token, tokenResult.Provider);

                    var meeting = appointment.Meetings.FirstOrDefault();
                    if (meeting != null)
                    {
                        _api.Meetings.Delete(meeting.MeetingId, tokenResult.Token, tokenResult.Provider);
                    }

                    await _calendar.CancelEvent(appointment.Doctor, appointment);
                    await _calendar.CancelEvent(appointment.Patient, appointment);

                    return RedirectToAction("History", "Treatment");
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
    }
}
