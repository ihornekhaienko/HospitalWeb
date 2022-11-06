using HospitalWeb.BLL.Services.Interfaces;
using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.Filters.Models;
using HospitalWeb.Filters.Models.FilterModels;
using HospitalWeb.Filters.Models.SortModels;
using HospitalWeb.Filters.Models.ViewModels;
using HospitalWeb.ViewModels.Manage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.Controllers
{
    public class ManageController : Controller
    {
        int _pageSize = 10;
        private readonly ILogger<ManageController> _logger;
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly UnitOfWork _uow;
        private readonly IPasswordGenerator _passwordGenerator;

        public ManageController(
            ILogger<ManageController> logger,
            AppDbContext db,
            UserManager<AppUser> userManager,
            UnitOfWork uow,
            IPasswordGenerator passwordGenerator
            )
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _uow = uow;
            _passwordGenerator = passwordGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> Admins(
            string? searchString,
            int page = 1, 
            AdminSortState sortOrder = AdminSortState.Id)
        {
            ViewBag.CurrentAdmin = _db.Admins.Where(a => a.UserName == User.Identity.Name).FirstOrDefault();

            IQueryable<Admin> admins = _db.Admins;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                admins = admins.Where(a =>
                    a.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    a.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                );
            }

            switch (sortOrder)
            {
                case AdminSortState.NameAsc:
                    admins = admins.OrderBy(a => a.Name);
                    break;
                case AdminSortState.NameDesc:
                    admins = admins.OrderByDescending(a => a.Name);
                    break;
                case AdminSortState.SurnameAsc:
                    admins = admins.OrderBy(a => a.Surname);
                    break;
                case AdminSortState.SurnameDesc:
                    admins = admins.OrderByDescending(a => a.Surname);
                    break;
                case AdminSortState.EmailAsc:
                    admins = admins.OrderBy(a => a.Email);
                    break;
                case AdminSortState.EmailDesc:
                    admins = admins.OrderByDescending(a => a.Email);
                    break;
                case AdminSortState.PhoneAsc:
                    admins = admins.OrderBy(a => a.PhoneNumber);
                    break;
                case AdminSortState.PhoneDesc:
                    admins = admins.OrderByDescending(a => a.PhoneNumber);
                    break;
                case AdminSortState.LevelAsc:
                    admins = admins.OrderBy(a => a.IsSuperAdmin);
                    break;
                case AdminSortState.LevelDesc:
                    admins = admins.OrderByDescending(a => a.IsSuperAdmin);
                    break;
                default:
                    admins = admins.OrderBy(a => a.Id);
                    break;
            }

            var count = await admins.CountAsync();
            var items = await admins
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            var pageModel = new PageModel(count, page, _pageSize);
            var viewModel = new AdminsViewModel
            {
                PageModel = pageModel,
                SortModel = new AdminSortModel(sortOrder),
                FilterModel = new AdminFilterModel(searchString),
                Admins = items
            };
            
            return View(viewModel);
        }

        public async Task<IActionResult> DeleteAdmin(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var admin = await _db.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (admin == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(admin);

            return RedirectToAction("Admins", "Manage");
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
                var admin = new Admin
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.Phone,
                    IsSuperAdmin = model.IsSuperAdmin
                };
                var password = _passwordGenerator.GeneratePassword(null);
                
                var result = await _userManager.CreateAsync(admin, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");

                    return RedirectToAction("Admins", "Manage");
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

        [HttpGet]
        public async Task<IActionResult> EditAdmin(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var admin = await _db.Admins
                .FirstOrDefaultAsync(a => a.Id == id);

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
            if (ModelState.IsValid)
            {
                var admin = await _db.Admins
                    .FirstOrDefaultAsync(a => a.Email == model.Email);

                admin.UserName = model.Email;
                admin.Email = model.Email;
                admin.Name = model.Name;
                admin.Surname = model.Surname;
                admin.PhoneNumber = model.Phone;
                admin.IsSuperAdmin = model.IsSuperAdmin;

                var result = await _userManager.UpdateAsync(admin);

                if (result.Succeeded)
                {
                    return RedirectToAction("Admins", "Manage");
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

        [HttpGet]
        public async Task<IActionResult> Doctors(
            string? searchString,
            int? specialty,
            int page = 1,
            DoctorSortState sortOrder = DoctorSortState.Id)
        {
            IQueryable<Doctor> doctors = _db.Doctors.Include(d => d.Specialty);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                doctors = doctors.Where(d =>
                    d.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    d.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    d.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (specialty != null && specialty != 0)
            {
                doctors = doctors.Where(p => p.Specialty.SpecialtyId == specialty);
            }

            switch (sortOrder)
            {
                case DoctorSortState.NameAsc:
                    doctors = doctors.OrderBy(d => d.Name);
                    break;
                case DoctorSortState.NameDesc:
                    doctors = doctors.OrderByDescending(d => d.Name);
                    break;
                case DoctorSortState.SurnameAsc:
                    doctors = doctors.OrderBy(d => d.Surname);
                    break;
                case DoctorSortState.SurnameDesc:
                    doctors = doctors.OrderByDescending(d => d.Surname);
                    break;
                case DoctorSortState.EmailAsc:
                    doctors = doctors.OrderBy(d => d.Email);
                    break;
                case DoctorSortState.EmailDesc:
                    doctors = doctors.OrderByDescending(d => d.Email);
                    break;
                case DoctorSortState.PhoneAsc:
                    doctors = doctors.OrderBy(d => d.PhoneNumber);
                    break;
                case DoctorSortState.PhoneDesc:
                    doctors = doctors.OrderByDescending(d => d.PhoneNumber);
                    break;
                case DoctorSortState.SpecialtyAsc:
                    doctors = doctors.OrderBy(d => d.Specialty.SpecialtyName);
                    break;
                case DoctorSortState.SpecialtyDesc:
                    doctors = doctors.OrderByDescending(d => d.Specialty.SpecialtyName);
                    break;
                default:
                    doctors = doctors.OrderBy(d => d.Id);
                    break;
            }

            var count = await doctors.CountAsync();
            var items = await doctors
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            var pageModel = new PageModel(count, page, _pageSize);
            var viewModel = new DoctorsViewModel
            {
                PageModel = pageModel,
                SortModel = new DoctorSortModel(sortOrder),
                FilterModel = new DoctorFilterModel(searchString, _uow.Specialties.GetAll().ToList(), specialty),
                Doctors = items
            };

            return View(viewModel);
        }

        public async Task<IActionResult> DeleteDoctor(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var doctor = await _db.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(doctor);

            return RedirectToAction("Doctors", "Manage");
        }

        [HttpGet]
        public IActionResult CreateDoctor()
        {
            ViewBag.Specialties = _uow.Specialties.GetAll().Select(s => s.SpecialtyName);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(DoctorViewModel model)
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
                    Specialty = specialty
                };
                var password = _passwordGenerator.GeneratePassword(null);

                var result = await _userManager.CreateAsync(doctor, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(doctor, "Doctor");

                    return RedirectToAction("Doctors", "Manage");
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

        [HttpGet]
        public async Task<IActionResult> EditDoctor(string id)
        {
            ViewBag.Specialties = _uow.Specialties.GetAll().Select(s => s.SpecialtyName);
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var doctor = await _db.Doctors
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(d => d.Id == id);

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
            if (ModelState.IsValid)
            {
                var specialty = _uow.Specialties.GetOrCreate(model.Specialty);
                var doctor = await _db.Doctors
                    .Include(d => d.Specialty)
                    .FirstOrDefaultAsync(a => a.Email == model.Email);

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
                    return RedirectToAction("Doctors", "Manage");
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

        [HttpGet]
        public async Task<IActionResult> Patients(
            string? searchString,
            int? locality,
            int page = 1,
            PatientSortState sortOrder = PatientSortState.Id)
        {
            IQueryable<Patient> patients = _db.Patients
                .Include(p => p.Address)
                .ThenInclude(a => a.Locality);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                patients = patients.Where(p =>
                    p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    p.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (locality != null && locality != 0)
            {
                patients = patients
                    .Where(p => p.Address.Locality.LocalityId == locality);
            }

            switch (sortOrder)
            {
                case PatientSortState.NameAsc:
                    patients = patients.OrderBy(p => p.Name);
                    break;
                case PatientSortState.NameDesc:
                    patients = patients.OrderByDescending(p => p.Name);
                    break;
                case PatientSortState.SurnameAsc:
                    patients = patients.OrderBy(p => p.Surname);
                    break;
                case PatientSortState.SurnameDesc:
                    patients = patients.OrderByDescending(p => p.Surname);
                    break;
                case PatientSortState.EmailAsc:
                    patients = patients.OrderBy(p => p.Email);
                    break;
                case PatientSortState.EmailDesc:
                    patients = patients.OrderByDescending(p => p.Email);
                    break;
                case PatientSortState.PhoneAsc:
                    patients = patients.OrderBy(p => p.PhoneNumber);
                    break;
                case PatientSortState.PhoneDesc:
                    patients = patients.OrderByDescending(p => p.PhoneNumber);
                    break;
                case PatientSortState.AddressAsc:
                    patients = patients
                        .OrderBy(p => p.Address.FullAddress)
                        .ThenBy(a => a.Address.Locality.LocalityName);
                    break;
                case PatientSortState.AddressDesc:
                    patients = patients
                        .OrderByDescending(p => p.Address.FullAddress)
                        .ThenByDescending(a => a.Address.Locality.LocalityName);
                    break;
                default:
                    patients = patients.OrderBy(d => d.Id);
                    break;
            }

            var count = await patients.CountAsync();
            var items = await patients
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            var pageModel = new PageModel(count, page, _pageSize);
            var viewModel = new PatientsViewModel
            {
                PageModel = pageModel,
                SortModel = new PatientSortModel(sortOrder),
                FilterModel = new PatientFilterModel(searchString, _uow.Localities.GetAll().ToList(), locality),
                Patients = items
            };

            return View(viewModel);
        }

        public async Task<IActionResult> DeletePatient(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var patient = await _db.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(patient);

            return RedirectToAction("Patients", "Manage");
        }
        
        public IActionResult DoctorSchedule(string id, string day)
        {
            if (_uow.Schedules.GetAll().Any(s => s.Doctor.Id == id && s.DayOfWeek.ToString() == day))
            {
                return RedirectToAction("EditDoctorSchedule", "Manage", new { id = id, day = day });
            }
            else
            {
                return RedirectToAction("AddDoctorSchedule", "Manage", new { id = id, day = day });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddDoctorSchedule(string id, string day)
        {
            DoctorSlotViewModel model;

            RedirectToAction("AddDoctorSchedule", "Manage", new { id = id, day = day });
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

                return RedirectToAction("Doctors", "Manage");
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

                var schedule = _uow.Schedules.Get((int)model.ScheduleId);

                schedule.StartTime = model.StartTime;
                schedule.EndTime = model.EndTime;

                _uow.Schedules.Update(schedule);

                return RedirectToAction("Doctors", "Manage");
            }
            return View(model);
        }

        public IActionResult DeleteDoctorSchedule(DoctorSlotViewModel model)
        {
            if (model.ScheduleId == null)
            {
                return NotFound();
            }

            var schedule = _uow.Schedules.Get((int)model.ScheduleId);
            _uow.Schedules.Delete(schedule);

            return RedirectToAction("Doctors", "Manage");
        }
    }
}
