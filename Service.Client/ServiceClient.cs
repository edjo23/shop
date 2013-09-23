using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Client
{
    public class ServiceClient<T> : ClientBase<T>, IDisposable
        where T : class
    {
        public ServiceClient()
        {
        }

        public ServiceClient(Binding binding, EndpointAddress address)
            : base(binding, address)
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
