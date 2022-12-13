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
        public async Task<IEnumerable<Diagnosis>> Get()
        {
            return await _uow.Diagnoses.GetAllAsync(                
                include: d => d
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient));
        }

        /// <summary>
        /// Returns a Diagnosis found by id
        /// </summary>
        /// <param name="id">Diagnosis id</param>
        /// <returns>The Diagnosis object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Diagnosis>> Get(int id)
        {
            var diagnosis = await _uow.Diagnoses.GetAsync(d => d.DiagnosisId == id,
                include: d => d
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient));

            if (diagnosis == null)
            {
                return NotFound();
            }

            return new ObjectResult(diagnosis);
        }

        /// <summary>
        /// Returns a Diagnosis found by its name
        /// </summary>
        /// <param name="name">Diagnosis name</param>
        /// <returns>The Diagnosis object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Diagnosis>> Get(string name)
        {
            var diagnosis = await _uow.Diagnoses.GetAsync(d => d.DiagnosisName == name, 
                include: d => d
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient));

            if (diagnosis == null)
            {
                return NotFound();
            }

            return new ObjectResult(diagnosis);
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
            if (diagnosis == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<DiagnosisResourceModel, Diagnosis>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<DiagnosisResourceModel, Diagnosis>(diagnosis);

            await _uow.Diagnoses.CreateAsync(entity);

            return Ok(entity);
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
            if (diagnosis == null)
            {
                return BadRequest();
            }

            await _uow.Diagnoses.UpdateAsync(diagnosis);

            return Ok(diagnosis);
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
            var diagnosis = await _uow.Diagnoses.GetAsync(d => d.DiagnosisId == id);

            if (diagnosis == null)
            {
                return NotFound();
            }

            await _uow.Diagnoses.DeleteAsync(diagnosis);

            return Ok(diagnosis);
        }
    }
}
