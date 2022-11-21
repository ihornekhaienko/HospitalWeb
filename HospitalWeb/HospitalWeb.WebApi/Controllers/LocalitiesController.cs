﻿using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;
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
        public async Task<ActionResult<Locality>> Post(LocalityResourceModel locality)
        {
            if (locality == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<LocalityResourceModel, Locality>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<LocalityResourceModel, Locality>(locality);

            await _uow.Localities.CreateAsync(entity);

            return Ok(entity);
        }

        /// <summary>
        /// Updates the Locality object data
        /// </summary>
        /// <param name="locality">The Locality to update</param>
        /// <returns>The Locality object</returns>
        [HttpPut]
        public async Task<ActionResult<Locality>> Put(LocalityResourceModel locality)
        {
            if (locality == null)
            {
                return BadRequest();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<LocalityResourceModel, Locality>());
            var mapper = new Mapper(config);

            var entity = mapper.Map<LocalityResourceModel, Locality>(locality);

            await _uow.Localities.UpdateAsync(entity);

            return Ok(entity);
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
    }
}
