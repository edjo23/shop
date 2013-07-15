using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using Shop.Business.Managers;
using Shop.Business.Services;
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

            builder.RegisterType<ProductService>().As<IProductService>();
            builder.RegisterType<CustomerService>().As<ICustomerService>();
            builder.RegisterType<InvoiceService>().As<IInvoiceService>();
            builder.RegisterType<DiscountService>().As<IDiscountService>();

            builder.RegisterType<MenuItemViewModel<SummaryViewModel>>();
            builder.RegisterType<MenuItemViewModel<ProductsViewModel>>();
            builder.RegisterType<MenuItemViewModel<CustomersViewModel>>();
            builder.RegisterType<MenuItemViewModel<DiscountsViewModel>>();

            base.ConfigureContainer(builder);
        }

    }  
}
