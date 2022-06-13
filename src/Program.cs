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
                    /**
                     * Here you should add every json file that contains routes redirection.
                     * Ocelot will be in charge of redirecting every request.
                     */

                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"./Routes/{hostingContext.HostingEnvironment.EnvironmentName}/identity.json")
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
                     .SetIsOriginAllowed(origin => true) // allow any origin
                     .AllowCredentials()); // allow credentials
                    app.UseOcelot().Wait();
                });
    }
}
