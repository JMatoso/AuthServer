namespace jwt_identity_api.Installers
{
    public interface IServiceInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }

    public interface IExtensionInstaller
    {
        void InstallExtensions(WebApplication app);
    }
}