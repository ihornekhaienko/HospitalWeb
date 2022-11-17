using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
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

        [HttpGet]
        public async Task<IEnumerable<Locality>> Get()
        {
            return await _uow.Localities.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Locality>> Get(int id)
        {
            return await _uow.Localities.GetAsync(l => l.LocalityId == id);
        }

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
    }
}
