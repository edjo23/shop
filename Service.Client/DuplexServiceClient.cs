using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Client
{
    public class DuplexServiceClient<T> : DuplexClientBase<T>, IDisposable
        where T : class
    {
        public DuplexServiceClient(InstanceContext callbackInstance)
            : base(callbackInstance)
        {
        }

        public DuplexServiceClient(InstanceContext callbackInstance, Binding binding, EndpointAddress address)
            : base(callbackInstance, binding, address)
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
