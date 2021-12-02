using jwt_identity_api.Models.Request;
using jwt_identity_api.Models.Response;
using jwt_identity_api.Services;
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

        public AuthController(IIdentityService identity)
        {
            _identity = identity;
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
        [HttpPost]
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


    }
}