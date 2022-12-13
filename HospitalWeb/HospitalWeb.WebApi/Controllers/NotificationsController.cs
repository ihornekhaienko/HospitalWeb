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
        public async Task<IEnumerable<Notification>> Get()
        {
            return await _uow.Notifications.GetAllAsync(include: n => n.Include(n => n.AppUser));
        }

        /// <summary>
        /// Returns the Notifications by User
        /// </summary>
        /// <param name="owner">User's id</param>
        /// <param name="pageSize">Count of the result on one page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns>List of Notifications</returns>
        [HttpGet("details")]
        public async Task<IEnumerable<Notification>> Get(string owner, int pageSize = 10, int pageNumber = 1)
        {
            int totalCount = 0;

            Func<Notification, bool> filter = (n) =>
            {
                return n.AppUserId == owner;
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

            return notifications;
        }

        /// <summary>
        /// Returns the Notification found by Id
        /// </summary>
        /// <param name="id">Notification's id</param>
        /// <returns>The Notification object</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Notification>> Get(int id)
        {
            var notification = await _uow.Notifications.GetAsync(s => s.NotificationId == id, include: n => n.Include(n => n.AppUser));

            if (notification == null)
            {
                return NotFound();
            }

            return new ObjectResult(notification);
        }

        /// <summary>
        /// Creates the new Notification
        /// </summary>
        /// <param name="Notification">Notification to create</param>
        /// <returns>The Notification object</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Notification>> Post(NotificationResourceModel Notification)
        {
            if (Notification == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<NotificationResourceModel, Notification>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<NotificationResourceModel, Notification>(Notification);

            await _uow.Notifications.CreateAsync(entity);

            return Ok(entity);
        }

        /// <summary>
        /// Updates the Notification object data
        /// </summary>
        /// <param name="notification">The Notification to update</param>
        /// <returns>The Notification object</returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<Notification>> Put(Notification notification)
        {
            if (notification == null)
            {
                return BadRequest();
            }

            await _uow.Notifications.UpdateAsync(notification);

            return Ok(notification);
        }

        /// <summary>
        /// Deletes the Notification found by Id
        /// </summary>
        /// <param name="id">Notification's id</param>
        /// <returns>The Notification object</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Notification>> Delete(int id)
        {
            var notification = await _uow.Notifications.GetAsync(s => s.NotificationId == id);

            if (notification == null)
            {
                return NotFound();
            }

            await _uow.Notifications.DeleteAsync(notification);

            return Ok(notification);
        }
    }
}
