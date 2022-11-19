using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
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

        [HttpGet]
        public async Task<IEnumerable<Diagnosis>> Get()
        {
            return await _uow.Diagnoses.GetAllAsync();
        }

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
