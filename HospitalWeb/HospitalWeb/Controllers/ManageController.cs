using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Services.Extensions;
using HospitalWeb.Services.Interfaces;
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

        public ManageController(
            ILogger<ManageController> logger, 
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager,
            UnitOfWork uow,
            IFileManager fileManager)
        {
            _logger = logger;
            _environment = environment;
            _userManager = userManager;
            _uow = uow;
            _fileManager = fileManager;
        }

        [Authorize]
        public IActionResult Profile()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminProfile", "Manage");
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("DoctorProfile", "Manage");
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("PatientProfile", "Manage");
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminProfile()
        {
            var admin = _uow.Admins.Get(a => a.Email == User.Identity.Name);

            byte[] image;

            if (admin.Image != null)
            {
                image = admin.Image;
            }
            else
            {
                image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));
            }

            var model = new AdminProfileViewModel
            {
                Name = admin.Name,
                Surname = admin.Surname,
                Email = admin.Email,
                Phone = admin.PhoneNumber,
                Image = image,
                IsSuperAdmin = admin.IsSuperAdmin
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AdminProfile(AdminProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _uow.Admins.Get(a => a.Email == User.Identity.Name);

                admin.Email = model.Email;
                admin.Surname = model.Surname;
                admin.Name = model.Name;
                admin.PhoneNumber = model.Phone;

                _uow.Admins.Update(admin);

                return RedirectToAction("Profile", "Manage");
            }

            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> DoctorProfile()
        {
            var doctor = _uow.Doctors.Get(d => d.Email == User.Identity.Name);

            byte[] image;

            if (doctor.Image != null)
            {
                image = doctor.Image;
            }
            else
            {
                image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));
            }

            var model = new DoctorProfileViewModel
            {
                Name = doctor.Name,
                Surname = doctor.Surname,
                Email = doctor.Email,
                Phone = doctor.PhoneNumber,
                Image = image,
                Specialty = doctor.Specialty.SpecialtyName
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult DoctorProfile(DoctorProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doctor = _uow.Doctors.Get(d => d.Email == User.Identity.Name);

                doctor.Email = model.Email;
                doctor.Surname = model.Surname;
                doctor.Name = model.Name;
                doctor.PhoneNumber = model.Phone;

                _uow.Doctors.Update(doctor);

                return RedirectToAction("Profile", "Manage");
            }

            return View(model);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> PatientProfile()
        {
            var patient = _uow.Patients.Get(p => p.Email == User.Identity.Name);

            byte[] image;

            if (patient.Image != null)
            {
                image = patient.Image;
            }
            else
            {
                image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));
            }

            var model = new PatientProfileViewModel
            {
                Name = patient.Name,
                Surname = patient.Surname,
                Email = patient.Email,
                Phone = patient.PhoneNumber,
                Image = image,
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
                var locality = _uow.Localities.GetOrCreate(model.Locality);
                var address = _uow.Addresses.GetOrCreate(model.Address, locality);
                var patient = _uow.Patients.Get(d => d.Email == User.Identity.Name);

                patient.Email = model.Email;
                patient.Surname = model.Surname;
                patient.Name = model.Name;
                patient.PhoneNumber = model.Phone;
                patient.Address = address;

                _uow.Patients.Update(patient);

                return RedirectToAction("Profile", "Manage");
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
                }

                return NotFound();
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.Message);
                return Ok();
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
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "Manage");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Wrong password");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            //    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            //    if (result.Succeeded)
            //    {
            //        return RedirectToAction("Profile", "Account");
            //    }
            //    else
            //    {
            //        ModelState.AddModelError(string.Empty, "Wrong password");
            //        return View(model);
            //    }
            //}

            //return View(model);
            throw new NotImplementedException();
        }
    }
}
