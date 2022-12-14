using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Localities
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class LocalitiesController : ControllerBase
    {
        private readonly ILogger<LocalitiesController> _logger;
        private readonly IUnitOfWork _uow;

        public LocalitiesController(
            ILogger<LocalitiesController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Returns a list of Localities
        /// </summary>
        /// <returns>List of Localities</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Locality>>> Get()
        {
            try
            {
                var localities = await _uow.Localities.GetAllAsync();

                return new ObjectResult(localities);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in LocalitiesController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Locality found by Id
        /// </summary>
        /// <param name="id">Locality's id</param>
        /// <returns>The Locality object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Locality>> Get(int id)
        {
            try
            {
                var locality = await _uow.Localities.GetAsync(l => l.LocalityId == id,
                include: l => l
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients));

                if (locality == null)
                {
                    return NotFound("The locality object wasn't found");
                }

                return new ObjectResult(locality);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in LocalitiesController.Get(id): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Locality found by name
        /// </summary>
        /// <param name="name">Locality's name</param>
        /// <returns>The Locality object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Locality>> Get(string name)
        {
            try
            {
                var locality = await _uow.Localities.GetAsync(l => l.LocalityName == name,
                include: l => l
                .Include(l => l.Addresses)
                    .ThenInclude(a => a.Patients));

                if (locality == null)
                {
                    return NotFound("The locality object wasn't found");
                }

                return new ObjectResult(locality);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in LocalitiesController.Get(name): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new Locality
        /// </summary>
        /// <param name="locality">Locality to create</param>
        /// <returns>The Locality object</returns>
        [HttpPost]
        public async Task<ActionResult<Locality>> Post(LocalityResourceModel locality)
        {
            try
            {
                if (locality == null)
                {
                    return BadRequest("Passing null object to the LocalitiesController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<LocalityResourceModel, Locality>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<LocalityResourceModel, Locality>(locality);

                await _uow.Localities.CreateAsync(entity);

                _logger.LogDebug($"Created locality with id {entity.LocalityId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in LocalitiesController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Locality object data
        /// </summary>
        /// <param name="locality">The Locality to update</param>
        /// <returns>The Locality object</returns>
        [HttpPut]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Locality>> Put(Locality locality)
        {
            try
            {
                if (locality == null)
                {
                    return BadRequest("Passing null object to the LocalitiesController.Put method");
                }

                await _uow.Localities.UpdateAsync(locality);

                _logger.LogDebug($"Updated locality with id {locality.LocalityId}");

                return Ok(locality);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in LocalitiesController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Locality found by Id
        /// </summary>
        /// <param name="id">Locality's id</param>
        /// <returns>The Locality object</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Locality>> Delete(int id)
        {
            try
            {
                var locality = await _uow.Localities.GetAsync(l => l.LocalityId == id);

                if (locality == null)
                {
                    return NotFound("The locality object wasn't found");
                }

                await _uow.Localities.DeleteAsync(locality);

                _logger.LogDebug($"Deleted locality with id {locality.LocalityId}");

                return Ok(locality);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in LocalitiesController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
