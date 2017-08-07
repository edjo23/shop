using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shop.Business.Database;
using Shop.Business.Managers;
using Shop.Business.Services;
using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // services.AddSingleton<IConfigurationRoot>(o => BuildConfiguration());
            services.AddSingleton<IConfiguration>(o => BuildConfiguration());

            services.AddTransient<ApplicationManager>();
            services.AddTransient<ProductManager>();
            services.AddTransient<CustomerManager>();
            services.AddTransient<InvoiceManager>();
            services.AddTransient<DiscountManager>();

            services.AddTransient<IConnectionProvider, ConnectionProvider>();

            services.AddTransient<IApplicationService, ApplicationService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IInvoiceService, InvoiceService>();
            services.AddTransient<IDiscountService, DiscountService>();

            services.AddCors();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            loggerFactory.AddConsole(configuration.GetSection("Logging"));

            app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseMvc();           

            var logger = loggerFactory.CreateLogger("General");
            logger.LogInformation("Starting...");
        }

        private IConfigurationRoot BuildConfiguration()
        {
            var config = new ConfigurationBuilder();
            config.SetBasePath(AppContext.BaseDirectory);
            config.AddJsonFile(@".\config.json", true);

            return config.Build();
        }
    }
}
