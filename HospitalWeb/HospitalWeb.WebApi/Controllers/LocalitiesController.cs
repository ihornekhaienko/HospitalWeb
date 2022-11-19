using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

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
        private readonly UnitOfWork _uow;

        public LocalitiesController(
            ILogger<LocalitiesController> logger,
            UnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Returns a list of Localities
        /// </summary>
        /// <returns>List of Localities</returns>
        [HttpGet]
        public async Task<IEnumerable<Locality>> Get()
        {
            return await _uow.Localities.GetAllAsync();
        }

        /// <summary>
        /// Returns the Locality found by Id
        /// </summary>
        /// <param name="id">Locality's id</param>
        /// <returns>The Locality object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Locality>> Get(int id)
        {
            var locality = await _uow.Localities.GetAsync(l => l.LocalityId == id);

            if (locality == null)
            {
                return NotFound();
            }

            return new ObjectResult(locality);
        }

        /// <summary>
        /// Returns the Locality found by name
        /// </summary>
        /// <param name="name">Locality's name</param>
        /// <returns>The Locality object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Locality>> Get(string name)
        {
            var locality = await _uow.Localities.GetAsync(l => l.LocalityName == name);

            if (locality == null)
            {
                return NotFound();
            }

            return new ObjectResult(locality);
        }

        /// <summary>
        /// Creates the new Locality
        /// </summary>
        /// <param name="locality">Locality to create</param>
        /// <returns>The Locality object</returns>
        [HttpPost]
        public async Task<ActionResult<Locality>> Post(Locality locality)
        {
            if (locality == null)
            {
                return BadRequest();
            }

            await _uow.Localities.CreateAsync(locality);

            return Ok(locality);
        }

        /// <summary>
        /// Updates the Locality object data
        /// </summary>
        /// <param name="locality">The Locality to update</param>
        /// <returns>The Locality object</returns>
        [HttpPut]
        public async Task<ActionResult<Locality>> Put(Locality locality)
        {
            if (locality == null)
            {
                return BadRequest();
            }

            await _uow.Localities.UpdateAsync(locality);

            return Ok(locality);
        }

        /// <summary>
        /// Deletes the Locality found by Id
        /// </summary>
        /// <param name="id">Locality's id</param>
        /// <returns>The Locality object</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Locality>> Delete(int id)
        {
            var locality = await _uow.Localities.GetAsync(l => l.LocalityId == id);

            if (locality == null)
            {
                return NotFound();
            }

            await _uow.Localities.DeleteAsync(locality);

            return Ok(locality);
        }

        /// <summary>
        /// Deletes the Locality 
        /// </summary>
        /// <param name="locality">The Locality object</param>
        /// <returns>The Locality object</returns>
        [HttpDelete("{Locality}")]
        public async Task<ActionResult<Locality>> Delete(Locality locality)
        {
            if (locality == null)
            {
                return NotFound();
            }

            await _uow.Localities.DeleteAsync(locality);

            return Ok(locality);
        }
    }
}
