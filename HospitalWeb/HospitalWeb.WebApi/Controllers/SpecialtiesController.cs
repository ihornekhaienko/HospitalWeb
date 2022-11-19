﻿using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
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

        [HttpGet]
        public async Task<IEnumerable<Specialty>> Get()
        {
            return await _uow.Specialties.GetAllAsync();
        }

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
