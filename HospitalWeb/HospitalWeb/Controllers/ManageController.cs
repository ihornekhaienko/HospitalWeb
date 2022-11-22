using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Services.Extensions;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Error;
using HospitalWeb.ViewModels.Manage;
using HospitalWeb.WebApi.Clients.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;
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
        private readonly IFileManager _fileManager;
        private readonly INotifier _notifier;

        public ManageController(
            ILogger<ManageController> logger, 
            IWebHostEnvironment environment,
            ApiUnitOfWork api,
            UserManager<AppUser> userManager,
            IFileManager fileManager,
            INotifier notifier)
        {
            _logger = logger;
            _environment = environment;
            _api = api;
            _userManager = userManager;
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

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminProfile()
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

            var response = _api.Admins.Get(User.Identity.Name);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var admin = _api.Admins.Read(response);

            var model = new AdminProfileViewModel
            {
                Name = admin.Name,
                Surname = admin.Surname,
                Email = admin.Email,
                Phone = admin.PhoneNumber,
                Image = admin.Image,
                IsSuperAdmin = admin.IsSuperAdmin
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AdminProfile(AdminProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _api.Admins.Get(User.Identity.Name);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var admin = _api.Admins.Read(response);

                admin.UserName = model.Email;
                admin.Email = model.Email;
                admin.Surname = model.Surname;
                admin.Name = model.Name;
                admin.PhoneNumber = model.Phone;

                response = _api.Admins.Put(admin);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile", "Manage");
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

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> DoctorProfile()
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

            var response = _api.Doctors.Get(User.Identity.Name);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var doctor = _api.Doctors.Read(response);

            var model = new DoctorProfileViewModel
            {
                Name = doctor.Name,
                Surname = doctor.Surname,
                Email = doctor.Email,
                Phone = doctor.PhoneNumber,
                Image = doctor.Image,
                Specialty = doctor.Specialty.SpecialtyName
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult DoctorProfile(DoctorProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _api.Doctors.Get(User.Identity.Name);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var doctor = _api.Doctors.Read(response);

                doctor.UserName = model.Email;
                doctor.Email = model.Email;
                doctor.Surname = model.Surname;
                doctor.Name = model.Name;
                doctor.PhoneNumber = model.Phone;

                response = _api.Doctors.Put(doctor);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile", "Manage");
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

            return View(model);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> PatientProfile()
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

            var response = _api.Patients.Get(User.Identity.Name);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var patient = _api.Patients.Read(response);

            var model = new PatientProfileViewModel
            {
                Name = patient.Name,
                Surname = patient.Surname,
                Email = patient.Email,
                Phone = patient.PhoneNumber,
                Image = patient.Image,
                Address = patient.Address.FullAddress,
                Locality = patient.Address.Locality.LocalityName
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult PatientProfile(PatientProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var locality = _api.Localities.GetOrCreate(model.Locality);
                var address = _api.Addresses.GetOrCreate(model.Address, locality);

                var response = _api.Patients.Get(User.Identity.Name);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var patient = _api.Patients.Read(response);

                patient.UserName = model.Email;
                patient.Email = model.Email;
                patient.Surname = model.Surname;
                patient.Name = model.Name;
                patient.PhoneNumber = model.Phone;
                patient.Address = address;

                response = _api.Patients.Put(patient);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile", "Manage");
                }
                else
                {
                    var errors = _api.Patients.ReadErrors(response);

                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    var bytes = await _fileManager.GetBytes(file);

                    if (bytes != null && bytes.IsImage())
                    {
                        var response = _api.AppUsers.Get(User.Identity.Name);

                        if (!response.IsSuccessStatusCode)
                        {
                            return NotFound();
                        }

                        var user = _api.AppUsers.Read(response);

                        user.Image = bytes;
                        
                        _api.AppUsers.Put(user);

                        return RedirectToAction("Profile", "Manage");
                    }
                    else
                    {
                        throw new Exception("Your file is not an image");
                    }
                }
                else
                {
                    throw new Exception("Failed loading file");
                }
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
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
                    var response = _api.AppUsers.Get(User.Identity.Name);
                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound();
                    }
                    var user = _api.AppUsers.Read(response);

                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        await _notifier.NotifyUpdate(user.Email, user.Email);
                        return RedirectToAction("Profile", "Manage");
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
                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
    }
}
