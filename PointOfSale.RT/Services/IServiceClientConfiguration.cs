using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Service.Client
{
    public interface IServiceClientConfiguration
    {
        Binding Binding { get; }
        string Host { get; set; }
        string EndpointAddressFormatString { get; set; }
    }

    public class BasicHttpServiceClientConfiguration : IServiceClientConfiguration
    {
        public Binding Binding
        {
            get
            {
                return new BasicHttpBinding() { MaxReceivedMessageSize = 1000000 }; 
            }
        }

        public string Host { get; set; }

        public string EndpointAddressFormatString { get; set; }
    }
}
