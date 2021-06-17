using System;
using System.IO;
using System.Reflection;
using System.Text;
using jwt_identity_api.Data;
using jwt_identity_api.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace jwt_identity_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

	        services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials());
            });

            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserApp, UserApp>();
            services.AddScoped<IBookApp, BookApp>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<AppDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;
            });

            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSettings.SecretKey)),
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true
                };
            });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(2, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Authentication using JWT and AspNetCore Identity", Version = "v2" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "JWT-Identity-API v2"));
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
