using AutoMapper;
using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    /// <summary>
    /// Patients Addresses
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly ILogger<AddressesController> _logger;
        private readonly IUnitOfWork _uow;

        public AddressesController(
            ILogger<AddressesController> logger,
            IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Returns a list of Addresses
        /// </summary>
        /// <returns> A list of Addresses </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> Get()
        {
            try
            {
                var addresses = await _uow.Addresses.GetAllAsync(include: a => a
                    .Include(a => a.Locality)
                    .Include(a => a.Hospitals)
                    .Include(a => a.Patients));

                return new ObjectResult(addresses);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AddressesController.Get(): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Address object found by Id
        /// </summary>
        /// <param name="id"> Address id </param>
        /// <returns> The Address object </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> Get(int id)
        {
            try
            {
                var address = await _uow.Addresses.GetAsync(a => a.AddressId == id,
                    include: a => a
                    .Include(a => a.Locality)
                    .Include(a => a.Hospitals)
                    .Include(a => a.Patients));

                if (address == null)
                {
                    return NotFound("The address object wasn't found");
                }

                return new ObjectResult(address);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AddressesController.Get(id): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Returns the Address object found by full address text and locality name
        /// </summary>
        /// <param name="address"> Full address </param>
        /// <param name="locality"> Locality name </param>
        /// <returns> The Address object </returns>
        [HttpGet("details")]
        public async Task<ActionResult<Address>> Get(string address, string locality)
        {
            try
            {
                var obj = await _uow.Addresses
                    .GetAsync(a => a.FullAddress == address && a.Locality.LocalityName == locality,
                include: a => a
                    .Include(a => a.Locality)
                    .Include(a => a.Hospitals)
                    .Include(a => a.Patients));

                if (obj == null)
                {
                    return NotFound("The address object wasn't found");
                }

                return new ObjectResult(obj);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AddressesController.Get(address, locality): {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Creates a new Address object
        /// </summary>
        /// <param name="address"> The Address object  </param>
        /// <returns> The Address object </returns>
        [HttpPost]
        public async Task<ActionResult<Address>> Post(AddressResourceModel address)
        {
            try
            {
                if (address == null)
                {
                    return BadRequest("Passing null object to the AddressesController.Post method");
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<AddressResourceModel, Address>());
                var mapper = new Mapper(config);

                var entity = mapper.Map<AddressResourceModel, Address>(address);

                await _uow.Addresses.CreateAsync(entity);

                _logger.LogDebug($"Created address with id {entity.AddressId}");

                return Ok(entity);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AddressesController.Post: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Updates the Address object data
        /// </summary>
        /// <param name="address"> The Address object  </param>
        /// <returns> The Address object </returns>
        [HttpPut]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Address>> Put(Address address)
        {
            try
            {
                if (address == null)
                {
                    return BadRequest("Passing null object to the AddressesController.Put method");
                }

                await _uow.Addresses.UpdateAsync(address);

                _logger.LogDebug($"Updated address with id {address.AddressId}");

                return Ok(address);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AddressesController.Put: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Deletes the Address object
        /// </summary>
        /// <param name="id"> The Address object id </param>
        /// <returns> The Address object </returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminsOnly")]
        public async Task<ActionResult<Address>> Delete(int id)
        {
            try
            {
                var address = await _uow.Addresses.GetAsync(a => a.AddressId == id);

                if (address == null)
                {
                    return NotFound("The address object wasn't found");
                }

                await _uow.Addresses.DeleteAsync(address);

                _logger.LogDebug($"Deleted address with id {address.AddressId}");

                return Ok(address);
            }
            catch (Exception err)
            {
                _logger.LogError($"Error in AddressesController.Delete: {err.Message}");
                _logger.LogError($"Inner exception:\n{err.InnerException}");
                _logger.LogTrace(err.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }
    }
}
