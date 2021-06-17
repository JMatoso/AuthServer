using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace jwt_identity_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
                
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .ReadFrom.Configuration(configuration)
                    .WriteTo.Console()
                    .CreateLogger();

                Serilog.Debugging.SelfLog.Enable(msg =>
                    {
                        Debug.Print(msg);
                        Debugger.Break();
                    });
                
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception e)
            {
                Log.Fatal($"Host terminated unexpectedly: {e.Message}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://localhost:44381");
                });
    }
}
