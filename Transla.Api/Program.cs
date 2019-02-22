using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;

namespace Transla.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("##################################################################");
            Console.WriteLine("dotnet.exe process id: #" + Process.GetCurrentProcess().Id);
            Console.WriteLine("##################################################################");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
