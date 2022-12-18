using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Notifications
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly IUnitOfWork _uow;

        public NotificationsController(
            ILogger<NotificationsController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Return a list of Notifications
        /// </summary>
        /// <returns>List of Notifications</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> Get()
        {
            try
            {
                var notifications = await _uow.Notifications.GetAllAsync(include: n => n.Include(n => n.AppUser));

                return new ObjectResult(notifications);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in NotificationsController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Notifications by User
        /// </summary>
        /// <param name="owner">User's id</param>
        /// <param name="isRead">Notifications state</param>
        /// <param name="pageSize">Count of the result on one page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns>List of Notifications</returns>
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<Notification>>> Get(string owner, bool? isRead = null, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                int totalCount = 0;

                Func<Notification, bool> filter = (n) =>
                {
                    var result = n.AppUserId == owner;

                    if (isRead != null)
                    {
                        result &= n.IsRead == isRead;
                    }

                    return result;
                };

                Func<IQueryable<Notification>, IOrderedQueryable<Notification>> orderBy = (notifications) =>
                {
                    totalCount = notifications.Count();

                    return notifications.OrderByDescending(n => n.NotificationId);
                };

                var notifications = await _uow.Notifications.GetAllAsync(
                    filter: filter,
                    orderBy: orderBy,
                    first: pageSize,
                    offset: (pageNumber - 1) * pageSize,
                    include: n => n.Include(n => n.AppUser));

                Response.Headers.Add("TotalCount", totalCount.ToString());
                Response.Headers.Add("Count", notifications.Count().ToString());
                Response.Headers.Add("PageSize", pageSize.ToString());
                Response.Headers.Add("PageNumber", pageNumber.ToString());
                Response.Headers.Add("TotalPages", ((int)Math.Ceiling(totalCount / (double)pageSize)).ToString());

                return new ObjectResult(notifications);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in NotificationsController.Get(owner): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Notification found by Id
        /// </summary>
        /// <param name="id">Notification's id</param>
        /// <returns>The Notification object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> Get(int id)
        {
            try
            {
                var notification = await _uow.Notifications.GetAsync(s => s.NotificationId == id, include: n => n.Include(n => n.AppUser));

                if (notification == null)
                {
                    return NotFound("The notifcation object wasn't found");
                }

                return new ObjectResult(notification);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in NotificationsController.Get(id): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new Notification
        /// </summary>
        /// <param name="Notification">Notification to create</param>
        /// <returns>The Notification object</returns>
        [HttpPost]
        public async Task<ActionResult<Notification>> Post(NotificationResourceModel Notification)
        {
            try
            {
                if (Notification == null)
                {
                    return BadRequest("Passing null object to the NotificationsController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<NotificationResourceModel, Notification>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<NotificationResourceModel, Notification>(Notification);

                await _uow.Notifications.CreateAsync(entity);

                _logger.LogDebug($"Created notification with id {entity.NotificationId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in NotificationsController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Notification object data
        /// </summary>
        /// <param name="notification">The Notification to update</param>
        /// <returns>The Notification object</returns>
        [HttpPut]
        public async Task<ActionResult<Notification>> Put(Notification notification)
        {
            try
            {
                if (notification == null)
                {
                    return BadRequest("Passing null object to the NotificationsController.Put method");
                }

                await _uow.Notifications.UpdateAsync(notification);

                _logger.LogDebug($"Updated notification with id {notification.NotificationId}");

                return Ok(notification);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in NotificationsController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Notification found by Id
        /// </summary>
        /// <param name="id">Notification's id</param>
        /// <returns>The Notification object</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Notification>> Delete(int id)
        {
            try
            {
                var notification = await _uow.Notifications.GetAsync(s => s.NotificationId == id);

                if (notification == null)
                {
                    return NotFound("The notifcation object wasn't found");
                }

                await _uow.Notifications.DeleteAsync(notification);

                _logger.LogDebug($"Deleted notification with id {notification.NotificationId}");

                return Ok(notification);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in NotificationsController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
