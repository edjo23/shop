using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service.Client
{
    public class ServiceClient<T> : ClientBase<T>, IDisposable
        where T : class
    {
        public ServiceClient()
        {
        }

        public ServiceClient(IServiceClientConfiguration configuration)
            : base(configuration.Binding, new EndpointAddress(String.Format(configuration.EndpointAddressFormatString, configuration.Host, typeof(T).Name.Substring(1))))
        {
        }

        public new T Channel
        {
            get
            {
                return base.Channel;
            }
        }

        public void Dispose()
        {
        }
    }
}
