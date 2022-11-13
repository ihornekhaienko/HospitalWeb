using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Filters.Models.SortStates;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Doctors;
using HospitalWeb.ViewModels.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ILogger<DoctorsController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly UnitOfWork _uow;
        private readonly IFileManager _fileManager;
        private readonly IScheduleGenerator _scheduleGenerator;

        public DoctorsController(
            ILogger<DoctorsController> logger,
            IWebHostEnvironment environment,
            UnitOfWork uow,
            IFileManager fileManager,
            IScheduleGenerator scheduleGenerator
            )
        {
            _logger = logger;
            _environment = environment;
            _uow = uow;
            _fileManager = fileManager;
            _scheduleGenerator = scheduleGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            string searchString,
            int? specialty,
            int page = 1,
            DoctorSortState sortOrder = DoctorSortState.Id)
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

            var builder = new DoctorsViewModelBuilder(_uow, page, searchString, sortOrder, specialty);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));
            var doctor = _uow.Doctors
                .Get(d => d.Id == id);          

            var model = new DoctorDetailsViewModel
            {
                Id = doctor.Id,
                FullName = doctor.ToString(),
                Email = doctor.Email,
                Phone = doctor.PhoneNumber,
                Image = doctor.Image,
                Specialty = doctor.Specialty.SpecialtyName
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Schedule(string id, DateTime date)
        {
            var doctor = _uow.Doctors
                .Get(d => d.Id == id);
            var model = _scheduleGenerator.GenerateWeekSchedule(doctor, date);

            return PartialView("_SchedulePartial", model);
        }

        [Authorize(Roles = "Patient")]
        public IActionResult SignUpForAppointment(string doctorId, DateTime date)
        {
            try
            {
                var patient = _uow.Patients.Get(p => p.Email == User.Identity.Name);
                var doctor = _uow.Doctors.Get(d => d.Id == doctorId);
                _logger.LogCritical(_uow.Appointments.IsDateFree(doctor, date).ToString());

               var appointment = new Appointment
                {
                    AppointmentDate = date,
                    State = State.Planned,
                    Doctor = doctor,
                    Patient = patient
                };

                _uow.Appointments.Create(appointment);

                return RedirectToAction("Details", "Doctors", new { id = doctorId });
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
    }
}