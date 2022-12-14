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
    /// Diagnoses
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosesController : ControllerBase
    {
        private readonly ILogger<DiagnosesController> _logger;
        private readonly IUnitOfWork _uow;

        public DiagnosesController(
            ILogger<DiagnosesController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Returns a list of diagnoses
        /// </summary>
        /// <returns>List of diagnoses</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Diagnosis>>> Get()
        {
            try
            {
                var diagnoses = await _uow.Diagnoses.GetAllAsync(
                include: d => d
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient));

                return new ObjectResult(diagnoses);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DiagnosesController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns a Diagnosis found by id
        /// </summary>
        /// <param name="id">Diagnosis id</param>
        /// <returns>The Diagnosis object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Diagnosis>> Get(int id)
        {
            try
            {
                var diagnosis = await _uow.Diagnoses.GetAsync(d => d.DiagnosisId == id,
               include: d => d
               .Include(d => d.Appointments)
                   .ThenInclude(a => a.Doctor)
               .Include(d => d.Appointments)
                   .ThenInclude(a => a.Patient));

                if (diagnosis == null)
                {
                    return NotFound("The diagnosis object wasn't found");
                }

                return new ObjectResult(diagnosis);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DiagnosesController.Get(id): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns a Diagnosis found by its name
        /// </summary>
        /// <param name="name">Diagnosis name</param>
        /// <returns>The Diagnosis object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Diagnosis>> Get(string name)
        {
            try
            {
                var diagnosis = await _uow.Diagnoses.GetAsync(d => d.DiagnosisName == name,
                include: d => d
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient));

                if (diagnosis == null)
                {
                    return NotFound("The diagnosis object wasn't found");
                }

                return new ObjectResult(diagnosis);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DiagnosesController.Get(name): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates the new Diagnosis
        /// </summary>
        /// <param name="diagnosis">Diagnosis to create</param>
        /// <returns>The Diagnosis object</returns>
        [HttpPost]
        [Authorize(Policy = "AdminsDoctorsOnly")]
        public async Task<ActionResult<Diagnosis>> Post(DiagnosisResourceModel diagnosis)
        {
            try
            {
                if (diagnosis == null)
                {
                    return BadRequest("Passing null object to the DiagnosesController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<DiagnosisResourceModel, Diagnosis>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<DiagnosisResourceModel, Diagnosis>(diagnosis);

                await _uow.Diagnoses.CreateAsync(entity);

                _logger.LogDebug($"Created diagnosis with id {entity.DiagnosisId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DiagnosesController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Diagnosis object data
        /// </summary>
        /// <param name="diagnosis">The Diagnosis to update</param>
        /// <returns>The Diagnosis object</returns>
        [HttpPut]
        [Authorize]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Diagnosis>> Put(Diagnosis diagnosis)
        {
            try
            {
                if (diagnosis == null)
                {
                    return BadRequest("Passing null object to the DiagnosesController.Put method");
                }

                await _uow.Diagnoses.UpdateAsync(diagnosis);

                _logger.LogDebug($"Updated diagnosis with id {diagnosis.DiagnosisId}");

                return Ok(diagnosis);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DiagnosesController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Diagnosis found by Id
        /// </summary>
        /// <param name="id">Diagnosis's id</param>
        /// <returns>The Diagnosis object</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Diagnosis>> Delete(int id)
        {
            try
            {
                var diagnosis = await _uow.Diagnoses.GetAsync(d => d.DiagnosisId == id);

                if (diagnosis == null)
                {
                    return NotFound("The diagnosis object wasn't found");
                }

                await _uow.Diagnoses.DeleteAsync(diagnosis);

                _logger.LogDebug($"Deleted diagnosis with id {diagnosis.DiagnosisId}");

                return Ok(diagnosis);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in DiagnosesController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
