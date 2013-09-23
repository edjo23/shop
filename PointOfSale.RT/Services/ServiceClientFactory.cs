using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Shop.Service.Client;

namespace PointOfSale.RT.Services
{
    public class WindowsStoreServiceClientFactory : IServiceClientFactory
    {
        public ServiceClient<TChannel> GetClient<TChannel>()
            where TChannel : class
        {
            return new ServiceClient<TChannel>(GetBinding<TChannel>(), GetAddress<TChannel>());
        }

        public string Host { get; set; }

        public string EndpointAddressFormatString { get; set; }

        public Binding GetBinding<TChannel>()
        {
            return new BasicHttpBinding() { MaxReceivedMessageSize = 1000000 };
        }

        public EndpointAddress GetAddress<TChannel>()
        {
            return new EndpointAddress(String.Format(EndpointAddressFormatString, Host, typeof(TChannel).Name.StartsWith("I") ? typeof(TChannel).Name.Substring(1) : typeof(TChannel).Name));
        }
    }
}
