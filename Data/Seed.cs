using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace jwt_identity_api.Data
{
    public static class Seed
    {
        public static string[] Roles = new string[]{ "Client", "Admin" };
        public static async Task Role(RoleManager<IdentityRole> _roleManager)
        {
            List<IdentityRole> roles = new List<IdentityRole>();

            foreach (var item in Roles)
            {
                roles.Add(new IdentityRole(item));   
            }

            foreach (var item in roles)
            {
                if(!_roleManager.RoleExistsAsync(item.Name).Result)
                {
                    item.NormalizedName = item.Name.ToLower();
                    await _roleManager.CreateAsync(item);
                }
            }
        }
    }
}