namespace jwt_identity_api.Installers.Extensions
{
    public class ApplicationInstaller : IExtensionInstaller
    {
        public void InstallExtensions(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error-local-development");

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(opt => {
                opt.AllowAnyHeader();
                opt.AllowAnyHeader();
                opt.SetIsOriginAllowed((host) => true);
                opt.AllowCredentials();
            });

            app.UseAuthorization();

            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}