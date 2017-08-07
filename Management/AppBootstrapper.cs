using Autofac;
using Caliburn.Micro.Autofac;
using Shop.Contracts.Services;
//using Shop.Business.Managers;
using Shop.Management.ViewModels;
using Shop.Service.Client;

namespace Shop.Management
{
    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        protected override void ConfigureContainer(Autofac.ContainerBuilder builder)
        {
            //builder.RegisterType<ProductManager>();
            //builder.RegisterType<CustomerManager>();
            //builder.RegisterType<InvoiceManager>();
            //builder.RegisterType<DiscountManager>();

            builder.RegisterType<UrlProvider>().As<IUrlProvider>();

            builder.RegisterType<ProductClient>().As<IProductService>();
            builder.RegisterType<CustomerClient>().As<ICustomerService>();
            builder.RegisterType<InvoiceClient>().As<IInvoiceService>();
            builder.RegisterType<ReceiptClient>().As<IReceiptService>();
            builder.RegisterType<DiscountClient>().As<IDiscountService>();

            builder.RegisterType<MenuItemViewModel<SummaryViewModel>>();
            builder.RegisterType<MenuItemViewModel<ProductsViewModel>>();
            builder.RegisterType<MenuItemViewModel<CustomersViewModel>>();
            builder.RegisterType<MenuItemViewModel<DiscountsViewModel>>();

            base.ConfigureContainer(builder);
        }

    }  
}
