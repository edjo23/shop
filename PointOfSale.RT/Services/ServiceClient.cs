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

        public ServiceClient(string address)
            : base(new BasicHttpBinding(), new EndpointAddress(address))
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
