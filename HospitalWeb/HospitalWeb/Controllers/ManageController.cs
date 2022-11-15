using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Services.Extensions;
using HospitalWeb.Services.Interfaces;
using HospitalWeb.ViewModels.Error;
using HospitalWeb.ViewModels.Manage;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly UnitOfWork _uow;
        private readonly IFileManager _fileManager;
        private readonly INotifier _notifier;

        public ManageController(
            ILogger<ManageController> logger, 
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager,
            UnitOfWork uow,
            IFileManager fileManager,
            INotifier notifier)
        {
            _logger = logger;
            _environment = environment;
            _userManager = userManager;
            _uow = uow;
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
            var admin = _uow.Admins.Get(a => a.Email == User.Identity.Name);

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
        public async Task<IActionResult> AdminProfile(AdminProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _uow.Admins.Get(a => a.Email == User.Identity.Name);

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

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> DoctorProfile()
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));
            var doctor = _uow.Doctors.Get(d => d.Email == User.Identity.Name);

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
        public async Task<IActionResult> DoctorProfile(DoctorProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doctor = _uow.Doctors.Get(d => d.Email == User.Identity.Name);

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

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> PatientProfile()
        {
            ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));
            var patient = _uow.Patients.Get(p => p.Email == User.Identity.Name);

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
        public async Task<IActionResult> PatientProfile(PatientProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var locality = _uow.Localities.GetOrCreate(model.Locality);
                var address = _uow.Addresses.GetOrCreate(model.Address, locality);
                var patient = _uow.Patients.Get(d => d.Email == User.Identity.Name);

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
                        var user = await _userManager.FindByEmailAsync(User.Identity.Name);

                        user.Image = bytes;
                        var result = await _userManager.UpdateAsync(user);

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
                    var user = await _userManager.FindByEmailAsync(User.Identity.Name);
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
