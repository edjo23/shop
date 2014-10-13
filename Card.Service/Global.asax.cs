using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Autofac;
using Autofac.Integration.Wcf;
using Caliburn.Micro;
using Card.Service.Business;
using log4net;
using Shop.Contracts.Services;

namespace Card.Service
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();

            var log = LogManager.GetLogger("General");
            log.Info("Application starting");

            var builder = new ContainerBuilder();

            builder.RegisterInstance<ILog>(log);
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();

            builder.RegisterType<CardReader>().As<ICardReader>().InstancePerLifetimeScope(); ;
            builder.RegisterType<CardWriter>().As<ICardWriter>().InstancePerLifetimeScope(); ;

            builder.RegisterType<CardReadService>().As<ICardReadService>();
            builder.RegisterType<CardWriteService>().As<ICardWriteService>();

            AutofacHostFactory.Container = builder.Build();
        }
    }
}