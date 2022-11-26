using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
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
        /// <param name="id">User's id</param>
        /// <returns>List of Notifications</returns>
        [HttpGet("details")]
        public async Task<IEnumerable<Notification>> Get(string id)
        {
            return await _uow.Notifications.GetAllAsync(filter: n => n.AppUserId == id, include: n => n.Include(n => n.AppUser));
        }

        /// <summary>
        /// Returns the Notification found by Id
        /// </summary>
        /// <param name="id">Notification's id</param>
        /// <returns>The Notification object</returns>
        [HttpGet("{id}")]
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
