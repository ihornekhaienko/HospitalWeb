using HospitalWeb.Services.Interfaces;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HospitalWeb.ViewModels.Error;
using System.Security.Claims;
using HospitalWeb.WebApi.Clients.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;

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
        private readonly ApiUnitOfWork _api;
        private readonly IFileManager _fileManager;
        private readonly INotifier _notifier;

        public AccountController(
            ILogger<AccountController> logger,
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ApiUnitOfWork api,
            IFileManager fileManager,
            INotifier notifier
            )
        {
            _logger = logger;
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
            _api = api;
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
                var locality = _api.Localities.GetOrCreate(model.Locality);
                var address = _api.Addresses.GetOrCreate(model.Address, locality);
                Sex sex;
                Enum.TryParse(model.Sex, out sex);

                var resource = new PatientResourceModel
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    AddressId = address.AddressId,
                    BirthDate = model.BirthDate,
                    Sex = sex,
                    Password = model.Password
                };

                var response = _api.Patients.Post(resource);

                if (response.IsSuccessStatusCode)
                {
                    var patient = _api.Patients.Read(response);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(patient);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = patient.Id, code = code },
                        protocol: HttpContext.Request.Scheme
                        );
                    await _notifier.SendConfirmationLink(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(patient, false);

                    return RedirectToLocal(returnUrl);
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
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                    ModelState.AddModelError(string.Empty, "Wrong email or password");

                    return View(model);
                }
            }

            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                    new { returnUrl });

            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = $"Error from external provider: {remoteError}" });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _signInManager
                .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);

            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = info.LoginProvider;
                var model = new ExternalLoginViewModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                };

                return View("ExternalLogin", model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirm(ExternalLoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var info = await _signInManager.GetExternalLoginInfoAsync();

                    if (info == null)
                    {
                        throw new ApplicationException("Error loading external login information during confirmation.");
                    }

                    var locality = _api.Localities.GetOrCreate(model.Locality);
                    var address = _api.Addresses.GetOrCreate(model.Address, locality);
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

                    var response = _api.Patients.Post(patient);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await _userManager.AddLoginAsync(patient, info);

                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(patient, false);

                            return RedirectToLocal(returnUrl);
                        }
                    }
                }

                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLogin", model);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
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

                var response = _api.AppUsers.Get(userId);
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }
                var user = _api.AppUsers.Read(response);

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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _api.AppUsers.Get(model.Email);
                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound();
                    }
                    var user = _api.AppUsers.Read(response);

                    if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        return RedirectToAction("ForgotPasswordConfirmation");
                    }

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme
                        );
                    await _notifier.SendResetPasswordLink(model.Email, callbackUrl);

                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            try
            {
                if (code == null)
                {
                    throw new ApplicationException("A code must be supplied for password reset.");
                }

                var model = new ResetPasswordViewModel { Code = code };

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogCritical(err.StackTrace);
                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return RedirectToAction("ResetPasswordConfirm", "Account");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirm", "Account");
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
        public IActionResult ResetPasswordConfirm()
        {
            return View();
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
