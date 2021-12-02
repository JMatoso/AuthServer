using System.Text;
using jwt_identity_api.Data;
using jwt_identity_api.Options;
using jwt_identity_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace jwt_identity_api.Installers.Services
{
    public class AuthenticationInstaller : IServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = new JwtOptions();

            configuration.Bind(nameof(JwtOptions), jwtOptions);
            services.AddSingleton(jwtOptions);

            var facebookOptions = new FacebookOptions();

            configuration.Bind(nameof(FacebookOptions), facebookOptions);
            services.AddSingleton(facebookOptions);

            services.AddScoped<IFacebookAuthService, FacebookAuthService>();
            services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "dataprotectionkeys")))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(31))
                .UseCryptographicAlgorithms(
                    new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.SecretKey)),
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true
            };

            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt => {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.TokenValidationParameters = tokenValidationParameters;
            });
        }
    }
}