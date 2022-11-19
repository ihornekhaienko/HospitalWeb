using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Doctor Specialties
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ILogger<SpecialtiesController> _logger;
        private readonly UnitOfWork _uow;

        public SpecialtiesController(
            ILogger<SpecialtiesController> logger,
            UnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Return a list of Specialties
        /// </summary>
        /// <returns>List of Specialties</returns>
        [HttpGet]
        public async Task<IEnumerable<Specialty>> Get()
        {
            return await _uow.Specialties.GetAllAsync();
        }

        /// <summary>
        /// Returns the Specialty found by Id
        /// </summary>
        /// <param name="id">Specialty's id</param>
        /// <returns>The Specialty object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Specialty>> Get(int id)
        {
            var specialty = await _uow.Specialties.GetAsync(s => s.SpecialtyId == id);

            if (specialty == null)
            {
                return NotFound();
            }

            return new ObjectResult(specialty);
        }

        /// <summary>
        /// Returns the Specialty found by name
        /// </summary>
        /// <param name="name">Specialty's name</param>
        /// <returns>The Specialty object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Specialty>> Get(string name)
        {
            var specialty = await _uow.Specialties.GetAsync(s => s.SpecialtyName == name);

            if (specialty == null)
            {
                return NotFound();
            }

            return new ObjectResult(specialty);
        }

        /// <summary>
        /// Creates the new Specialty
        /// </summary>
        /// <param name="specialty">Specialty to create</param>
        /// <returns>The Specialty object</returns>
        [HttpPost]
        public async Task<ActionResult<Specialty>> Post(Specialty specialty)
        {
            if (specialty == null)
            {
                return BadRequest();
            }

            await _uow.Specialties.CreateAsync(specialty);

            return Ok(specialty);
        }

        /// <summary>
        /// Updates the Specialty object data
        /// </summary>
        /// <param name="specialty">The Specialty to update</param>
        /// <returns>The Specialty object</returns>
        [HttpPut]
        public async Task<ActionResult<Specialty>> Put(Specialty specialty)
        {
            if (specialty == null)
            {
                return BadRequest();
            }

            await _uow.Specialties.UpdateAsync(specialty);

            return Ok(specialty);
        }

        /// <summary>
        /// Deletes the Specialty found by Id
        /// </summary>
        /// <param name="id">Specialty's id</param>
        /// <returns>The Specialty object</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Specialty>> Delete(int id)
        {
            var specialty = await _uow.Specialties.GetAsync(s => s.SpecialtyId == id);

            if (specialty == null)
            {
                return NotFound();
            }

            await _uow.Specialties.DeleteAsync(specialty);

            return Ok(specialty);
        }

        /// <summary>
        /// Deletes the Specialty 
        /// </summary>
        /// <param name="specialty">The Specialty object</param>
        /// <returns>The Specialty object</returns>
        [HttpDelete("{Specialty}")]
        public async Task<ActionResult<Specialty>> Delete(Specialty specialty)
        {
            if (specialty == null)
            {
                return NotFound();
            }

            await _uow.Specialties.DeleteAsync(specialty);

            return Ok(specialty);
        }
    }
}
