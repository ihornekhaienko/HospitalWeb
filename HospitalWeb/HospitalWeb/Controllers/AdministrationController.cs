using HospitalWeb.Services.Interfaces;
using HospitalWeb.DAL.Entities;
using HospitalWeb.ViewModels.Administration;
using Microsoft.AspNetCore.Mvc;
using HospitalWeb.Filters.Builders.Implementations;
using Microsoft.AspNetCore.Authorization;
using HospitalWeb.WebApi.Models.SortStates;
using HospitalWeb.WebApi.Clients.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HospitalWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly ILogger<AdministrationController> _logger;
        private readonly ApiUnitOfWork _api;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenManager _tokenManager;
        private readonly ICalendarService _calendar;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly INotifier _notifier;

        public AdministrationController(
            ILogger<AdministrationController> logger,
            ApiUnitOfWork api,
            UserManager<AppUser> userManager,
            ITokenManager tokenManager,
            ICalendarService calendar,
            IPasswordGenerator passwordGenerator,
            INotifier notifier)
        {
            _logger = logger;
            _api = api;
            _userManager = userManager;
            _tokenManager = tokenManager;
            _calendar = calendar;
            _passwordGenerator = passwordGenerator;
            _notifier = notifier;
        }

        [HttpGet]
        public IActionResult Admins(
            string searchString,
            int page = 1,
            AdminSortState sortOrder = AdminSortState.Id)
        {
            var response = _api.Admins.Get(User.Identity.Name, null, null);
            ViewBag.CurrentAdmin = _api.Admins.Read(response);

            var builder = new AdminsViewModelBuilder(_api, page, searchString, sortOrder);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();
            
            return View(viewModel);
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
                    var password = _passwordGenerator.GeneratePassword(null);

                    var admin = new AdminResourceModel
                    {
                        Name = model.Name,
                        Surname = model.Surname,
                        Email = model.Email,
                        UserName = model.Email,
                        PhoneNumber = model.Phone,
                        IsSuperAdmin = model.IsSuperAdmin,
                        EmailConfirmed = true,
                        Password = password
                    };

                    var response =_api.Admins.Post(admin);

                    if (response.IsSuccessStatusCode)
                    {
                        await _notifier.NotifyAdd(admin.Email, admin.Email, password);

                        return RedirectToAction("Admins", "Administration");
                    }
                    else
                    {
                        var errors = _api.Admins.ReadErrors(response);

                        foreach (var error in errors)
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

            var response = _api.Admins.Get(id, null, null);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var admin = _api.Admins.Read(response);

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
                    var response = _api.Admins.Get(model.Email, null, null);

                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound();
                    }

                    var admin = _api.Admins.Read(response);
                    
                    admin.UserName = model.Email;
                    admin.Email = model.Email;
                    admin.Name = model.Name;
                    admin.Surname = model.Surname;
                    admin.PhoneNumber = model.Phone;
                    admin.IsSuperAdmin = model.IsSuperAdmin;

                    var tokenResult = await _tokenManager.GetToken(admin);

                    response = _api.Admins.Put(admin, tokenResult.Token, tokenResult.Provider);

                    if (response.IsSuccessStatusCode)
                    {
                        await _notifier.NotifyUpdate(admin.Email, admin.Email);

                        return RedirectToAction("Admins", "Administration");
                    }
                    else
                    {
                        var errors = _api.Admins.ReadErrors(response);

                        foreach (var error in errors)
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

        public async Task<IActionResult> DeleteAdmin(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var response = _api.Admins.Get(id, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var admin = _api.Admins.Read(response);

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                var result = _api.Admins.Delete(id, tokenResult.Token, tokenResult.Provider);

                if (result.IsSuccessStatusCode)
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
        public IActionResult Doctors(
            string searchString,
            int? specialty,
            int page = 1,
            DoctorSortState sortOrder = DoctorSortState.Id)
        {
            var builder = new DoctorsViewModelBuilder(_api, page, searchString, sortOrder, specialty);
            var director = new ViewModelBuilderDirector();
            director.MakeViewModel(builder);
            var viewModel = builder.GetViewModel();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateDoctor()
        {
            var response = _api.Specialties.Get();

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            ViewBag.Specialties = _api.Specialties
                .ReadMany(response)
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
                    var user = await _userManager.GetUserAsync(User);
                    var tokenResult = await _tokenManager.GetToken(user);

                    var specialty = _api.Specialties.GetOrCreate(model.Specialty, tokenResult.Token, tokenResult.Provider);
                    //var password = _passwordGenerator.GeneratePassword(null);
                    var password = "Pass_1111";

                    var calendarId = await _calendar.CreateCalendar(model.Email);

                    var doctor = new DoctorResourceModel
                    {
                        Name = model.Name,
                        Surname = model.Surname,
                        Email = model.Email,
                        UserName = model.Email,
                        PhoneNumber = model.Phone,
                        SpecialtyId = specialty.SpecialtyId,
                        EmailConfirmed = true,
                        CalendarId = calendarId,
                        Password = password
                    };

                    var response = _api.Doctors.Post(doctor, tokenResult.Token, tokenResult.Provider);

                    if (response.IsSuccessStatusCode)
                    {
                        var entity = _api.Doctors.Read(response);
                        await _notifier.NotifyAdd(entity.Email, entity.Email, password);

                        return RedirectToAction("Doctors", "Administration");
                    }
                    else
                    {
                        var errors = _api.Doctors.ReadErrors(response);

                        foreach (var error in errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                var result = _api.Specialties.Get();
                if (!result.IsSuccessStatusCode)
                {
                    return NotFound();
                }
                ViewBag.Specialties = _api.Specialties
                    .ReadMany(result)
                    .Select(s => s.SpecialtyName)
                    .OrderBy(s => s);

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
            var response = _api.Specialties.Get();
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            ViewBag.Specialties = _api.Specialties.ReadMany(response);
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            response = _api.Doctors.Get(id, null, null);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var doctor = _api.Doctors.Read(response);

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
                    var user = await _userManager.GetUserAsync(User);
                    var tokenResult = await _tokenManager.GetToken(user);

                    var specialty = _api.Specialties.GetOrCreate(model.Specialty, tokenResult.Token, tokenResult.Provider);

                    var response = _api.Doctors.Get(model.Email, null, null);

                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound();
                    }

                    var doctor = _api.Doctors.Read(response);

                    doctor.UserName = model.Email;
                    doctor.Email = model.Email;
                    doctor.Name = model.Name;
                    doctor.Surname = model.Surname;
                    doctor.PhoneNumber = model.Phone;
                    doctor.SpecialtyId = specialty.SpecialtyId;

                    response = _api.Doctors.Put(doctor, tokenResult.Token, tokenResult.Provider);

                    if (response.IsSuccessStatusCode)
                    {
                        await _notifier.NotifyUpdate(doctor.Email, doctor.Email);

                        return RedirectToAction("Doctors", "Administration");
                    }
                    else
                    {
                        var errors = _api.Doctors.ReadErrors(response);

                        foreach (var error in errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                var result = _api.Specialties.Get();
                if (!result.IsSuccessStatusCode)
                {
                    return NotFound();
                }
                ViewBag.Specialties = _api.Specialties
                    .ReadMany(result)
                    .Select(s => s.SpecialtyName)
                    .OrderBy(s => s);

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", err.Message);
            }
        }

        public async Task<IActionResult> DeleteDoctor(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return NotFound();
                }

                var response = _api.Doctors.Get(id, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var doctor = _api.Doctors.Read(response);

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                response = _api.Doctors.Delete(doctor.Id, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    await _calendar.DeleteCalendar(doctor);
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
        public IActionResult Patients(
            string searchString,
            int? locality,
            int page = 1,
            PatientSortState sortOrder = PatientSortState.Id)
        {
            var builder = new PatientsViewModelBuilder(_api, page, searchString, sortOrder, locality);
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

                var response = _api.Patients.Get(id, null, null);
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }
                var patient = _api.Patients.Read(response);

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                response = _api.Patients.Delete(patient.Id, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    await _calendar.DeleteCalendar(patient);
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
            if (_api.Schedules.Get(id, day).IsSuccessStatusCode)
            {
                return RedirectToAction("EditDoctorSchedule", "Administration", new { id = id, day = day });
            }
            else
            {
                return RedirectToAction("AddDoctorSchedule", "Administration", new { id = id, day = day });
            }
        }

        [HttpGet]
        public IActionResult AddDoctorSchedule(string id, string day)
        {
            DoctorSlotViewModel model;

            var response = _api.Doctors.Get(id, null, null);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var doctor = _api.Doctors.Read(response);

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
                var response = _api.Doctors.Get(model.DoctorId, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var doctor = _api.Doctors.Read(response);

                DayOfWeek dayOfWeek;
                Enum.TryParse(model.DayOfWeek, out dayOfWeek);

                var schedule = new Schedule
                {
                    DoctorId = doctor.Id,
                    DayOfWeek = dayOfWeek,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                };

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                _api.Schedules.Post(schedule, tokenResult.Token, tokenResult.Provider);

                return RedirectToAction("DoctorSchedule", "Administration", new { id = model.DoctorId, day = model.DayOfWeek });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditDoctorSchedule(string id, string day)
        {
            var response = _api.Schedules.Get(id, day);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var schedule = _api.Schedules.Read(response);

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
        public async Task<IActionResult> EditDoctorSchedule(DoctorSlotViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ScheduleId == null)
                {
                    return NotFound();
                }
                
                var response = _api.Schedules.Get((int)model.ScheduleId);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var schedule = _api.Schedules.Read(response);

                schedule.StartTime = model.StartTime;
                schedule.EndTime = model.EndTime;

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                _api.Schedules.Put(schedule, tokenResult.Token, tokenResult.Provider);

                return RedirectToAction("DoctorSchedule", "Administration", new { id = model.DoctorId, day = model.DayOfWeek });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctorSchedule(DoctorSlotViewModel model)
        {
            if (model.ScheduleId == null)
            {
                return NotFound();
            }

            var response = _api.Schedules.Get((int)model.ScheduleId);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var schedule = _api.Schedules.Read(response);

            var user = await _userManager.GetUserAsync(User);
            var tokenResult = await _tokenManager.GetToken(user);

            _api.Schedules.Delete(schedule.ScheduleId, tokenResult.Token, tokenResult.Provider);

            return RedirectToAction("Doctors", "Administration");
        }
    }
}
