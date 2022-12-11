﻿using AutoMapper;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using HospitalWeb.WebApi.Models.SortStates;
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
        /// Returns a filtered list of Hospitals
        /// </summary>
        /// <param name="searchString">Search string that identifies Hospital</param>
        /// <param name="locality">Locality's Id</param>
        /// <param name="sortOrder">Sorting order of the filtered list</param>
        /// <param name="pageSize">Count of the result on one page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns>Fltered list of Hospitals</returns>
        [HttpGet]
        public async Task<IEnumerable<Hospital>> Get(
            string searchString,
            int? locality,
            HospitalSortState sortOrder = HospitalSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            int totalCount = 0;

            Func<Hospital, bool> filter = (h) =>
            {
                bool result = true;

                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    result = h.HospitalName.Contains(searchString, StringComparison.OrdinalIgnoreCase);
                }

                if (locality != null && locality != 0)
                {
                    result = result && h.Address.Locality.LocalityId == locality;
                }

                return result;
            };

            Func<IQueryable<Hospital>, IOrderedQueryable<Hospital>> orderBy = (hospitals) =>
            {
                totalCount = hospitals.Count();

                switch (sortOrder)
                {
                    case HospitalSortState.NameAsc:
                        hospitals = hospitals.OrderBy(h => h.HospitalName);
                        break;
                    case HospitalSortState.NameDesc:
                        hospitals = hospitals.OrderByDescending(h => h.HospitalName);
                        break;
                    case HospitalSortState.AddressAsc:
                        hospitals = hospitals.OrderBy(h => h.Address.ToString());
                        break;
                    case HospitalSortState.AddressDesc:
                        hospitals = hospitals.OrderByDescending(h => h.Address.ToString());
                        break;
                    case HospitalSortState.DoctorsCountAsc:
                        hospitals = hospitals.OrderBy(h => h.Doctors.Count);
                        break;
                    case HospitalSortState.DoctorsCountDesc:
                        hospitals = hospitals.OrderByDescending(h => h.Doctors.Count);
                        break;
                    default:
                        hospitals = hospitals.OrderBy(h => h.HospitalId);
                        break;
                }

                return (IOrderedQueryable<Hospital>)hospitals;
            };

            var hospitals = await _uow.Hospitals
                .GetAllAsync(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize,
                include: p => p
                 .Include(p => p.Address)
                    .ThenInclude(a => a.Locality)
                .Include(p => p.Doctors));

            Response.Headers.Add("TotalCount", totalCount.ToString());
            Response.Headers.Add("Count", hospitals.Count().ToString());
            Response.Headers.Add("PageSize", pageSize.ToString());
            Response.Headers.Add("PageNumber", pageNumber.ToString());
            Response.Headers.Add("TotalPages", ((int)Math.Ceiling(totalCount / (double)pageSize)).ToString());

            return hospitals;
        }

        /// <summary>
        /// Returns the Hospital found by Id
        /// </summary>
        /// <param name="id">Hospital's id</param>
        /// <returns>The Hospital object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Hospital>> Get(int id)
        {
            var hospital = await _uow.Hospitals
                .GetAsync(s => s.HospitalId == id, 
                include: s => s.Include(s => s.Doctors)
                    .Include(s => s.Address)
                        .ThenInclude(s => s.Locality));

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
            var hospital = await _uow.Hospitals
                .GetAsync(s => s.HospitalName == name, 
                include: s => s.Include(s => s.Doctors)
                    .Include(s => s.Address)
                        .ThenInclude(s => s.Locality));

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