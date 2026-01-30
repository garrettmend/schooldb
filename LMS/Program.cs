using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LMS
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }


    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureKestrel((context, options) =>
           {
             // If a certificate PFX is provided via environment, use it for HTTPS.
             // Otherwise, do not configure TLS (platform like Railway handles TLS termination).
             var certPath = Environment.GetEnvironmentVariable("HTTPS_PFX_PATH");
             var certPassword = Environment.GetEnvironmentVariable("HTTPS_PFX_PASSWORD");

             if (!string.IsNullOrEmpty(certPath) && File.Exists(certPath))
             {
               // Listen on HTTP (port 80) and HTTPS (port 443) using provided cert
               options.Listen(IPAddress.Any, 80);
               options.Listen(IPAddress.Any, 443, listenOptions =>
               {
                 listenOptions.UseHttps(certPath, certPassword);
               });
             }
             else
             {
               // No local certificate found - listen on the port supplied by the environment (Railway sets PORT)
               var portEnv = Environment.GetEnvironmentVariable("PORT");
               int port = 80;
               if (!string.IsNullOrEmpty(portEnv) && int.TryParse(portEnv, out var p))
               {
                 port = p;
               }
               options.Listen(IPAddress.Any, port);
             }
           });

  }
}
