using HospitalWeb.Services.Interfaces;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.ViewModels.Administration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Filters.Models.SortStates;
using Microsoft.AspNetCore.Authorization;

namespace HospitalWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly ILogger<AdministrationController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly UnitOfWork _uow;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly INotifier _notifier;

        public AdministrationController(
            ILogger<AdministrationController> logger,
            UserManager<AppUser> userManager,
            UnitOfWork uow,
            IPasswordGenerator passwordGenerator,
            INotifier notifier
            )
        {
            _logger = logger;
            _userManager = userManager;
            _uow = uow;
            _passwordGenerator = passwordGenerator;
            _notifier = notifier;
        }

        [HttpGet]
        public IActionResult Admins(
            string searchString,
            int page = 1,
            AdminSortState sortOrder = AdminSortState.Id)
        {
            ViewBag.CurrentAdmin = _uow.Admins
                .Get(a => a.UserName == User.Identity.Name);

            var builder = new AdminsViewModelBuilder(_uow, page, searchString, sortOrder);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

        public async Task<IActionResult> DeleteAdmin(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var admin = _uow.Admins
                    .Get(m => m.Id == id);

                if (admin == null)
                {
                    return NotFound();
                }

                var result = await _userManager.DeleteAsync(admin);

                if (result.Succeeded)
                {
                    await _notifier.NotifyDelete(admin.Email, admin.Email);
                }

                return RedirectToAction("Admins", "Administration");
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", err.Message);
            }
        }

        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(AdminViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var admin = new Admin
                    {
                        Name = model.Name,
                        Surname = model.Surname,
                        Email = model.Email,
                        UserName = model.Email,
                        PhoneNumber = model.Phone,
                        IsSuperAdmin = model.IsSuperAdmin,
                        EmailConfirmed = true
                    };

                    var password = _passwordGenerator.GeneratePassword(null);

                    var result = await _userManager.CreateAsync(admin, password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(admin, "Admin");
                        await _notifier.NotifyAdd(admin.Email, admin.Email, password);

                        return RedirectToAction("Admins", "Administration");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", err.Message);
            }
        }

        [HttpGet]
        public IActionResult EditAdmin(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var admin = _uow.Admins
                .Get(a => a.Id == id);

            if (admin == null)
            {
                return NotFound();
            }

            var model = new AdminViewModel
            {
                Name = admin.Name,
                Surname = admin.Surname,
                Email = admin.Email,
                Phone = admin.PhoneNumber,
                IsSuperAdmin = admin.IsSuperAdmin,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAdmin(AdminViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var admin = _uow.Admins
                        .Get(a => a.Email == model.Email);

                    admin.UserName = model.Email;
                    admin.Email = model.Email;
                    admin.Name = model.Name;
                    admin.Surname = model.Surname;
                    admin.PhoneNumber = model.Phone;
                    admin.IsSuperAdmin = model.IsSuperAdmin;

                    var result = await _userManager.UpdateAsync(admin);

                    if (result.Succeeded)
                    {
                        await _notifier.NotifyUpdate(admin.Email, admin.Email);

                        return RedirectToAction("Admins", "Administration");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", err.Message);
            }
        }

        [HttpGet]
        public IActionResult Doctors(
            string searchString,
            int? specialty,
            int page = 1,
            DoctorSortState sortOrder = DoctorSortState.Id)
        {
            var builder = new DoctorsViewModelBuilder(_uow, page, searchString, sortOrder, specialty);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

        public async Task<IActionResult> DeleteDoctor(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var doctor = _uow.Doctors
                    .Get(m => m.Id == id);

                if (doctor == null)
                {
                    return NotFound();
                }

                var result = await _userManager.DeleteAsync(doctor);

                if (result.Succeeded)
                {
                    await _notifier.NotifyDelete(doctor.Email, doctor.Email);
                }

                return RedirectToAction("Doctors", "Administration");
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", err.Message);
            }
        }

        [HttpGet]
        public IActionResult CreateDoctor()
        {
            ViewBag.Specialties = _uow.Specialties
                .GetAll()
                .Select(s => s.SpecialtyName)
                .OrderBy(s => s);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(DoctorViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var specialty = _uow.Specialties.GetOrCreate(model.Specialty);
                    var doctor = new Doctor
                    {
                        Name = model.Name,
                        Surname = model.Surname,
                        Email = model.Email,
                        UserName = model.Email,
                        PhoneNumber = model.Phone,
                        Specialty = specialty,
                        EmailConfirmed = true
                    };
                    var password = _passwordGenerator.GeneratePassword(null);

                    var result = await _userManager.CreateAsync(doctor, password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(doctor, "Doctor");
                        await _notifier.NotifyAdd(doctor.Email, doctor.Email, password);

                        return RedirectToAction("Doctors", "Administration");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", err.Message);
            }
        }

        [HttpGet]
        public IActionResult EditDoctor(string id)
        {
            ViewBag.Specialties = _uow.Specialties.GetAll().Select(s => s.SpecialtyName);
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var doctor = _uow.Doctors
                .Get(d => d.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            var model = new DoctorViewModel
            {
                Name = doctor.Name,
                Surname = doctor.Surname,
                Email = doctor.Email,
                Phone = doctor.PhoneNumber,
                Specialty = doctor.Specialty.SpecialtyName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctor(DoctorViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var specialty = _uow.Specialties
                        .GetOrCreate(model.Specialty);
                    var doctor = _uow.Doctors
                        .Get(a => a.Email == model.Email);

                    if (doctor == null || specialty == null)
                    {
                        return NotFound();
                    }

                    doctor.UserName = model.Email;
                    doctor.Email = model.Email;
                    doctor.Name = model.Name;
                    doctor.Surname = model.Surname;
                    doctor.PhoneNumber = model.Phone;
                    doctor.Specialty = specialty;

                    var result = await _userManager.UpdateAsync(doctor);

                    if (result.Succeeded)
                    {
                        await _notifier.NotifyUpdate(doctor.Email, doctor.Email);

                        return RedirectToAction("Doctors", "Administration");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", err.Message);
            }
        }

        [HttpGet]
        public IActionResult Patients(
            string searchString,
            int? locality,
            int page = 1,
            PatientSortState sortOrder = PatientSortState.Id)
        {
            var builder = new PatientsViewModelBuilder(_uow, page, searchString, sortOrder, locality);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

        public async Task<IActionResult> DeletePatient(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var patient = _uow.Patients
                    .Get(m => m.Id == id);

                if (patient == null)
                {
                    return NotFound();
                }

                var result = await _userManager.DeleteAsync(patient);

                if (result.Succeeded)
                {
                    await _notifier.NotifyDelete(patient.Email, patient.Email);
                }

                return RedirectToAction("Patients", "Administration");
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", err.Message);
            }
        }
        
        public IActionResult DoctorSchedule(string id, string day)
        {
            if (_uow.Schedules.Contains(s => s.Doctor.Id == id && s.DayOfWeek.ToString() == day))
            {
                return RedirectToAction("EditDoctorSchedule", "Administration", new { id = id, day = day });
            }
            else
            {
                return RedirectToAction("AddDoctorSchedule", "Administration", new { id = id, day = day });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddDoctorSchedule(string id, string day)
        {
            DoctorSlotViewModel model;

            var doctor = await _userManager.FindByIdAsync(id);

            model = new DoctorSlotViewModel
            {
                DoctorId = id,
                DoctorFullName = doctor.ToString(),
                DayOfWeek = day
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctorSchedule(DoctorSlotViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doctor = await _userManager.FindByIdAsync(model.DoctorId) as Doctor;
                DayOfWeek dayOfWeek;
                Enum.TryParse(model.DayOfWeek, out dayOfWeek);

                var schedule = new Schedule
                {
                    Doctor = doctor,
                    DayOfWeek = dayOfWeek,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                };

                _uow.Schedules.Create(schedule);

                return RedirectToAction("DoctorSchedule", "Administration", new { id = model.DoctorId, day = model.DayOfWeek });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditDoctorSchedule(string id, string day)
        {
            var schedule = _uow.Schedules.GetDoctorScheduleByDay(id, day);

            DoctorSlotViewModel model;

            model = new DoctorSlotViewModel
            {
                ScheduleId = schedule.ScheduleId,
                DoctorId = id,
                DoctorFullName = schedule.Doctor.ToString(),
                DayOfWeek = schedule.DayOfWeek.ToString(),
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditDoctorSchedule(DoctorSlotViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ScheduleId == null)
                {
                    return NotFound();
                }
                
                var schedule = _uow.Schedules.Get(s => s.ScheduleId == (int)model.ScheduleId);

                schedule.StartTime = model.StartTime;
                schedule.EndTime = model.EndTime;

                _uow.Schedules.Update(schedule);

                return RedirectToAction("DoctorSchedule", "Administration", new { id = model.DoctorId, day = model.DayOfWeek });
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteDoctorSchedule(DoctorSlotViewModel model)
        {
            if (model.ScheduleId == null)
            {
                return NotFound();
            }

            var schedule = _uow.Schedules.Get(s => s.ScheduleId == (int)model.ScheduleId);
            _uow.Schedules.Delete(schedule);

            return RedirectToAction("Doctors", "Administration");
        }
    }
}
