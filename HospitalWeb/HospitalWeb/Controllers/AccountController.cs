using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
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
        private readonly UnitOfWork _uof;

        public AccountController(
            ILogger<AccountController> logger, 
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            UnitOfWork uof
            )
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _uof = uof;
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
                var locality = _uof.Localities.GetOrCreate(model.Locality);
                var address = _uof.Addresses.GetOrCreate(model.Address, locality);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _signInManager.SignOutAsync();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
