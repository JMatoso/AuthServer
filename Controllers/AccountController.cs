using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jwt_identity_api.Data;
using jwt_identity_api.Data.Repositories;
using jwt_identity_api.DTO;
using jwt_identity_api.Models;
using jwt_identity_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace jwt_identity_api.Controllers
{
    /// <summary>
    /// Accounts Management and Authentication.
    /// </summary>
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserApp _user;

        public AccountController(
            IUserApp user,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _user = user;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <response code="200">Ok.</response>
        /// <response code="400">Fill all required fields.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost("auth")]
        public async Task<ActionResult<TokenReturned>> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var user = await _user.Get(model.Email);
                    return TokenService.GenerateToken(user);
                }
                else if(result.IsLockedOut)
                {
                    return Unauthorized("User is currently locked out.");
                }
                else
                {
                    return Unauthorized("Wrong Credentials.");
                }
            }
            else
            {
                return BadRequest("Fill all required fields.");
            }
        }

        /// <summary>
        /// List all users.
        /// </summary>
        /// <response code="200">Ok.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Any user found.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ApplicationUser>>> All()
        {
            var users = await _user.Get();
            return users == null ? NotFound("Any user found.") : Ok(users);
        } 
    
        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <response code="200">Ok.</response>
        /// <response code="400">Fill all required fields.</response>
        [HttpPost]
        public async Task<ActionResult> Create(User model)
        {
            if(ModelState.IsValid)
            {
                return await _user.Add(model) == true ? Ok("User created successfully.") : BadRequest("Something went wrong, try again.");
            }

            return BadRequest("Fill all required fields.");
        }

        /// <summary>
        /// Select a user.
        /// </summary>
        /// <param name="userEmail">User email.</param>
        /// <response code="200">Ok.</response>
        /// <response code="400">Insert a valid email.</response>
        /// <response code="404">Not Found.</response>
        [HttpGet("{userEmail}")]
        public async Task<ActionResult<ApplicationUser>> Get(string userEmail)
        {
            if(string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("Insert a valid email.");
            }

            var user = await _user.Get(userEmail);
            user.PasswordHash = string.Empty;

            return user == null ? NotFound("User not found.") : Ok(user);
        }

        /// <summary>
        /// Remove a user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <response code="200">Ok.</response>
        /// <response code="400">Insert a valid user id.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<ActionResult<ApplicationUser>> Remove(Guid userId)
        {
            if(Guid.Empty == userId)
            {
                return BadRequest("Insert a valid id.");
            }

            return await _user.Remove(userId) == false ? NotFound("User not found or something went wrong!") : Ok("User has been removed.");
        }
    }
}