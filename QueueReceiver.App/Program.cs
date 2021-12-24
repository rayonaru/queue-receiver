using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using System.IO;
using System.Reflection;

namespace QueueReceiver.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((hostContext, configuration) =>
                {
                    var workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                    configuration.SetBasePath(workingDirectory)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    configuration.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddNLog();
                    logging.AddConsole();

                    NLog.LogManager.Configuration = new NLogLoggingConfiguration(hostContext.Configuration.GetSection("NLog"));
                })
                .ConfigureServices((hostContext, services) =>
                {

                })
                .UseNLog();
    }
}
