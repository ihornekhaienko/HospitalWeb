﻿using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Services.Extensions;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Error;
using HospitalWeb.ViewModels.Manage;
using HospitalWeb.WebApi.Clients.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly ILogger<ManageController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ApiUnitOfWork _api;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenManager _tokenManager;
        private readonly IFileManager _fileManager;
        private readonly INotifier _notifier;

        public ManageController(
            ILogger<ManageController> logger, 
            IWebHostEnvironment environment,
            ApiUnitOfWork api,
            UserManager<AppUser> userManager,
            ITokenManager tokenManager,
            IFileManager fileManager,
            INotifier notifier)
        {
            _logger = logger;
            _environment = environment;
            _api = api;
            _userManager = userManager;
            _tokenManager = tokenManager;
            _fileManager = fileManager;
            _notifier = notifier;
        }

        [Authorize]
        public IActionResult Profile()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminProfile", "Manage");
            }
            else if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("DoctorProfile", "Manage");
            }
            else if (User.IsInRole("Patient"))
            {
                return RedirectToAction("PatientProfile", "Manage");
            }

            return RedirectToAction("NotFound", "Error");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminProfile(int page = 1)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

                var response = _api.Admins.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Admins.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var admin = _api.Admins.Read(response);

                var builder = new NotificationsViewModelBuilder(_api, page, admin.Id, 5);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var notifications = builder.GetViewModel();

                var model = new AdminProfileViewModel
                {
                    Name = admin.Name,
                    Surname = admin.Surname,
                    Email = admin.Email,
                    Phone = admin.PhoneNumber,
                    Image = admin.Image,
                    IsSuperAdmin = admin.IsSuperAdmin,
                    Notifications = notifications
                };

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in ManageController.AdminProfile.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AdminProfile(AdminProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _api.Admins.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Admins.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var admin = _api.Admins.Read(response);

                admin.UserName = model.Email;
                admin.Email = model.Email;
                admin.Surname = model.Surname;
                admin.Name = model.Name;
                admin.PhoneNumber = model.Phone;

                var tokenResult = await _tokenManager.GetToken(admin);

                response = _api.Admins.Put(admin, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile", "Manage");
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

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> DoctorProfile(int page = 1)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

                var response = _api.Doctors.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var doctor = _api.Doctors.Read(response);

                var builder = new NotificationsViewModelBuilder(_api, page, doctor.Id, 5);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var notifications = builder.GetViewModel();

                var model = new DoctorProfileViewModel
                {
                    Name = doctor.Name,
                    Surname = doctor.Surname,
                    Email = doctor.Email,
                    Phone = doctor.PhoneNumber,
                    Image = doctor.Image,
                    Specialty = doctor.Specialty.SpecialtyName,
                    Notifications = notifications
                };

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in ManageController.DoctorProfile.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DoctorProfile(DoctorProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _api.Doctors.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Doctors.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var doctor = _api.Doctors.Read(response);

                doctor.UserName = model.Email;
                doctor.Email = model.Email;
                doctor.Surname = model.Surname;
                doctor.Name = model.Name;
                doctor.PhoneNumber = model.Phone;

                var tokenResult = await _tokenManager.GetToken(doctor);

                response = _api.Doctors.Put(doctor, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile", "Manage");
                }

                var errors = _api.Doctors.ReadErrors(response);

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

            return View(model);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> PatientProfile(int page = 1)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

                var response = _api.Patients.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Patients.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var patient = _api.Patients.Read(response);

                var builder = new NotificationsViewModelBuilder(_api, page, patient.Id, 5);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var notifications = builder.GetViewModel();

                var model = new PatientProfileViewModel
                {
                    Name = patient.Name,
                    Surname = patient.Surname,
                    Email = patient.Email,
                    Phone = patient.PhoneNumber,
                    Image = patient.Image,
                    Address = patient.Address.FullAddress,
                    Locality = patient.Address.Locality.LocalityName,
                    Notifications = notifications
                };

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in ManageController.PatientProfile.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PatientProfile(PatientProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var locality = _api.Localities.GetOrCreate(model.Locality);
                var address = _api.Addresses.GetOrCreate(model.Address, locality);

                var response = _api.Patients.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Patients.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var patient = _api.Patients.Read(response);

                patient.UserName = model.Email;
                patient.Email = model.Email;
                patient.Surname = model.Surname;
                patient.Name = model.Name;
                patient.PhoneNumber = model.Phone;
                patient.Address = address;

                var tokenResult = await _tokenManager.GetToken(patient);

                response = _api.Patients.Put(patient, tokenResult.Token, tokenResult.Provider);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile", "Manage");
                }

                var errors = _api.Patients.ReadError<IEnumerable<IdentityError>>(response);

                if (errors == null)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Patients.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    throw new Exception("Failed loading file");
                }

                var bytes = await _fileManager.GetBytes(file);

                if (bytes == null && !bytes.IsImage())
                {
                    throw new Exception("Your file is not an image");
                }

                var response = _api.AppUsers.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.AppUsers.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var user = _api.AppUsers.Read(response);

                user.Image = bytes;

                var tokenResult = await _tokenManager.GetToken(user);

                _api.AppUsers.Put(user, tokenResult.Token, tokenResult.Provider);

                return RedirectToAction("Profile", "Manage");
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in ManageController.UploadPhoto.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _api.AppUsers.Get(User.Identity.Name, null, null);
                    if (!response.IsSuccessStatusCode)
                    {
                        var statusCode = response.StatusCode;
                        var message = _api.AppUsers.ReadError<string>(response);

                        return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                    }
                    var user = _api.AppUsers.Read(response);

                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        await _notifier.NotifyUpdate(user.Email, user.Email);
                        return RedirectToAction("Profile", "Manage");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
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

        public async Task<IActionResult> ReadNotification(int id)
        {
            var response = _api.Notifications.Get(id);
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.Notifications.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }
            var notification = _api.Notifications.Read(response);

            notification.IsRead = true;
            notification.Type = NotificationType.Secondary;

            var user = await _userManager.GetUserAsync(User);
            var tokenResult = await _tokenManager.GetToken(user);

            _api.Notifications.Put(notification, tokenResult.Token, tokenResult.Provider);

            return Ok();
        }
    }
}
