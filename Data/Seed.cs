using Microsoft.AspNetCore.Identity;

namespace jwt_identity_api.Data
{
    public static class Seed
    {
        public static string[] Roles = new string[]
        { 
            "ROLE_ADMIN", 
            "ROLE_USER"
        };

        public static async Task ApplyRole(this RoleManager<IdentityRole> _roleManager)
        {
            List<IdentityRole> roles = new List<IdentityRole>();

            foreach (var item in Roles) roles.Add(new IdentityRole(item));  
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