using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Autofac;
using Autofac.Integration.Wcf;
using Shop.Business.Managers;
using Shop.Business.Services;
using Shop.Contracts.Services;

namespace Shop.Service
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ProductManager>();
            builder.RegisterType<CustomerManager>();
            builder.RegisterType<InvoiceManager>();
            builder.RegisterType<DiscountManager>();

            builder.RegisterType<ProductService>().As<IProductService>();
            builder.RegisterType<CustomerService>().As<ICustomerService>();
            builder.RegisterType<InvoiceService>().As<IInvoiceService>();
            builder.RegisterType<DiscountService>().As<IDiscountService>();

            AutofacHostFactory.Container = builder.Build();
        }
    }
}