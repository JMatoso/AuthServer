using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jwt_identity_api.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace jwt_identity_api.Data.Repositories
{
    public interface IUserApp
    {
        Task<bool> Add(User model);
        Task<List<ApplicationUser>> Get();
        Task<ApplicationUser> Get(Guid userId);
        Task<ApplicationUser> Get(string userEmail);
        Task<bool> Remove(Guid userId);
    }

    public class UserApp : IUserApp
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserApp> _logger;

        public UserApp(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserApp> logger
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<List<ApplicationUser>> Get() => await _userManager.Users.ToListAsync();

        public async Task<ApplicationUser> Get(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user == null ? null : user;
        }

        public async Task<ApplicationUser> Get(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            return user == null ? null : user;
        }

        public async Task<bool> Add(User model)
        {
            if (await Get(model.Email) != null)
            {
                return false;
            }

            var user = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.FirstName,
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                UserName = model.Email,
                NormalizedUserName = model.Email.ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = true,
                Role = model.Role,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            await Seed.Role(_roleManager);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var result2 = await _userManager.AddToRoleAsync(user, model.Role);

                if (!result2.Succeeded)
                {
                    IdentityLogErrors(result2.Errors);
                    await _userManager.DeleteAsync(user);
                }

                return true;
            }

            IdentityLogErrors(result.Errors);
            return false;
        }

        public async Task<bool> Remove(Guid userId)
        {
            var user = await Get(userId);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);

            IdentityLogErrors(result.Errors);
            return result.Succeeded == true ? true : false;
        }

        private void IdentityLogErrors(IEnumerable<IdentityError> result)
        {
            if (result != null)
            {
                foreach (var item in result)
                {
                    _logger.LogWarning(item.Description);
                }
            }
        }
    }
}