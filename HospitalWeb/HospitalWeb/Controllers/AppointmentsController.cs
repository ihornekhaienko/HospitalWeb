using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Filters.Models.SortStates;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Appointments;
using HospitalWeb.ViewModels.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly UnitOfWork _uow;
        private readonly IFileManager _fileManager;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
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

        [Authorize(Roles = "Doctor, Patient")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            _uow.Appointments.UpdateStates();
            var appointment = _uow.Appointments
                .Get(a => a.AppointmentId == id);

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
            _uow.Appointments.UpdateStates();
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));
            var userId = _uow.Doctors.Get(p => p.Email == User.Identity.Name).Id;

            var builder = new AppointmentsViewModelBuilder(_uow,
                page, searchString, sortOrder, state: state, fromTime: fromDate, toTime: toDate, doctorId: userId);
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
            _uow.Appointments.UpdateStates();
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));
            var userId = _uow.Doctors
                .Get(p => p.Email == User.Identity.Name).Id;
            var start = DateTime.Today;
            var end = start.AddDays(1).AddTicks(-1);

            var builder = new AppointmentsViewModelBuilder(_uow,
                page, searchString, sortOrder, state: state, fromTime: start, toTime: end, doctorId: userId);
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
                _uow.Appointments.UpdateStates();
                var appointment = _uow.Appointments
                    .Get(a => a.AppointmentId == id);

                if (appointment.State == State.Planned)
                {
                    appointment.State = State.Canceled;
                    _uow.Appointments.Update(appointment);

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
            _uow.Appointments.UpdateStates();
            ViewBag.Diagnoses = _uow.Diagnoses
                .GetAll()
                .OrderBy(d => d.DiagnosisName)
                .Select(d => d.DiagnosisName);
            var appointment = _uow.Appointments
                .Get(a => a.AppointmentId == id);

            var model = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                DoctorId = appointment.Doctor.Id,
                Doctor = appointment.Doctor.ToString(),
                DoctorSpecialty = appointment.Doctor.Specialty.SpecialtyName,
                Patient = appointment.Patient.ToString()
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
                    _uow.Appointments.UpdateStates();
                    var appointment = _uow.Appointments
                    .Get(a => a.AppointmentId == model.AppointmentId);

                    if (appointment.State == State.Planned)
                    {
                        appointment.Diagnosis = _uow.Diagnoses.GetOrCreate(model.Diagnosis);
                        appointment.Prescription = model.Prescription;
                        appointment.State = State.Completed;
                        _uow.Appointments.Update(appointment);

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
    }
}
