using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

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
        private readonly UnitOfWork _uow;

        public DiagnosesController(
            ILogger<DiagnosesController> logger,
            UnitOfWork uow)
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
            return await _uow.Diagnoses.GetAllAsync();
        }

        /// <summary>
        /// Returns a Diagnosis found by id
        /// </summary>
        /// <param name="id">Diagnosis id</param>
        /// <returns>The Diagnosis object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Diagnosis>> Get(int id)
        {
            var diagnosis = await _uow.Diagnoses.GetAsync(d => d.DiagnosisId == id);

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
            var diagnosis = await _uow.Diagnoses.GetAsync(d => d.DiagnosisName == name);

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
        public async Task<ActionResult<Diagnosis>> Post(Diagnosis diagnosis)
        {
            if (diagnosis == null)
            {
                return BadRequest();
            }

            await _uow.Diagnoses.CreateAsync(diagnosis);

            return Ok(diagnosis);
        }

        /// <summary>
        /// Updates the Diagnosis object data
        /// </summary>
        /// <param name="diagnosis">The Diagnosis to update</param>
        /// <returns>The Diagnosis object</returns>
        [HttpPut]
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

        /// <summary>
        /// Deletes the Diagnosis 
        /// </summary>
        /// <param name="diagnosis">The Diagnosis object</param>
        /// <returns>The Diagnosis object</returns>
        [HttpDelete("{Diagnosis}")]
        public async Task<ActionResult<Diagnosis>> Delete(Diagnosis diagnosis)
        {
            if (diagnosis == null)
            {
                return NotFound();
            }

            await _uow.Diagnoses.DeleteAsync(diagnosis);

            return Ok(diagnosis);
        }
    }
}
