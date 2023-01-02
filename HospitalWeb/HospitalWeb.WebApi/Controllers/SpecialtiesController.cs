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
    /// Doctor Specialties
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("/")]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ILogger<SpecialtiesController> _logger;
        private readonly IUnitOfWork _uow;

        public SpecialtiesController(
            ILogger<SpecialtiesController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Return a list of Specialties
        /// </summary>
        /// <returns>List of Specialties</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specialty>>> Get()
        {
            try
            {
                var specialties = await _uow.Specialties.GetAllAsync(include: s => s.Include(s => s.Doctors));

                return new ObjectResult(specialties);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SpecialtiesController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Specialty found by Id
        /// </summary>
        /// <param name="id">Specialty's id</param>
        /// <returns>The Specialty object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Specialty>> Get(int id)
        {
            try
            {
                var specialty = await _uow.Specialties.GetAsync(s => s.SpecialtyId == id, include: s => s.Include(s => s.Doctors));

                if (specialty == null)
                {
                    return NotFound("The specialty object wasn't found");
                }

                return new ObjectResult(specialty);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SpecialtiesController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Specialty found by name
        /// </summary>
        /// <param name="name">Specialty's name</param>
        /// <returns>The Specialty object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Specialty>> Get(string name)
        {
            try
            {
                var specialty = await _uow.Specialties.GetAsync(s => s.SpecialtyName == name, include: s => s.Include(s => s.Doctors));

                if (specialty == null)
                {
                    return NotFound("The specialty object wasn't found");
                }

                return new ObjectResult(specialty);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SpecialtiesController.Get(name): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new Specialty
        /// </summary>
        /// <param name="specialty">Specialty to create</param>
        /// <returns>The Specialty object</returns>
        [HttpPost]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Specialty>> Post(SpecialtyResourceModel specialty)
        {
            try
            {
                if (specialty == null)
                {
                    return BadRequest("Passing null object to the SpecialtiesController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<SpecialtyResourceModel, Specialty>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<SpecialtyResourceModel, Specialty>(specialty);

                await _uow.Specialties.CreateAsync(entity);

                _logger.LogDebug($"Created specialty with id {entity.SpecialtyId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SpecialtiesController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Specialty object data
        /// </summary>
        /// <param name="specialty">The Specialty to update</param>
        /// <returns>The Specialty object</returns>
        [HttpPut]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Specialty>> Put(Specialty specialty)
        {
            try
            {
                if (specialty == null)
                {
                    return BadRequest("Passing null object to the SpecialtiesController.Post method");
                }

                await _uow.Specialties.UpdateAsync(specialty);

                _logger.LogDebug($"Updated specialty with id {specialty.SpecialtyId}");

                return Ok(specialty);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SpecialtiesController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Specialty found by Id
        /// </summary>
        /// <param name="id">Specialty's id</param>
        /// <returns>The Specialty object</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Specialty>> Delete(int id)
        {
            try
            {
                var specialty = await _uow.Specialties.GetAsync(s => s.SpecialtyId == id);

                if (specialty == null)
                {
                    return NotFound("The specialty object wasn't found");
                }

                await _uow.Specialties.DeleteAsync(specialty);

                _logger.LogDebug($"Deleted specialty with id {specialty.SpecialtyId}");

                return Ok(specialty);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in SpecialtiesController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
