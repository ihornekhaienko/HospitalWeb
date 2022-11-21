using AutoMapper;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;
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
        private readonly UnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;

        public AppUsersController(
            ILogger<AppUsersController> logger,
            UnitOfWork uow,
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
        public async Task<IEnumerable<AppUser>> Get()
        {
            return await _uow.AppUsers.GetAllAsync();
        }

        /// <summary>
        /// Returns the User found by Id or Email
        /// </summary>
        /// <param name="searchString">User's id or Email</param>
        /// <returns>The User object</returns>
        [HttpGet("{searchString}")]
        public async Task<ActionResult<AppUser>> Get(string searchString)
        {
            var user = await _uow.AppUsers.GetAsync(a => a.Id == searchString || a.Email == searchString);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        /// <summary>
        /// Creates the new User
        /// </summary>
        /// <param name="user">User to create</param>
        /// <returns>The User object</returns>
        [HttpPost]
        public async Task<ActionResult<AppUser>> Post(AppUserResourceModel user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<AppUserResourceModel, AppUser>());
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
                return Ok(entity);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Updates the User object data
        /// </summary>
        /// <param name="user">The User to update</param>
        /// <returns>The User object</returns>
        [HttpPut]
        public async Task<ActionResult<AppUser>> Put(AppUserResourceModel user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<AppUserResourceModel, AppUser>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<AppUserResourceModel, AppUser>(user);

            IdentityResult result;

            if (!string.IsNullOrWhiteSpace(user.NewPassword))
            {
                result = await _userManager.ChangePasswordAsync(entity, user.Password, user.NewPassword);

                if (result.Succeeded)
                {
                    return Ok(entity);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            result = await _userManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                return Ok(entity);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Deletes the User found by Id or Email
        /// </summary>
        /// <param name="searchString">User's id or Email</param>
        /// <returns>The User object</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<AppUser>> Delete(string searchString)
        {
            var user = await _uow.AppUsers.GetAsync(a => a.Id == searchString || a.Email == searchString);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return Ok(user);
        }
    }
}
