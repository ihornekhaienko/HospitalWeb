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
    /// Doctor Hospitals
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalsController : ControllerBase
    {
        private readonly ILogger<HospitalsController> _logger;
        private readonly IUnitOfWork _uow;

        public HospitalsController(
            ILogger<HospitalsController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Return a list of Hospitals
        /// </summary>
        /// <returns>List of Hospitals</returns>
        [HttpGet]
        public async Task<IEnumerable<Hospital>> Get()
        {
            return await _uow.Hospitals.GetAllAsync(include: s => s.Include(s => s.Doctors).Include(s => s.Address));
        }

        /// <summary>
        /// Returns the Hospital found by Id
        /// </summary>
        /// <param name="id">Hospital's id</param>
        /// <returns>The Hospital object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Hospital>> Get(int id)
        {
            var hospital = await _uow.Hospitals.GetAsync(s => s.HospitalId == id, include: s => s.Include(s => s.Doctors).Include(s => s.Address));

            if (hospital == null)
            {
                return NotFound();
            }

            return new ObjectResult(hospital);
        }

        /// <summary>
        /// Returns the Hospital found by name
        /// </summary>
        /// <param name="name">Hospital's name</param>
        /// <returns>The Hospital object</returns>
        [HttpGet("details")]
        public async Task<ActionResult<Hospital>> Get(string name)
        {
            var hospital = await _uow.Hospitals.GetAsync(s => s.HospitalName == name, include: s => s.Include(s => s.Doctors).Include(s => s.Address));

            if (hospital == null)
            {
                return NotFound();
            }

            return new ObjectResult(hospital);
        }

        /// <summary>
        /// Creates the new Hospital
        /// </summary>
        /// <param name="hospital">Hospital to create</param>
        /// <returns>The Hospital object</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Hospital>> Post(HospitalResourceModel hospital)
        {
            if (hospital == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<HospitalResourceModel, Hospital>()
                .ForMember(d => d.Image, o => o.AllowNull()));
            var mapper = new Mapper(config);

            var entity = mapper.Map<HospitalResourceModel, Hospital>(hospital);

            await _uow.Hospitals.CreateAsync(entity);

            return Ok(entity);
        }

        /// <summary>
        /// Updates the Hospital object data
        /// </summary>
        /// <param name="hospital">The Hospital to update</param>
        /// <returns>The Hospital object</returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<Hospital>> Put(Hospital hospital)
        {
            if (hospital == null)
            {
                return BadRequest();
            }

            await _uow.Hospitals.UpdateAsync(hospital);

            return Ok(hospital);
        }

        /// <summary>
        /// Deletes the Hospital found by Id
        /// </summary>
        /// <param name="id">Hospital's id</param>
        /// <returns>The Hospital object</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Hospital>> Delete(int id)
        {
            var hospital = await _uow.Hospitals.GetAsync(s => s.HospitalId == id);

            if (hospital == null)
            {
                return NotFound();
            }

            await _uow.Hospitals.DeleteAsync(hospital);

            return Ok(hospital);
        }
    }
}
