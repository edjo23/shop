using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service.Client
{
    public class ServiceClient<T> : ClientBase<T> where T : class
    {
        public new T Channel
        {
            get
            {
                return base.Channel;
            }
        }
    }
}
