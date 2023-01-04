using AutoMapper;
using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Messages
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly IUnitOfWork _uow;

        public MessagesController(
            ILogger<MessagesController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Return a list of Messages
        /// </summary>
        /// <returns>List of Messages</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> Get()
        {
            try
            {
                var messages = await _uow.Messages.GetAllAsync(include: m => m.Include(m => m.User));

                return new ObjectResult(messages);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MessagesController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Message found by Id
        /// </summary>
        /// <param name="id">Message's id</param>
        /// <returns>The Message object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> Get(int id)
        {
            try
            {
                var message = await _uow.Messages.GetAsync(m => m.MessageId == id, 
                    include: m => m
                    .Include(m => m.User));

                if (message == null)
                {
                    return NotFound("The Message object wasn't found");
                }

                return new ObjectResult(message);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MessagesController.Get(id): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Filtered messages
        /// </summary>
        /// <param name="user">User's id</param>
        /// <param name="pageSize">Count of the result on one page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns>The Message object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<Message>>> Filter(
            string user = null, 
            int pageSize = 10, 
            int pageNumber = 1)
        {
            try
            {
                Func<Message, bool> filter = (m) =>
                {
                    var result = true;

                    result = result && m.UserId == user;

                    return result;
                };

                var messages = await _uow.Messages.GetAllAsync(
                    filter: filter,
                    first: pageSize,
                    offset: (pageNumber - 1) * pageSize,
                    include: m => m.Include(m => m.User));

                return new ObjectResult(messages);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MessagesController.Filter: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new Message
        /// </summary>
        /// <param name="message">Message to create</param>
        /// <returns>The Message object</returns>
        [HttpPost]
        public async Task<ActionResult<Message>> Post(MessageResourceModel message)
        {
            try
            {
                if (message == null)
                {
                    return BadRequest("Passing null object to the MessagesController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<MessageResourceModel, Message>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<MessageResourceModel, Message>(message);

                await _uow.Messages.CreateAsync(entity);

                _logger.LogDebug($"Created Message with id {entity.MessageId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MessagesController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Message object data
        /// </summary>
        /// <param name="message">The Message to update</param>
        /// <returns>The Message object</returns>
        [HttpPut]
        public async Task<ActionResult<Message>> Put(Message message)
        {
            try
            {
                if (message == null)
                {
                    return BadRequest("Passing null object to the MessagesController.Put method");
                }

                await _uow.Messages.UpdateAsync(message);

                _logger.LogDebug($"Updated Message with id {message.MessageId}");

                return Ok(message);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MessagesController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Message found by Id
        /// </summary>
        /// <param name="id">Message's id</param>
        /// <returns>The Message object</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Message>> Delete(int id)
        {
            try
            {
                var message = await _uow.Messages.GetAsync(m => m.MessageId == id);

                if (message == null)
                {
                    return NotFound("The Message object wasn't found");
                }

                await _uow.Messages.DeleteAsync(message);

                _logger.LogDebug($"Deleted Message with id {message.MessageId}");

                return Ok(message);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in MessagesController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
