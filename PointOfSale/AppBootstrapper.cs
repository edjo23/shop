using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Business.Services;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using Shop.Business.Managers;
using Shop.Contracts.Services;
using Shop.PointOfSale.ViewModels;
using Shop.PointOfSale.Services;

namespace Shop.PointOfSale
{
    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {       
        protected override void ConfigureContainer(Autofac.ContainerBuilder builder)
        {
            builder.RegisterType<ProductManager>();
            builder.RegisterType<CustomerManager>();
            builder.RegisterType<InvoiceManager>();

            builder.RegisterType<ProductService>().As<IProductService>();
            builder.RegisterType<CustomerService>().As<ICustomerService>();
            builder.RegisterType<InvoiceService>().As<IInvoiceService>();

            builder.RegisterType<ScreenCoordinator>().InstancePerLifetimeScope();

            base.ConfigureContainer(builder);
        }
    }
}
