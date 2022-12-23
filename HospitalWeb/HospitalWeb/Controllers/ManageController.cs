using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Filters.Builders.Implementations;
using HospitalWeb.Mvc.Services.Interfaces;
using HospitalWeb.ViewModels.Error;
using HospitalWeb.ViewModels.Manage;
using HospitalWeb.Clients.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Extensions.Localization;

namespace HospitalWeb.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly ILogger<ManageController> _logger;
        private readonly IStringLocalizer<ExceptionResource> _errLocalizer;
        private readonly IWebHostEnvironment _environment;
        private readonly ApiUnitOfWork _api;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenManager _tokenManager;
        private readonly IFileManager _fileManager;
        private readonly INotifier _notifier;
        private readonly IAuthenticatorKeyService _authenticator;

        public ManageController(
            ILogger<ManageController> logger,
            IStringLocalizer<ExceptionResource> errLocalizer,
            IWebHostEnvironment environment,
            ApiUnitOfWork api,
            UserManager<AppUser> userManager,
            ITokenManager tokenManager,
            IFileManager fileManager,
            INotifier notifier,
            IAuthenticatorKeyService authenticator)
        {
            _logger = logger;
            _errLocalizer = errLocalizer;
            _environment = environment;
            _api = api;
            _userManager = userManager;
            _tokenManager = tokenManager;
            _fileManager = fileManager;
            _notifier = notifier;
            _authenticator = authenticator;
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

                var builder = new NotificationsViewModelBuilder(_api, page, admin.Id, pageSize: 5);
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
                    Notifications = notifications,
                    Is2faEnabled = admin.TwoFactorEnabled
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

                var result = await _userManager.UpdateAsync(admin);

                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "Manage");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

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

                var builder = new NotificationsViewModelBuilder(_api, page, doctor.Id, pageSize: 5);
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
                    Notifications = notifications,
                    Is2faEnabled = doctor.TwoFactorEnabled
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

                var result = await _userManager.UpdateAsync(doctor);

                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "Manage");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

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

                var builder = new NotificationsViewModelBuilder(_api, page, patient.Id, pageSize: 5);
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
                    Notifications = notifications,
                    Is2faEnabled = patient.TwoFactorEnabled
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

                var result = await _userManager.UpdateAsync(patient);

                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "Manage");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    throw new Exception(_errLocalizer["LoadFile"]);
                }

                var bytes = await _fileManager.GetBytes(file);

                if (bytes == null && !bytes.IsImage())
                {
                    throw new Exception(_errLocalizer["NotImage"]);
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

        [HttpGet]
        public async Task<IActionResult> Enable2fa()
        {
            var response = _api.AppUsers.Get(User.Identity.Name, null, null);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.AppUsers.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            var user = _api.AppUsers.Read(response);
            var model = new Enable2faViewModel();
            model.SharedKey = await _authenticator.LoadSharedKey(user);
            model.AuthenticatorUri = _authenticator.LoadQrCodeUri(user, model.SharedKey);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enable2fa(Enable2faViewModel model)
        {
            var response = _api.AppUsers.Get(User.Identity.Name, null, null);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.AppUsers.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            var user = _api.AppUsers.Read(response);

            if (!ModelState.IsValid)
            {
                model.SharedKey = await _authenticator.LoadSharedKey(user);
                model.AuthenticatorUri = _authenticator.LoadQrCodeUri(user, model.SharedKey);

                return View(model);
            }

            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Verification code is invalid.");

                model.SharedKey = await _authenticator.LoadSharedKey(user);
                model.AuthenticatorUri = _authenticator.LoadQrCodeUri(user, model.SharedKey);

                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData["RecoveryCodes"] = recoveryCodes.ToArray();

            return RedirectToAction("RecoveryCodes");
        }

        [HttpGet]
        [Authorize]
        public IActionResult RecoveryCodes()
        {
            var recoveryCodes = TempData["RecoveryCodes"] as string[];

            if (recoveryCodes == null)
            {
                return RedirectToAction("Profile", "Manage");
            }

            return View(recoveryCodes);
        }

        [HttpGet]
        public IActionResult Disable2fa()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Disable2faConfirm()
        {
            var response = _api.AppUsers.Get(User.Identity.Name, null, null);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = response.StatusCode;
                var message = _api.AppUsers.ReadError<string>(response);

                return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
            }

            var user = _api.AppUsers.Read(response);

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction("Profile", "Manage");
        }
    }
}
