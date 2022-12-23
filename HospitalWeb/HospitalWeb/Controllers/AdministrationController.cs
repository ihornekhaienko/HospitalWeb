using HospitalWeb.Services.Interfaces;
using HospitalWeb.Domain.Entities;
using HospitalWeb.ViewModels.Administration;
using Microsoft.AspNetCore.Mvc;
using HospitalWeb.Filters.Builders.Implementations;
using Microsoft.AspNetCore.Authorization;
using HospitalWeb.Models.SortStates;
using HospitalWeb.Clients.Implementations;
using HospitalWeb.Models.ResourceModels;
using HospitalWeb.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using HospitalWeb.Services.Extensions;
using HospitalWeb.ViewModels.Error;
using Newtonsoft.Json;
using Microsoft.Extensions.Localization;

namespace HospitalWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly ILogger<AdministrationController> _logger;
        private readonly IStringLocalizer<ExceptionResource> _errLocalizer;
        private readonly IWebHostEnvironment _environment;
        private readonly ApiUnitOfWork _api;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenManager _tokenManager;
        private readonly ICalendarService _calendar;
        private readonly IFileManager _fileManager;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly INotifier _notifier;

        public AdministrationController(
            ILogger<AdministrationController> logger,
            IStringLocalizer<ExceptionResource> errLocalizer,
            IWebHostEnvironment environment,
            ApiUnitOfWork api,
            UserManager<AppUser> userManager,
            ITokenManager tokenManager,
            ICalendarService calendar,
            IFileManager fileManager,
            IPasswordGenerator passwordGenerator,
            INotifier notifier)
        {
            _logger = logger;
            _errLocalizer = errLocalizer;
            _environment = environment;
            _api = api;
            _fileManager = fileManager;
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
            try
            {
                var response = _api.Admins.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Admins.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message } );
                }

                ViewBag.CurrentAdmin = _api.Admins.Read(response);

                var builder = new AdminsViewModelBuilder(_api, page, searchString, sortOrder);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var viewModel = builder.GetViewModel();

                return View(viewModel);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdministrationController.Admins.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
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

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                var response = _api.Admins.Post(admin, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    await _notifier.NotifyAdd(admin.Email, admin.Email, password);

                    return RedirectToAction("Admins", "Administration");
                }

                var errors = _api.Admins.ReadError<IEnumerable<IdentityError>>(response);

                if (errors == null)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Admins.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EditAdmin(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("NotFound", "Error", new ErrorViewModel { Message = "Admin wasn't found" });
            }

            var response = _api.Admins.Get(id, null, null);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Admins.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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
            if (ModelState.IsValid)
            {
                var response = _api.Admins.Get(model.Email, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Admins.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var admin = _api.Admins.Read(response);

                admin.UserName = model.Email;
                admin.Email = model.Email;
                admin.Name = model.Name;
                admin.Surname = model.Surname;
                admin.PhoneNumber = model.Phone;
                admin.IsSuperAdmin = model.IsSuperAdmin;

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                response = _api.Admins.Put(admin, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    await _notifier.NotifyUpdate(admin.Email, admin.Email);

                    return RedirectToAction("Admins", "Administration");
                }

                var errors = _api.Admins.ReadError<IEnumerable<IdentityError>>(response);

                if (errors == null)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Admins.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteAdmin(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("NotFound", "Error", new ErrorViewModel { Message = "Admin wasn't found" });
            }

            var response = _api.Admins.Get(id, null, null);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Admins.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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

        [HttpGet]
        public IActionResult Hospitals(
            string searchString,
            int? locality,
            int page = 1,
            HospitalSortState sortOrder = HospitalSortState.Id)
        {
            try
            {
                var builder = new HospitalsViewModelBuilder(_api, page, searchString, sortOrder, locality);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var viewModel = builder.GetViewModel();

                return View(viewModel);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdministrationController.Hospitals.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpGet]
        public IActionResult CreateHospital()
        {
            var response = _api.Localities.Get();

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Localities.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            ViewBag.Localities = _api.Localities
                .ReadMany(response)
                .Select(l => l.LocalityName)
                .OrderBy(l => l);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateHospital(HospitalViewModel model)
        {
            if (ModelState.IsValid)
            {
                var locality = _api.Localities.GetOrCreate(model.Locality);
                var address = _api.Addresses.GetOrCreate(model.Address, locality);

                var hospital = new HospitalResourceModel
                {
                    HospitalName = model.Name,
                    Type = model.Type,
                    AddressId = address.AddressId
                };

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                var response = _api.Hospitals.Post(hospital, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Hospitals", "Administration");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditHospital(int id)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/hospital-icon.png"));

                if (id < 0)
                {
                    return RedirectToAction("NotFound", "Error", new ErrorViewModel { Message = "Hospital wasn't found" });
                }

                var response = _api.Hospitals.Get(id, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Hospitals.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var hospital = _api.Hospitals.Read(response);

                response = _api.Localities.Get();

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Localities.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                ViewBag.Localities = _api.Localities
                    .ReadMany(response)
                    .Select(l => l.LocalityName)
                    .OrderBy(l => l);

                var model = new HospitalViewModel
                {
                    Id = hospital.HospitalId,
                    Name = hospital.HospitalName,
                    Address = hospital.Address.FullAddress,
                    Locality = hospital.Address.Locality.LocalityName,
                    Image = hospital.Image
                };

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdministrationController.EditHospital.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditHospital(HospitalViewModel model)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/hospital-icon.png"));

                if (ModelState.IsValid)
                {
                    var response = _api.Hospitals.Get(model.Id, null, null);

                    if (!response.IsSuccessStatusCode)
                    {
                        var statusCode = response.StatusCode;
                        var message = _api.Hospitals.ReadError<string>(response);

                        return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                    }

                    var hospital = _api.Hospitals.Read(response);

                    var locality = _api.Localities.GetOrCreate(model.Locality);
                    var address = _api.Addresses.GetOrCreate(model.Address, locality);

                    hospital.HospitalName = model.Name;
                    hospital.AddressId = address.AddressId;

                    var user = await _userManager.GetUserAsync(User);
                    var tokenResult = await _tokenManager.GetToken(user);

                    response = _api.Hospitals.Put(hospital, tokenResult.Token, tokenResult.Provider);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Hospitals", "Administration");
                    }
                }

                var result = _api.Localities.Get();

                if (!result.IsSuccessStatusCode)
                {
                    var statusCode = result.StatusCode;
                    var message = _api.Localities.ReadError<string>(result);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                ViewBag.Localities = _api.Localities
                    .ReadMany(result)
                    .Select(l => l.LocalityName)
                    .OrderBy(l => l);

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdministrationController.EditHospital.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile file, int id)
        {
            try
            {
                if (file == null)
                {
                    throw new Exception(_errLocalizer["LoadFile"]);
                }

                var bytes = await _fileManager.GetBytes(file);

                if (bytes != null && bytes.IsImage())
                {
                    throw new Exception(_errLocalizer["NotImage"]);
                }

                var response = _api.Hospitals.Get(id, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Hospitals.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var hospital = _api.Hospitals.Read(response);

                hospital.Image = bytes;

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                _api.Hospitals.Put(hospital, tokenResult.Token, tokenResult.Provider);

                return RedirectToAction("EditHospital", "Administration", new { id = id });
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdministrationController.UploadPhoto.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });

            }
        }

        public async Task<IActionResult> DeleteHospital(int id)
        {
            if (id < 0)
            {
                return RedirectToAction("NotFound", "Error", new ErrorViewModel { Message = "Hospital wasn't found" });
            }

            var response = _api.Hospitals.Get(id, null, null);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Hospitals.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            var hospital = _api.Hospitals.Read(response);

            var user = await _userManager.GetUserAsync(User);
            var tokenResult = await _tokenManager.GetToken(user);

            var result = _api.Hospitals.Delete(hospital.HospitalId, tokenResult.Token, tokenResult.Provider);

            if (!result.IsSuccessStatusCode)
            {
                var statusCode = result.StatusCode;
                var message = _api.Hospitals.ReadError<string>(result);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            return RedirectToAction("Hospitals", "Administration");
        }

        [HttpGet]
        public IActionResult Doctors(
            string searchString,
            int? specialty,
            int? hospital,
            int? locality,
            int page = 1,
            DoctorSortState sortOrder = DoctorSortState.Id)
        {
            try
            {
                var builder = new DoctorsViewModelBuilder(_api, page, searchString, sortOrder, specialty, hospital, locality);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var viewModel = builder.GetViewModel();

                return View(viewModel);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdministrationController.Doctors.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpGet]
        public IActionResult CreateDoctor()
        {
            var response = _api.Specialties.Get();

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Specialties.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            ViewBag.Specialties = _api.Specialties
                .ReadMany(response)
                .Select(s => s.SpecialtyName)
                .OrderBy(s => s);

            response = _api.Hospitals.Get();

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Hospitals.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            var hospitals = _api.Hospitals.ReadMany(response);
            ViewBag.Hospitals = hospitals
                .Select(h => new { HospitalId = h.HospitalId, HospitalName = h.HospitalName })
                .OrderBy(h => h.HospitalName);
            ViewBag.PrivateHospitals = JsonConvert.SerializeObject(hospitals
                .Where(h => h.Type == HospitalType.Private)
                .Select(h => h.HospitalId)
                .ToList());

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(DoctorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                var specialty = _api.Specialties.GetOrCreate(model.Specialty, tokenResult.Token, tokenResult.Provider);
                //var password = _passwordGenerator.GeneratePassword(null);
                var password = "Pass_1111";

                var doctor = new DoctorResourceModel
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.Phone,
                    SpecialtyId = specialty.SpecialtyId,
                    HospitalId = model.Hospital,
                    EmailConfirmed = true,
                    Password = password,
                    ServicePrice = model.ServicePrice,
                };

                try
                {
                    var calendarId = await _calendar.CreateCalendar(model.Email);
                    doctor.CalendarId = calendarId;
                }
                catch (Exception err)
                {
                    _logger.LogError($"Google calendar error: {err.Message}");
                    _logger.LogError($"Unable to create calendar for {model.Email}");
                }

                var response = _api.Doctors.Post(doctor, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    var entity = _api.Doctors.Read(response);
                    await _notifier.NotifyAdd(entity.Email, entity.Email, password);

                    return RedirectToAction("Doctors", "Administration");
                }

                var errors = _api.Doctors.ReadError<IEnumerable<IdentityError>>(response);

                if (errors == null)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Doctors.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var result = _api.Specialties.Get();
            ViewBag.Specialties = _api.Specialties
                .ReadMany(result)
                .Select(s => s.SpecialtyName)
                .OrderBy(s => s);

            result = _api.Hospitals.Get();
            var hospitals = _api.Hospitals.ReadMany(result);
            ViewBag.Hospitals = hospitals
                .Select(h => new { HospitalId = h.HospitalId, HospitalName = h.HospitalName })
                .OrderBy(h => h.HospitalName);
            ViewBag.PrivateHospitals = hospitals
                .Where(h => h.Type == HospitalType.Private)
                .Select(h => h.HospitalId)
                .ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult EditDoctor(string id)
        {
            var response = _api.Specialties.Get();
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Specialties.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            ViewBag.Specialties = _api.Specialties
                    .ReadMany(response)
                    .Select(s => s.SpecialtyName)
                    .OrderBy(s => s);

            response = _api.Hospitals.Get();

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Hospitals.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            ViewBag.Hospitals = _api.Hospitals
                .ReadMany(response)
                .Select(h => new { HospitalId = h.HospitalId, HospitalName = h.HospitalName })
                .OrderBy(h => h.HospitalName);

            response = _api.Doctors.Get(id, null, null);
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Doctors.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }
            var doctor = _api.Doctors.Read(response);

            var model = new DoctorViewModel
            {
                Name = doctor.Name,
                Surname = doctor.Surname,
                Email = doctor.Email,
                Phone = doctor.PhoneNumber,
                Specialty = doctor.Specialty.SpecialtyName,
                Hospital = doctor.HospitalId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctor(DoctorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                var specialty = _api.Specialties.GetOrCreate(model.Specialty, tokenResult.Token, tokenResult.Provider);

                var response = _api.Doctors.Get(model.Email, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Doctors.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var doctor = _api.Doctors.Read(response);

                doctor.UserName = model.Email;
                doctor.Email = model.Email;
                doctor.Name = model.Name;
                doctor.Surname = model.Surname;
                doctor.PhoneNumber = model.Phone;
                doctor.SpecialtyId = specialty.SpecialtyId;
                doctor.HospitalId = model.Hospital;

                response = _api.Doctors.Put(doctor, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    await _notifier.NotifyUpdate(doctor.Email, doctor.Email);

                    return RedirectToAction("Doctors", "Administration");
                }

                var errors = _api.Doctors.ReadError<IEnumerable<IdentityError>>(response);

                if (errors == null)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Doctors.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var result = _api.Specialties.Get();
            ViewBag.Specialties = _api.Specialties
                .ReadMany(result)
                .Select(s => s.SpecialtyName)
                .OrderBy(s => s);

            result = _api.Hospitals.Get();
            ViewBag.Hospitals = _api.Hospitals
                .ReadMany(result)
                .Select(h => new { HospitalId = h.HospitalId, HospitalName = h.HospitalName })
                .OrderBy(h => h.HospitalName);

            return View(model);
        }

        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var response = _api.Doctors.Get(id, null, null);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Doctors.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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

        [HttpGet]
        public IActionResult Patients(
            string searchString,
            int? locality,
            int page = 1,
            PatientSortState sortOrder = PatientSortState.Id)
        {
            try
            {
                var builder = new PatientsViewModelBuilder(_api, page, searchString, sortOrder, locality);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var viewModel = builder.GetViewModel();

                return View(viewModel);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdministrationController.Patients.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        public async Task<IActionResult> DeletePatient(string id)
        {
            var response = _api.Patients.Get(id, null, null);
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Patients.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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
                var statusCode = response.StatusCode;
                var message = _api.Doctors.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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
                    var statusCode = response.StatusCode;
                    var message = _api.Doctors.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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
                var statusCode = response.StatusCode;
                var message = _api.Schedules.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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
                var response = _api.Schedules.Get((int)model.ScheduleId);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Schedules.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
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
            var response = _api.Schedules.Get((int)model.ScheduleId);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Schedules.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            var schedule = _api.Schedules.Read(response);

            var user = await _userManager.GetUserAsync(User);
            var tokenResult = await _tokenManager.GetToken(user);

            _api.Schedules.Delete(schedule.ScheduleId, tokenResult.Token, tokenResult.Provider);

            return RedirectToAction("Doctors", "Administration");
        }
    }
}
