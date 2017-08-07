using Microsoft.AspNetCore.Hosting;
using SimpleInjector;
using System;

namespace Core.Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Server...");

            var program = new Program();
            var host = program.CreateHost();

            host.Run();
        }

        private IWebHost Host;

        public IWebHost CreateHost()
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();
        }
    }
}