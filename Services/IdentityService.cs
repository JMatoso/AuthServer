using System.Text;
using jwt_identity_api.Data;
using jwt_identity_api.Data.Repositories.Interfaces;
using jwt_identity_api.DTO;
using jwt_identity_api.Extensions;
using jwt_identity_api.Models.Request;
using jwt_identity_api.Models.Response;
using jwt_identity_api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace jwt_identity_api.Services
{
    public interface IIdentityService
    {
        Task<Response> ForgotPasswordAsync(string email);
        Task<Response> GenerateResetPasswordTokenAsync(string code);
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<AuthResponse> LogInWithFacebook(string accessToken);
        Task<AuthResponse> RegisterAsync(ApplicationUser user, string password, bool confirmedAccount = true, bool lockOut = false);
        Task<Response> ResetPasswordAsync(Guid userId, string oldPassword, string newPassword);
        Task<Response> ResetPasswordAsync(ResetPasswordRequest passwordRequest);
    }

    public class IdentityService : IIdentityService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly FacebookOptions _facebookOptions;
        private readonly ITokenGeneratorService _tokenService;
        private readonly IGenericTypesRepo<Recovery> _recovery;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ITokenGeneratorService tokenService,
            JwtOptions jwtOptions, RoleManager<IdentityRole> roleManager,
            IFacebookAuthService facebookAuthService,
            FacebookOptions facebookOptions, IGenericTypesRepo<Recovery> recovery)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _jwtOptions = jwtOptions;
            _roleManager = roleManager;
            _facebookAuthService = facebookAuthService;
            _facebookOptions = facebookOptions;
            _recovery = recovery;
        }

        #region LocalSituations
        public async Task<AuthResponse> RegisterAsync(ApplicationUser user, string password, bool confirmedAccount = true, bool lockOut = false)
        {
            if (await _userManager.Users.AnyAsync(u => u.Id == user.Id)) return AuthResponseProvider.Set("Data conflict.", 409, false, new()
            {
                {
                    nameof(user.Id),
                    "Id already chosen."
                }
            });

            if (await _userManager.Users.AnyAsync(u => u.Email == user.Email)) return AuthResponseProvider.Set("Data conflict.", 409, false, new()
            {
                {
                    nameof(user.Email),
                    "Email already chosen."
                }
            });

             if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == user.PhoneNumber)) return AuthResponseProvider.Set("Data conflict.", 409, false, new()
            {
                {
                    nameof(user.PhoneNumber),
                    "PhoneNumber already chosen."
                }
            });

            await Seed.ApplyRole(_roleManager);

            user.EmailConfirmed = confirmedAccount;
            user.Created = DateTime.Now;

            var createdUser = await _userManager.CreateAsync(user, password);
            if (!createdUser.Succeeded)
            {
                var errors = new Dictionary<object, object>();

                foreach (var err in errors) errors[err.Key] = err.Value;
                return AuthResponseProvider.Set("Algo correu mal.", 500, false, errors);
            }

            await _userManager.AddToRoleAsync(user, user.Role);

            if (confirmedAccount) return await LoginAsync(new LoginRequest
            {
                User = user.Email,
                Password = password
            });
            else
            {
                // TODO use transational to send ativation user account code
                // TODO: what happens when user account is disabled

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            }

            return AuthResponseProvider.Set("User has been registered.", 200);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.Email.Equals(loginRequest.User) ||
                    u.PhoneNumber.Equals(loginRequest.User));

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password)) return AuthResponseProvider.Set("Wrong Ceredentials.", 401, false);

            if(user.UserStatus == UserStatus.Blocked) return AuthResponseProvider.Set("User not allowed.", 403, false, new()
            {
                {
                    nameof(loginRequest.User),
                    "Account blocked."
                }
            });

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginRequest.Password, true, true);

            if (result.Succeeded) return AuthResponseProvider.Set("Logged.", 200, true, null, _tokenService.GenerateToken(user.Id, user.Role));
            else if (result.IsNotAllowed) return AuthResponseProvider.Set("User not allowed.", 403, false, new()
            {
                {
                    nameof(loginRequest.User),
                    "Verify your email."
                }
            });
            else if (result.IsLockedOut) return AuthResponseProvider.Set("User tempory blocked.", 403, false);
            else return AuthResponseProvider.Set("Wrong Ceredentials.", 401, false);

        }

        public async Task<Response> ResetPasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return ResponseProvider.Set(404, "Not found.", false, new()
            {
                {
                    nameof(userId),
                    "User not found."
                }
            });

            if (await _userManager.CheckPasswordAsync(user, oldPassword))
            {
                var result = await _userManager
                    .ResetPasswordAsync(user,
                        await _userManager.GeneratePasswordResetTokenAsync(user),
                        newPassword);

                if (!result.Succeeded)
                {
                    var errors = GeneralExtensions.SetErrors(result.Errors);
                    return ResponseProvider.Set(409, "Something went wrong.", false, errors);
                }
                else return ResponseProvider.Set(200, "Ok", true);
            }

            return ResponseProvider.Set(401, "Unauthorized.", false, new()
            {
                {
                    nameof(oldPassword),
                    "Wrong password."
                }
            });
        }
        #endregion

        #region FacebookAuth
        public async Task<AuthResponse> LogInWithFacebook(string accessToken)
        {
            var validatedToken = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);
            if (validatedToken != null && !validatedToken.Data.IsValid) return AuthResponseProvider.Set("Invalid Facebook token.", 401, false, new()
            {
                {
                    nameof(accessToken),
                    "Ivalid token."
                }
            });

            var facebookUserInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);
            var user = await _userManager.FindByEmailAsync(facebookUserInfo?.Email);

            var instantPassword = new SymmetricSecurityKey
                (Encoding.ASCII.GetBytes(_facebookOptions.InstantPassword)).ToString();

            if (user is null)
            {
                return await RegisterAsync(new ApplicationUser
                {
                    Name = $"{facebookUserInfo?.FirstName} {facebookUserInfo?.LastName}",
                    PhoneNumber = "000",
                    Email = facebookUserInfo?.Email,
                    NormalizedEmail = facebookUserInfo?.Email.ToUpper(),
                    NormalizedUserName = facebookUserInfo?.Email.ToUpper(),
                    UserName = facebookUserInfo?.Email,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Role = Seed.Roles[2],
                    FacebookId = facebookUserInfo?.Id,
                    IsThirtyUser = true,
                    HasPasswordChanged = false,
                    Created = DateTime.Now
                }, instantPassword, true);
            }
            else return await LoginAsync(new LoginRequest
            {
                User = user.Email,
                Password = user.IsThirtyUser && user.HasPasswordChanged ? user.PasswordHash : instantPassword
            });
        }
        #endregion

        #region RecoverPassword
        public async Task<Response> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return ResponseProvider.Set(404, "Not found.", false, new()
            {
                {
                    nameof(email),
                    "User Not found."
                }
            });

            var time = DateTime.Now;
            var code = GeneralExtensions.GenerateCode();
            
            // TODO: Save recovery data

            // TODO: Send email to user with the redifinition code.

            return ResponseProvider.Set(200, "");
        }

        public async Task<Response> GenerateResetPasswordTokenAsync(string code)
        {
            var recovery = await _recovery.GetObjectAsync(r => r.Code.Equals(code) && r.IsValid);
            if (recovery is null) return ResponseProvider.Set(404, "Not found.", false, new()
            {
                {
                    nameof(code),
                    "Not found."
                }
            });

            if (recovery.Created >= recovery.ExpireTime)
            {
                recovery.IsValid = false;
                await _recovery.UpdateObjectAsync(recovery);

                return ResponseProvider.Set(409, "Expired code.", false, new()
                {
                    {
                        nameof(code),
                        "Expired."
                    }
                });
            }

            var user = await _userManager.FindByIdAsync(recovery.UserId.ToString());
            if (user is null) return ResponseProvider.Set(404, "Not found.", false, new()
            {
                {
                    nameof(recovery.UserId),
                    "User Not found."
                }
            });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(token)) return ResponseProvider.Set(500, "Something went wrong. Reset Token wasn't generated", false);

            recovery.Token = token;
            await _recovery.UpdateObjectAsync(recovery);

            return ResponseProvider.Set(200, "", true, null, token);
        }

        public async Task<Response> ResetPasswordAsync(ResetPasswordRequest passwordRequest)
        {
            var recovery = await _recovery.GetObjectAsync(r => r.Token.Equals(passwordRequest.Token) && r.IsValid);
            if (recovery is null) return ResponseProvider.Set(404, "Not found.", false, new()
            {
                {
                    nameof(passwordRequest.Token),
                    "Request Not found."
                }
            });

            if (recovery.Created >= recovery.ExpireTime)
            {
                recovery.IsValid = false;
                await _recovery.UpdateObjectAsync(recovery);

                return ResponseProvider.Set(409, "Invalid token.", false, new()
                {
                    {
                        nameof(passwordRequest.Token),
                        "Expired token."
                    }
                });
            }

            var user = await _userManager.FindByIdAsync(recovery.UserId.ToString());
            if (user is null) return ResponseProvider.Set(404, "Not found.", false, new()
            {
                {
                    nameof(recovery.UserId),
                    "User not found."
                }
            });

            var result = await _userManager.ResetPasswordAsync(user, passwordRequest.Token, passwordRequest.Password);
            if (!result.Succeeded) return ResponseProvider.Set(409, "Something went wrong.", false, GeneralExtensions.SetErrors(result.Errors));

            recovery.IsValid = false;
            await _recovery.UpdateObjectAsync(recovery);

            return ResponseProvider.Set(200, "");
        }
        #endregion
    }
}