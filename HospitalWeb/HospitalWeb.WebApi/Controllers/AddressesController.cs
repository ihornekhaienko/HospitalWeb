using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

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
        private readonly UnitOfWork _uow;

        public AddressesController(
            ILogger<AddressesController> logger,
            UnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Returns a list of Addresses
        /// </summary>
        /// <returns> A list of Addresses </returns>
        [HttpGet]
        public async Task<IEnumerable<Address>> Get()
        {
            return await _uow.Addresses.GetAllAsync();
        }

        /// <summary>
        /// Returns the Address object found by Id
        /// </summary>
        /// <param name="id"> Address id </param>
        /// <returns> The Address object </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> Get(int id)
        {
            var address = await _uow.Addresses.GetAsync(a => a.AddressId == id);

            if (address == null)
            {
                return NotFound();
            }

            return new ObjectResult(address);
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
            var obj = await _uow.Addresses.GetAsync(a => a.FullAddress == address && a.Locality.LocalityName == locality);

            if (obj == null)
            {
                return NotFound();
            }

            return new ObjectResult(obj);
        }

        /// <summary>
        /// Creates a new Address object
        /// </summary>
        /// <param name="address"> The Address object  </param>
        /// <returns> The Address object </returns>
        [HttpPost]
        public async Task<ActionResult<Address>> Post(Address address)
        {
            if (address == null)
            {
                return BadRequest();
            }

            await _uow.Addresses.CreateAsync(address);

            return Ok(address);
        }

        /// <summary>
        /// Updates the Address object data
        /// </summary>
        /// <param name="address"> The Address object  </param>
        /// <returns> The Address object </returns>
        [HttpPut]
        public async Task<ActionResult<Address>> Put(Address address)
        {
            if (address == null)
            {
                return BadRequest();
            }

            await _uow.Addresses.UpdateAsync(address);

            return Ok(address);
        }

        /// <summary>
        /// Deletes the Address object
        /// </summary>
        /// <param name="id"> The Address object id </param>
        /// <returns> The Address object </returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Address>> Delete(int id)
        {
            var address = await _uow.Addresses.GetAsync(a => a.AddressId == id);

            if (address == null)
            {
                return NotFound();
            }

            await _uow.Addresses.DeleteAsync(address);

            return Ok(address);
        }

        /// <summary>
        /// Deletes the Address object
        /// </summary>
        /// <param name="address"> The Address object  </param>
        /// <returns> The Address object </returns>
        [HttpDelete("{Address}")]
        public async Task<ActionResult<Address>> Delete(Address address)
        {
            if (address == null)
            {
                return NotFound();
            }

            await _uow.Addresses.DeleteAsync(address);

            return Ok(address);
        }
    }
}
