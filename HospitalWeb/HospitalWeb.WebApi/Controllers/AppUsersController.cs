﻿using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppUsersController : ControllerBase
    {
        private readonly ILogger<AppUsersController> _logger;
        private readonly UnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;

        public AppUsersController(
            ILogger<AppUsersController> logger,
            UnitOfWork uow,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _uow = uow;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IEnumerable<AppUser>> Get()
        {
            return await _uow.AppUsers.GetAllAsync();
        }

        [HttpGet("{searchString}")]
        public async Task<ActionResult<AppUser>> Get(string searchString)
        {
            var user = await _uow.AppUsers.GetAsync(a => a.Id == searchString || a.Email == searchString);

            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpPost]
        public async Task<ActionResult<AppUser>> Post(AppUserResourceModel user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var entity = new AppUser
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                UserName = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed
            };

            IdentityResult result;

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                result = await _userManager.CreateAsync(entity);
            }
            else
            {
                result = await _userManager.CreateAsync(entity, user.Password);
            }    

            if (result.Succeeded)
            {
                return Ok(entity);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPut]
        public async Task<ActionResult<AppUser>> Put(AppUserResourceModel user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var entity = await _uow.AppUsers.GetAsync(a => a.Email == user.Email);
            entity.UserName = user.UserName;
            entity.PhoneNumber = user.PhoneNumber;
            entity.Name = user.Name;
            entity.Surname = user.Surname;
            entity.Image = user.Image;

            if (entity == null)
            {
                return BadRequest();
            }

            IdentityResult result;

            if (!string.IsNullOrWhiteSpace(user.NewPassword))
            {
                result = await _userManager.ChangePasswordAsync(entity, user.Password, user.NewPassword);

                if (result.Succeeded)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            result = await _userManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                return Ok(user);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<AppUser>> Delete(string id)
        {
            var user = await _uow.AppUsers.GetAsync(a => a.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return Ok(user);
        }

        [HttpDelete("{AppUser}")]
        public async Task<ActionResult<AppUser>> Delete(AppUser user)
        {
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return Ok(user);
        }
    }
}
