using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Filters.Models.SortStates;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    [Authorize(Roles = "Patient")]
    public class TreatmentController : Controller
    {
        private readonly ILogger<TreatmentController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly UnitOfWork _uow;
        private readonly IFileManager _fileManager;

        public TreatmentController(
            ILogger<TreatmentController> logger,
            IWebHostEnvironment environment,
            UnitOfWork uow,
            IFileManager fileManager
            )
        {
            _logger = logger;
            _environment = environment;
            _uow = uow;
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
            var userId = _uow.Patients.Get(p => p.Email == User.Identity.Name).Id;

            var builder = new AppointmentsViewModelBuilder(_uow, 
                page, searchString, sortOrder, state: state, fromTime: fromDate, toTime: toDate, patientId: userId);
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
                var appointment = _uow.Appointments
                    .Get(a => a.AppointmentId == id);

                if (appointment.State == State.Planned)
                {
                    appointment.State = State.Canceled;
                    _uow.Appointments.Update(appointment);

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
