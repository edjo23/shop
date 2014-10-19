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

        public DuplexServiceClient<TChannel> GetDuplexClient<TChannel>(InstanceContext callbackInstance) where TChannel : class
        {
            return new DuplexServiceClient<TChannel>(callbackInstance, GetDuplexBinding<TChannel>(), GetAddress<TChannel>());
        }

        public string Host { get; set; }

        public string EndpointAddressFormatString { get; set; }

        public Dictionary<Type, string> EndpointAddressFormatStrings = new Dictionary<Type, string>();

        public Binding GetBinding<TChannel>()
        {
            return new BasicHttpBinding() { MaxReceivedMessageSize = 1000000 };
        }

        public Binding GetDuplexBinding<TChannel>()
        {
            return new NetHttpBinding() { MaxReceivedMessageSize = 1000000, ReceiveTimeout = TimeSpan.FromDays(7), SendTimeout = TimeSpan.FromDays(7) };
        }

        public EndpointAddress GetAddress<TChannel>()
        {
            var format = EndpointAddressFormatStrings.ContainsKey(typeof(TChannel)) ? EndpointAddressFormatStrings[typeof(TChannel)] : EndpointAddressFormatString;

            return new EndpointAddress(String.Format(format, Host, typeof(TChannel).Name.StartsWith("I") ? typeof(TChannel).Name.Substring(1) : typeof(TChannel).Name));
        }
    }
}
