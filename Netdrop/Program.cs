using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Netdrop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseKestrel(options =>
                    //{
                    //    options.Listen(IPAddress.Any, 5009, listenOptions => {
                    //        listenOptions.UseHttps("cert.pfx", "1");
                    //    });

                    //    options.Listen(IPAddress.Loopback, 5009, listenOptions => {
                    //        listenOptions.UseHttps("localhost.pfx", "1");
                    //    });

                    //    options.Listen(IPAddress.Any, 5008);

                    //});
                });
    }
}
