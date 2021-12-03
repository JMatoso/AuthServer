using jwt_identity_api.Data;
using jwt_identity_api.Extensions;
using jwt_identity_api.Models.Request;
using jwt_identity_api.Models.Response;
using jwt_identity_api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace jwt_identity_api.Controllers.v1
{
    /// <summary>
    /// Auth.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identity;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(
            IIdentityService identity, 
            UserManager<ApplicationUser> userManager)
        {
            _identity = identity;
            _userManager = userManager;
        }

        /// <summary>
        /// Login.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TokenResponse>> Login([FromBody]LoginRequest loginRequest)
        {
            if(ModelState.IsValid)
            {
                var result = await _identity.LoginAsync(loginRequest);
                return result.Successful ? Ok(result.Token) : StatusCode(result.StatusCode, ErrorReporterProvider.Set(result.Message, result.StatusCode, result.Errors));
            }

            return BadRequest();
        }

        /// <summary>
        /// Login with Facebook.
        /// </summary>
        /// <param name="accessToken">Facebook access token</param>
        [HttpPost("facebook")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> LoginwithFacebbok([FromQuery]string accessToken)
        {
            if(ModelState.IsValid)
            {
                var result = await _identity.LogInWithFacebook(accessToken);
                return result.Successful ? Ok(result.Token) : StatusCode(result.StatusCode, ErrorReporterProvider.Set(result.Message, result.StatusCode, result.Errors));
            }

            return BadRequest();
        }

        /// <summary>
        /// Account ativation.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="token">Ativation token.</param>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(405)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]  
        public async Task<ActionResult> AtivateAccount([FromQuery]Guid userId, [FromQuery]string token)
        {
            if(string.IsNullOrEmpty(token)) return BadRequest(ErrorReporterProvider.Set("Fill all required fields.", 400, new()
            {
                {
                    nameof(token),
                    "Required field."
                }
            }));

            if(Guid.Empty == userId) return BadRequest(ErrorReporterProvider.Set("Fill all required fields.", 400, new()
            {
                {
                    nameof(userId),
                    "Required field."
                }
            }));

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if(user is null)
            {
                return NotFound(ErrorReporterProvider.Set("Not found.", 404, new()
                {
                    {
                        nameof(user.Id),
                        "User not found."
                    }
                }));
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded ? Ok() : StatusCode(500, ErrorReporterProvider.Set("Something went wrong while ativating account.", 500, GeneralExtensions.SetErrors(result.Errors)));
        }    

        /// <summary>
        /// Reset password, step 1, email.
        /// </summary>
        /// <param name="email">User email.</param>
        [HttpPost("password/{email}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(405)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]  
        public async Task<ActionResult> ResetPasswordStepOne(string email)
        {
            if(string.IsNullOrEmpty(email)) return BadRequest(ErrorReporterProvider.Set("Fill all required fields.", 400, new()
            {
                {
                    nameof(email),
                    "Required field."
                }
            }));

            var result = await _identity.ForgotPasswordAsync(email);
            return result.Successful ? Ok() : StatusCode(result.StatusCode, ErrorReporterProvider.Set(result.Message, result.StatusCode, result.Errors));
        }

        /// <summary>
        /// Reset password, step 2, verify code.
        /// </summary>
        /// <param name="code">Password redifinition code.</param>
        [HttpPut("password/{code}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(405)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]  
        public async Task<ActionResult> ResetPasswordStepTwo(string code)
        {
            if(string.IsNullOrEmpty(code)) return BadRequest(ErrorReporterProvider.Set("Fill all required fields.", 400, new()
            {
                {
                    nameof(code),
                    "Required field."
                }
            }));

            var result = await _identity.GenerateResetPasswordTokenAsync(code);
            return result.Successful ? Ok(new
            {
                Token = result.Token
            }) : StatusCode(result.StatusCode, ErrorReporterProvider.Set(result.Message, result.StatusCode, result.Errors));
        }

        /// <summary>
        /// Change password, step 3.
        /// </summary>
        [HttpPut("password/reset")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(405)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]  
        public async Task<ActionResult> ChangePasswordStepThree([FromBody]ResetPasswordRequest resetPasswordRequest)
        {
            if(ModelState.IsValid)
            {
                var result = await _identity.ResetPasswordAsync(resetPasswordRequest);
                return result.Successful ? Ok() : StatusCode(result.StatusCode, ErrorReporterProvider.Set(result.Message, result.StatusCode, result.Errors));
            }

            return BadRequest();
        }        
    }
}