using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MineHealthAPI
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
                    //webBuilder.UseHttpSys(options =>
                    //{
                    //    options.UrlPrefixes.Add("http://localhost:9000");
                    //    options.UrlPrefixes.Add("http://172.30.1.127:44336");
                    //    options.UrlPrefixes.Add("https://172.30.1.127:44");
                    //});
                    

                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.ListenAnyIP(44336);
                    });

                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseHttpSys(options =>
                    //{
                    //    options.UrlPrefixes.Add("http://localhost:9000");
                    //    options.UrlPrefixes.Add("http://172.30.1.127:44336");
                    //    options.UrlPrefixes.Add("https://172.30.1.127:44");
                    //});

                });
    }
}
