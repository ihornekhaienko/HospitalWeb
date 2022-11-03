using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
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
        private readonly ILocalityService _localityService;

        public AccountController(
            ILogger<AccountController> logger, 
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAddressService addressService,
            ILocalityService localityService
            )
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _addressService = addressService;
            _localityService = localityService;
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
                var locality = await _localityService.Create(model.Locality);
                var address = await _addressService.Create(model.Address, locality);
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
        
        [HttpGet]    
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                _logger.LogCritical(_signInManager.IsSignedIn(User).ToString());

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ViewBag.ReturnUrl) && Url.IsLocalUrl(ViewBag.ReturnUrl))
                    {
                        return Redirect(ViewBag.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Wrong email or password");
                    return View(model);
                }
            }

            return View(model);
        }
    }
}
