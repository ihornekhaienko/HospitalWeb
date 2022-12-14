using HospitalWeb.Mvc.Services.Interfaces;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HospitalWeb.Mvc.ViewModels.Error;
using System.Security.Claims;
using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Mvc.Models.ResourceModels;
using Microsoft.Extensions.Localization;

namespace HospitalWeb.Mvc.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IStringLocalizer<ExceptionResource> _errLocalizer;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApiUnitOfWork _api;
        private readonly ICalendarService _calendar;
        private readonly IFileManager _fileManager;
        private readonly IEmailService _email;

        public AccountController(
            ILogger<AccountController> logger,
            IStringLocalizer<ExceptionResource> errLocalizer,
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ApiUnitOfWork api,
            ICalendarService calendar,
            IFileManager fileManager,
            IEmailService email
            )
        {
            _logger = logger;
            _errLocalizer = errLocalizer;
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
            _api = api;
            _calendar = calendar;
            _fileManager = fileManager;
            _email = email;
        }

        #region REGISTER
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

                try
                {
                    var calendarId = await _calendar.CreateCalendar(model.Email);
                    resource.CalendarId = calendarId;
                }
                catch (Exception err)
                {
                    _logger.LogError($"Google calendar error: {err.Message}");
                    _logger.LogError($"Unable to create calendar for {model.Email}");
                }

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
                    await _email.SendConfirmationLink(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(patient, false);

                    return RedirectToLocal(returnUrl);
                }

                var errors = _api.Patients.ReadError<IEnumerable<IdentityError>>(response);

                if (errors == null)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Patients.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        #endregion

        #region LOGIN
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
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { rememberMe = model.RememberMe, returnUrl = returnUrl });
                }

                ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                ModelState.AddModelError(string.Empty, "Wrong email or password");

                return View(model);
            }

            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(model);
        }
        #endregion

        #region EXTERNAL LOGIN
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
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("LoginWith2fa", new { rememberMe = false, returnUrl = returnUrl });
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginProvider = info.LoginProvider;

            var model = new ExternalLoginViewModel
            {
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            };

            return View("ExternalLogin", model);
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
                        throw new ApplicationException(_errLocalizer["LoadingExternalLoginInfo"]);
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
                        AddressId = address.AddressId,
                        BirthDate = model.BirthDate,
                        Sex = sex
                    };

                    try
                    {
                        var calendarId = await _calendar.CreateCalendar(model.Email);
                        patient.CalendarId = calendarId;
                    }
                    catch (Exception err)
                    {
                        _logger.LogError($"Google calendar error: {err.Message}");
                        _logger.LogError($"Unable to create calendar for {model.Email}");
                    }

                    var result = await _userManager.CreateAsync(patient);

                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(patient, info);

                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(patient, "Patient");
                            await _signInManager.SignInAsync(patient, false);
                            //await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                            return RedirectToLocal(returnUrl);
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLogin", model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AccountController.ExternalLoginConfirm.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
        #endregion

        #region LOGIN WITH 2FA
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            try
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

                if (user == null)
                {
                    throw new ApplicationException(_errLocalizer["Load2FA"]);
                }

                var model = new LoginWith2faViewModel { RememberMe = rememberMe };
                ViewBag.ReturnUrl = returnUrl;

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AccountController.LoginWith2fa.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    throw new ApplicationException(string.Format(_errLocalizer["LoadUserWithId"].Value, _userManager.GetUserId(User)));
                }

                var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

                var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberDevice);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                    return RedirectToLocal(returnUrl);
                }

                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");

                return View();
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AccountController.LoginWith2fa.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            try
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    throw new ApplicationException(_errLocalizer["Load2FA"]);
                }

                ViewBag.ReturnUrl = returnUrl;

                return View();
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AccountController.LoginWithRecoveryCode.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    throw new ApplicationException(_errLocalizer["Load2FA"]);
                }

                var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

                var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                    return RedirectToAction("Enable2fa", "Manage");
                }

                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");

                return View();
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AccountController.LoginWithRecoveryCode.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
        #endregion

        #region LOGOUT
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

                var response = _api.AppUsers.Get(userId, null, null);
                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.AppUsers.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }
                var user = _api.AppUsers.Read(response);

                var result = await _userManager.ConfirmEmailAsync(user, code);

                if (!result.Succeeded)
                {
                    throw new ApplicationException(_errLocalizer["FailedEmailConfirm"]);
                }

                return View("ConfirmEmail", "Account");
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AccountController.ConfirmEmail.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
        #endregion

        #region PASSWORD RESTORATION
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
                    var response = _api.AppUsers.Get(model.Email, null, null);
                    if (!response.IsSuccessStatusCode)
                    {
                        var statusCode = response.StatusCode;
                        var message = _api.AppUsers.ReadError<string>(response);

                        return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                    }
                    var user = _api.AppUsers.Read(response);

                    if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        return RedirectToAction("ForgotPasswordConfirm", "Account");
                    }

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme
                        );
                    await _email.SendResetPasswordLink(model.Email, callbackUrl);

                    return RedirectToAction("ForgotPasswordConfirm", "Account");
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
        public IActionResult ForgotPasswordConfirm()
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
                    throw new ApplicationException(_errLocalizer["CodeMustBeSupplied"]);
                }

                var model = new ResetPasswordViewModel { Code = code };

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AccountController.ResetPassword.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

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

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
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
        #endregion

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
