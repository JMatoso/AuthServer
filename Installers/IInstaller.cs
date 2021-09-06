using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace jwt_identity_api.Installers
{
    public interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}