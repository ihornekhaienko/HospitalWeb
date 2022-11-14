using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Appointments;
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
        public async Task<IActionResult> Details(int id)
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

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
    }
}
