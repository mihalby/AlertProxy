using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AlertProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((ctx, cfg) => cfg.ClearProviders())
                .UseSerilog((ctx, cfg) =>
                {
                    var config = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "cfg", "serilog.json"), optional: false, reloadOnChange: true)
                     .Build();
                    cfg.ReadFrom.Configuration(config);

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var config = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "cfg", "settings.json"), optional: false, reloadOnChange: true)
                       .AddCommandLine(args)
                       .Build();

                    webBuilder
                      .ConfigureAppConfiguration((builderContext, config) =>
                      {
                          IHostEnvironment env = builderContext.HostingEnvironment;
                          config
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "cfg", "settings.json"), optional: false, reloadOnChange: true)
                          .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "cfg", "targets.json"), optional: false, reloadOnChange: true)
                          .AddCommandLine(args);
                      }
                      )
                      //.UseConfiguration(config)

                      .ConfigureKestrel((context, options) =>
                      {

                          options.Listen(IPAddress.Any, int.Parse(config.GetSection("SSL")["port"]), listenOptions =>
                          {
                              listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                              if (config.GetSection("SSL")["sertificateName"].Trim() != "")
                                  listenOptions.UseHttps(Path.Combine(AppContext.BaseDirectory, "cfg", config.GetSection("SSL")["sertificateName"]), config.GetSection("SSL")["password"]);

                          });
                      })
                      .UseStartup<Startup>();
                });
    }
}
