using AutoMapper;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Application Users
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class AppUsersController : ControllerBase
    {
        private readonly ILogger<AppUsersController> _logger;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;

        public AppUsersController(
            ILogger<AppUsersController> logger,
            IUnitOfWork uow,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _uow = uow;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns a list of registered Users
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> Get()
        {
            try
            {
                var users = await _uow.AppUsers.GetAllAsync();

                return new ObjectResult(users);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppUsersController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the User found by Id or Email
        /// </summary>
        /// <param name="searchString">User's id or Email</param>
        /// <returns>The User object</returns>
        [HttpGet("{searchString}")]
        public async Task<ActionResult<AppUser>> Get(string searchString)
        {
            try
            {
                var user = await _uow.AppUsers.GetAsync(a => a.Id == searchString || a.Email == searchString);

                if (user == null)
                {
                    return NotFound("The user object wasn't found");
                }

                return new ObjectResult(user);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppUsersController.Get(searchString): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new User
        /// </summary>
        /// <param name="user">User to create</param>
        /// <returns>The User object</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AppUser>> Post(AppUserResourceModel user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("Passing null object to the AppUserController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<AppUserResourceModel, AppUser>()
                    .ForMember(d => d.Image, o => o.Ignore()));
                var mapper = new Mapper(config);

                var entity = mapper.Map<AppUserResourceModel, AppUser>(user);

                IdentityResult result;

                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    result = await _userManager.CreateAsync(entity);
                }
                else
                {
                    result = await _userManager.CreateAsync(entity, user.Password);
                }

                if (result.Succeeded)
                {
                    _logger.LogDebug($"Created user with id {entity.Id}");

                    return Ok(entity);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppUsersController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the User object data
        /// </summary>
        /// <param name="user">The User to update</param>
        /// <returns>The User object</returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<AppUser>> Put(AppUser user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("Passing null object to the AppUserController.Put method");
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogDebug($"Updated user with id {user.Id}");

                    return Ok(user);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppUsersController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the User found by Id or Email
        /// </summary>
        /// <param name="searchString">User's id or Email</param>
        /// <returns>The User object</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<AppUser>> Delete(string searchString)
        {
            try
            {
                var user = await _uow.AppUsers.GetAsync(a => a.Id == searchString || a.Email == searchString);

                if (user == null)
                {
                    return NotFound("The user object wasn't found");
                }

                await _userManager.DeleteAsync(user);

                _logger.LogDebug($"Deleted user with id {user.Id}");

                return Ok(user);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AppUsersController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
