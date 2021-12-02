using jwt_identity_api.Data;
using jwt_identity_api.Data.Repositories;
using jwt_identity_api.Data.Repositories.Interfaces;
using jwt_identity_api.DTO;
using Microsoft.EntityFrameworkCore;

namespace jwt_identity_api.Installers.Services
{
    public class DbInstaller : IServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt => 
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }

    public class RepoInstaller : IServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
           services.AddScoped<IGenericTypesRepo<Recovery>, GenericTypesRepo<Recovery>>();
        }
    }
}