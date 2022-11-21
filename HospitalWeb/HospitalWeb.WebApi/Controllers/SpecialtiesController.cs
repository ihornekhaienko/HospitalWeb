using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;
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
        public async Task<ActionResult<Specialty>> Post(SpecialtyResourceModel specialty)
        {
            if (specialty == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<SpecialtyResourceModel, Specialty>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<SpecialtyResourceModel, Specialty>(specialty);

            await _uow.Specialties.CreateAsync(entity);

            return Ok(entity);
        }

        /// <summary>
        /// Updates the Specialty object data
        /// </summary>
        /// <param name="specialty">The Specialty to update</param>
        /// <returns>The Specialty object</returns>
        [HttpPut]
        public async Task<ActionResult<Specialty>> Put(SpecialtyResourceModel specialty)
        {
            if (specialty == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<SpecialtyResourceModel, Specialty>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<SpecialtyResourceModel, Specialty>(specialty);

            await _uow.Specialties.UpdateAsync(entity);

            return Ok(entity);
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
    }
}
