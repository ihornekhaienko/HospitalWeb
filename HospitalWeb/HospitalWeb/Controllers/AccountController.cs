using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAddressService _addressService;

        public AccountController(
            ILogger<AccountController> logger, 
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAddressService addressService
            )
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _addressService = addressService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var address = await _addressService.Create(model.Address);
                Sex sex;
                Enum.TryParse(model.Sex, out sex);

                var patient = new Patient
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    Address = address,
                    BirthDate = model.BirthDate,
                    Sex = sex
                };

                var result = await _userManager.CreateAsync(patient, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(patient, "Patient");
                    await _signInManager.SignInAsync(patient, false);

                    _logger.LogCritical($"{_signInManager.IsSignedIn(User)}");
                    return RedirectToAction("Index", "Home");
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
    }
}
