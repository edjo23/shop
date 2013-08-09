using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using Service.Client;
using Shop.Business.Managers;
using Shop.Contracts.Services;
using Shop.Management.ViewModels;

namespace Shop.Management
{
    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        protected override void ConfigureContainer(Autofac.ContainerBuilder builder)
        {
            builder.RegisterType<ProductManager>();
            builder.RegisterType<CustomerManager>();
            builder.RegisterType<InvoiceManager>();
            builder.RegisterType<DiscountManager>();

            builder.RegisterType<ProductServiceProxy>().As<IProductService>();
            builder.RegisterType<CustomerServiceProxy>().As<ICustomerService>();
            builder.RegisterType<InvoiceServiceProxy>().As<IInvoiceService>();
            builder.RegisterType<DiscountServiceProxy>().As<IDiscountService>();

            builder.RegisterType<MenuItemViewModel<SummaryViewModel>>();
            builder.RegisterType<MenuItemViewModel<ProductsViewModel>>();
            builder.RegisterType<MenuItemViewModel<CustomersViewModel>>();
            builder.RegisterType<MenuItemViewModel<DiscountsViewModel>>();

            base.ConfigureContainer(builder);
        }

    }  
}
