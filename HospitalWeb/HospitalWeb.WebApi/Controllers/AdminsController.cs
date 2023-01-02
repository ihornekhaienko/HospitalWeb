using AutoMapper;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Users with Admin role
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly ILogger<AdminsController> _logger;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;

        public AdminsController(
            ILogger<AdminsController> logger,
            IUnitOfWork uow,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _uow = uow;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns a filtered list of Admins
        /// </summary>
        /// <param name="searchString">Search string that identifies Admin</param>
        /// <param name="sortOrder">Sorting order of the filtered list</param>
        /// <param name="pageSize">Count of the result on one page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns>The list of admins</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> Get(
            string searchString = null,
            AdminSortState sortOrder = AdminSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            try
            {
                int totalCount = 0;

                Func<Admin, bool> filter = (a) =>
                {
                    bool result = true;

                    if (!string.IsNullOrWhiteSpace(searchString))
                    {
                        result = a.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        a.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        a.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase);
                    }

                    return result;
                };

                Func<IQueryable<Admin>, IOrderedQueryable<Admin>> orderBy = (admins) =>
                {
                    totalCount = admins.Count();

                    switch (sortOrder)
                    {
                        case AdminSortState.NameAsc:
                            admins = admins.OrderBy(a => a.Name);
                            break;
                        case AdminSortState.NameDesc:
                            admins = admins.OrderByDescending(a => a.Name);
                            break;
                        case AdminSortState.SurnameAsc:
                            admins = admins.OrderBy(a => a.Surname);
                            break;
                        case AdminSortState.SurnameDesc:
                            admins = admins.OrderByDescending(a => a.Surname);
                            break;
                        case AdminSortState.EmailAsc:
                            admins = admins.OrderBy(a => a.Email);
                            break;
                        case AdminSortState.EmailDesc:
                            admins = admins.OrderByDescending(a => a.Email);
                            break;
                        case AdminSortState.PhoneAsc:
                            admins = admins.OrderBy(a => a.PhoneNumber);
                            break;
                        case AdminSortState.PhoneDesc:
                            admins = admins.OrderByDescending(a => a.PhoneNumber);
                            break;
                        case AdminSortState.LevelAsc:
                            admins = admins.OrderBy(a => a.IsSuperAdmin);
                            break;
                        case AdminSortState.LevelDesc:
                            admins = admins.OrderByDescending(a => a.IsSuperAdmin);
                            break;
                        default:
                            admins = admins.OrderBy(a => a.Id);
                            break;
                    }

                    return (IOrderedQueryable<Admin>)admins;
                };

                var admins = await _uow.Admins
                    .GetAllAsync(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize);

                Response.Headers.Add("TotalCount", totalCount.ToString());
                Response.Headers.Add("Count", admins.Count().ToString());
                Response.Headers.Add("PageSize", pageSize.ToString());
                Response.Headers.Add("PageNumber", pageNumber.ToString());
                Response.Headers.Add("TotalPages", ((int)Math.Ceiling(totalCount / (double)pageSize)).ToString());

                return new ObjectResult(admins);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdminsController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Admin found by Id or Email
        /// </summary>
        /// <param name="searchString">Admin's id or Email</param>
        /// <returns>The Admin object</returns>
        [HttpGet("{searchString}")]
        public async Task<ActionResult<Admin>> Get(string searchString)
        {
            try
            {
                var admin = await _uow.Admins.GetAsync(a => a.Id == searchString || a.Email == searchString);

                if (admin == null)
                {
                    return NotFound("The admin object wasn't found");
                }

                return new ObjectResult(admin);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdminsController.Get(searchString): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new Admin
        /// </summary>
        /// <param name="admin">Admin to create</param>
        /// <returns>The Admin object</returns>
        [HttpPost]
        [Authorize(Policy = "SuperAdminsOnly")]
        public async Task<ActionResult<Admin>> Post(AdminResourceModel admin)
        {
            try
            {
                if (admin == null)
                {
                    return BadRequest("Passing null object to the AdminsController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<AdminResourceModel, Admin>()
                    .ForMember(d => d.Image, o => o.Ignore()));
                var mapper = new Mapper(config);

                var entity = mapper.Map<AdminResourceModel, Admin>(admin);

                IdentityResult result;

                if (string.IsNullOrWhiteSpace(admin.Password))
                {
                    result = await _userManager.CreateAsync(entity);
                }
                else
                {
                    result = await _userManager.CreateAsync(entity, admin.Password);
                }

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(entity, "Admin");

                    _logger.LogDebug($"Created admin with id {entity.Id}");

                    return Ok(entity);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdminsController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Admin object data
        /// </summary>
        /// <param name="admin">The Admin to update</param>
        /// <returns>The Admin object</returns>
        [HttpPut]
        [Authorize(Policy = "SuperAdminsOnly")]
        public async Task<ActionResult<Admin>> Put(Admin admin)
        {
            try
            {
                if (admin == null)
                {
                    return BadRequest("Passing null object to the AdminsController.Put method");
                }

                var result = await _userManager.UpdateAsync(admin);

                if (result.Succeeded)
                {
                    _logger.LogDebug($"Updated admin with id {admin.Id}");

                    return Ok(admin);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdminsController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Admin found by Id or Email
        /// </summary>
        /// <param name="searchString">Admin's id or Email</param>
        /// <returns>The Admin object</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "SuperAdminsOnly")]
        public async Task<ActionResult<Admin>> Delete(string searchString)
        {
            try
            {
                var admin = await _uow.Admins.GetAsync(a => a.Id == searchString || a.Email == searchString);

                if (admin == null)
                {
                    return NotFound("The admin object wasn't found");
                }

                await _userManager.DeleteAsync(admin);

                _logger.LogDebug($"Deleted admin with id {admin.Id}");

                return Ok(admin);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AdminsController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
