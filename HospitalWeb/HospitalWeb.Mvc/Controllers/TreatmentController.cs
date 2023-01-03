using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Filters.Builders.Implementations;
using HospitalWeb.Mvc.Services.Interfaces;
using HospitalWeb.Mvc.ViewModels.Error;
using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Mvc.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HospitalWeb.Mvc.ViewModels.Treatment;
using HospitalWeb.Mvc.Services.Utility;
using Microsoft.Extensions.Localization;

namespace HospitalWeb.Mvc.Controllers
{
    [Authorize(Roles = "Patient")]
    public class TreatmentController : Controller
    {
        private readonly ILogger<TreatmentController> _logger;
        private readonly IStringLocalizer<ExceptionResource> _errLocalizer;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApiUnitOfWork _api;
        private readonly ICalendarService _calendar;
        private readonly IFileManager _fileManager;
        private readonly ITokenManager _tokenManager;
        private readonly ILiqPayClient _liqpay;

        public TreatmentController(
            ILogger<TreatmentController> logger,
            IStringLocalizer<ExceptionResource> errLocalizer,
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager,
            ApiUnitOfWork api,
            ICalendarService calendar,
            IFileManager fileManager,
            ITokenManager tokenManager,
            ILiqPayClient liqpay)
        {
            _logger = logger;
            _errLocalizer = errLocalizer;
            _environment = environment;
            _userManager = userManager;
            _api = api;
            _calendar = calendar;
            _fileManager = fileManager;
            _tokenManager = tokenManager;
            _liqpay = liqpay;
        }

        #region HISTORY
        [HttpGet]
        public async Task<IActionResult> History(
            string searchString,
            int? state,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            AppointmentSortState sortOrder = AppointmentSortState.DateDesc)
        {
            try
            {
                ViewBag.Image = await _fileManager.GetBytes(Path.Combine(_environment.WebRootPath, "files/images/profile.jpg"));

                var response = _api.Patients.Get(User.Identity.Name, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Patients.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var user = _api.Patients.Read(response);

                ViewBag.Calendar = _calendar.GetCalendar(user);

                var builder = new AppointmentsViewModelBuilder(
                    _api, page, searchString, sortOrder, user.Id, state: state, fromTime: fromDate, toTime: toDate);
                var director = new ViewModelBuilderDirector();
                director.MakeViewModel(builder);
                var viewModel = builder.GetViewModel();

                return View(viewModel);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in TreatmentController.History.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
        #endregion

        #region CANCEL
        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                var response = _api.Appointments.Get(id, tokenResult.Token, tokenResult.Provider);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Appointments.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var appointment = _api.Appointments.Read(response);

                if (appointment.State != State.Planned)
                {
                    throw new Exception(string.Format(_errLocalizer["MustBePlannedToCancel"], appointment.State));
                }

                appointment.State = State.Canceled;
                _api.Appointments.Put(appointment, tokenResult.Token, tokenResult.Provider);

                var meeting = appointment.Meetings.FirstOrDefault();
                if (meeting != null)
                {
                    _api.Meetings.Delete(meeting.MeetingId, tokenResult.Token, tokenResult.Provider);
                }

                await _calendar.CancelEvent(appointment.Doctor, appointment);
                await _calendar.CancelEvent(appointment.Patient, appointment);

                return RedirectToAction("History", "Treatment");
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in TreatmentController.Cancel.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
        #endregion

        #region PAYMENT
        [HttpGet]
        public IActionResult PayOff(int id)
        {
            try
            {
                var response = _api.Appointments.Get(id, null, null);

                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Appointments.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var appointment = _api.Appointments.Read(response);

                var model = new PaymentViewModel
                {
                    AppointmentId = appointment.AppointmentId,
                    Amount = appointment.Price,
                    Description = $"Appointment on {appointment.AppointmentDate.ToString("MM/dd/yyyy")}",
                    Phone = appointment.Patient.PhoneNumber,
                    Email = appointment.Patient.Email
                };

                return View(model);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in TreatmentController.PayOff.Get: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PayOff(PaymentViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var request = new LiqPayRequest
                {
                    Email = model.Email,
                    Action = LiqPayRequestAction.Pay,
                    Phone = model.Phone,
                    Amount = model.Amount,
                    Currency = "UAH",
                    Description = model.Description,
                    OrderId = Guid.NewGuid().ToString(),
                    Card = model.Card,
                    CardExpiredMonth = model.CardExpirationMonth,
                    CardExpiredYear = model.CardExpirationYear,
                    CardCvv = model.Cvv
                };

                var liqpayResponse = await _liqpay.RequestAsync("request", request);

                if (liqpayResponse.Status != LiqPayResponseStatus.Success)
                {
                    _logger.LogError($"Error during requiring");
                    _logger.LogError($"Status: {liqpayResponse.Status}");
                    _logger.LogError($"Error code: {liqpayResponse.ErrorCode}");
                    _logger.LogError($"Error description: {liqpayResponse.ErrorDescription}");

                    return RedirectToAction("Acquiring", "Error", new ErrorViewModel { Message = liqpayResponse.ErrorDescription });
                }

                var response = _api.Appointments.Get(model.AppointmentId);
                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = response.StatusCode;
                    var message = _api.Appointments.ReadError<string>(response);

                    return RedirectToAction("Http", "Error", new { statusCode = statusCode, message = message });
                }

                var user = await _userManager.GetUserAsync(User);
                var tokenResult = await _tokenManager.GetToken(user);

                var appointment = _api.Appointments.Read(response);

                appointment.IsPaid = true;
                response = _api.Appointments.Put(appointment, tokenResult.Token, tokenResult.Provider);

                return RedirectToAction("History", "Treatment");
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in TreatmentController.PayOff.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return RedirectToAction("Index", "Error", new ErrorViewModel { Message = err.Message });
            }
        }
        #endregion
    }
}
