using System.Net;
using AutoMapper;
using jwt_identity_api.Data;
using jwt_identity_api.Extensions;
using jwt_identity_api.Models.Request;
using jwt_identity_api.Models.Response;
using jwt_identity_api.Models.VModels;
using jwt_identity_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace jwt_identity_api.Controllers.v1
{
    /// <summary>
    /// Accounts management.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/account")]
    public class AccountController : ControllerBase
    { 
        private readonly IMapper _mapper;
        private readonly IIdentityService _identity;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            IIdentityService identity,
            UserManager<ApplicationUser> userManager, 
            IMapper mapper)
        {
            _identity = identity;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Get user.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserVM>> GetUserInfo()
        {
            var userId = HttpContext.GetDataFromJwtToken();
            if(userId is null) return Unauthorized(ErrorReporterProvider.Set("Login first.", 401, null));

            var user = await _userManager.FindByIdAsync(userId);
            if(user is null) return NotFound(ErrorReporterProvider.Set("Not found.", 404, new()
            {
                {
                    nameof(userId),
                    "User not found."
                }
            }));

            return Ok(_mapper.Map<UserVM>(user));
        }

        /// <summary>
        /// Register user.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TokenResponse>> RegisterUser([FromBody]UserRequest userRequest)
        {
            if(ModelState.IsValid)
            {
                var userId = Guid.NewGuid();

                var result = await _identity.RegisterAsync(new ApplicationUser()
                {
                    Id = userId.ToString(),
                    Name = userRequest.Name,
                    PhoneNumber = userRequest.PhoneNumber,
                    Email = userRequest.Email,
                    NormalizedEmail = userRequest.Email?.ToUpper(),
                    UserName = userRequest.Email,
                    NormalizedUserName = userRequest.Email?.ToUpper(),
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Role = Seed.Roles[1],
                    IsThirtyUser = false,
                    HasPasswordChanged = false,
                    Created = DateTime.Now

                }, userRequest.Password, true);

                return result.Successful ? Created(HttpContext.GetDomainPath() + userId, result.Token) : StatusCode(result.StatusCode, ErrorReporterProvider.Set(result.Message, result.StatusCode, result.Errors));
            }

            return BadRequest();
        }

        /// <summary>
        /// Update user.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserRequest userRequest)
        {
            var userId = HttpContext.GetDataFromJwtToken();
            if(userId is null) return Unauthorized(ErrorReporterProvider.Set("Login first.", 401, null));

            var user = await _userManager.FindByIdAsync(userId);
            if(user is null) return NotFound(ErrorReporterProvider.Set("Not found.", 404, new()
            {
                {
                    nameof(userId),
                    "User not found."
                }
            }));

            if(ModelState.IsValid)
            {
                user.Name = userRequest.Name;
                user.Email = userRequest.Email;
                user.NormalizedEmail = userRequest.Email?.ToUpper();
                user.UserName = user.Email;
                user.NormalizedUserName = userRequest.Email?.ToUpper();
                user.PhoneNumber = user.PhoneNumber;
                user.SecurityStamp = Guid.NewGuid().ToString();

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded ? Ok() : StatusCode(500, ErrorReporterProvider.Set("Something went wrong.", 500, GeneralExtensions.SetErrors(result.Errors)));
            }

            return BadRequest();
        }

        /// <summary>
        /// Change user's password.
        /// </summary>
        [HttpPut("password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequest passwordRequest)
        {
            var userId = HttpContext.GetDataFromJwtToken();
            if(userId is null) return Unauthorized(ErrorReporterProvider.Set("Login first.", 401, null));

            if(ModelState.IsValid)
            {
                var result = await _identity.ResetPasswordAsync(Guid.Parse(userId), passwordRequest.OldPassword, passwordRequest.Password);
                return result.Successful ? Ok() : StatusCode(result.StatusCode, ErrorReporterProvider.Set(result.Message, result.StatusCode, result.Errors));
            }

            return BadRequest();
        }
    }
}