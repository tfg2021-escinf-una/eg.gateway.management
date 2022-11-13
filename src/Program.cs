using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.IO;
using EG.Gateway.Microservice.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EG.Gateway.Microservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                { 
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddOcelot($"./Routes/{hostingContext.HostingEnvironment.EnvironmentName}/", hostingContext.HostingEnvironment)
                        .AddEnvironmentVariables();
                        
                })
                .ConfigureServices(services =>
                {
                    services.AddJwtAuthentication();
                    services.AddOcelot();
                    services.AddCors();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                })
                .Configure(app  =>
                {
                    app.UseCors(x => x
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .SetIsOriginAllowed(origin => true)
                     .AllowCredentials());
                    app.UseOcelot().Wait();
                });
    }
}
