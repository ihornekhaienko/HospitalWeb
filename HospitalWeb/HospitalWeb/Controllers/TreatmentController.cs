using HospitalWeb.DAL.Entities;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Error;
using HospitalWeb.WebApi.Clients.Implementations;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    [Authorize(Roles = "Patient")]
    public class TreatmentController : Controller
    {
        private readonly ILogger<TreatmentController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ApiUnitOfWork _api;
        private readonly IFileManager _fileManager;

        public TreatmentController(
            ILogger<TreatmentController> logger,
            IWebHostEnvironment environment,
            ApiUnitOfWork api,
            IFileManager fileManager
            )
        {
            _logger = logger;
            _environment = environment;
            _api = api;
            _fileManager = fileManager;
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
            var response = _api.Patients.Get(User.Identity.Name);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var userId = _api.Patients.Read(response).Id;

            var builder = new AppointmentsViewModelBuilder(_api, 
                page, searchString, sortOrder, userId, state: state, fromTime: fromDate, toTime: toDate);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

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

                    var meeting = appointment.Meetings.FirstOrDefault();
                    if (meeting != null)
                    {
                        _api.Meetings.Delete(meeting.MeetingId);
                    }

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
