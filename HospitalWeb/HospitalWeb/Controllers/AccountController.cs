using HospitalWeb.Services.Interfaces;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HospitalWeb.ViewModels.Error;

namespace HospitalWeb.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UnitOfWork _uow;
        private readonly IFileManager _fileManager;
        private readonly INotifier _notifier;

        public AccountController(
            ILogger<AccountController> logger,
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            UnitOfWork uow,
            IFileManager fileManager,
            INotifier notifier
            )
        {
            _logger = logger;
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
            _uow = uow;
            _fileManager = fileManager;
            _notifier = notifier;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var locality = _uow.Localities.GetOrCreate(model.Locality);
                var address = _uow.Addresses.GetOrCreate(model.Address, locality);
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
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(patient);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = patient.Id, code = code },
                        protocol: HttpContext.Request.Scheme
                        );
                    await _notifier.SendConfirmationLink(model.Email, callbackUrl);

                    await _userManager.AddToRoleAsync(patient, "Patient");
                    await _signInManager.SignInAsync(patient, false);

                    return RedirectToLocal(returnUrl);
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
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
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
        public async Task<IActionResult> Logout(string returnUrl = null)
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                if (userId == null || code == null)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{userId}'.");
                }
                var result = await _userManager.ConfirmEmailAsync(user, code);

                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Failed email confirmation");
                }

                return View("ConfirmEmail", "Account");
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
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
