using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Client
{
    public abstract class ServiceProxy<TService> where TService : class
    {
        public ServiceProxy(IServiceClientFactory factory)
        {
            Factory = factory;
        }

        protected readonly IServiceClientFactory Factory;

        protected TResult Invoke<TResult>(Func<TService, TResult> call)
        {
            using (var client = Factory.GetClient<TService>())
            {
                try
                {
                    return call(client.Channel);
                }
                catch
                {
                    client.Abort();
                    throw;
                }
            }
        }

        protected void Invoke(Action<TService> call)
        {
            using (var client = Factory.GetClient<TService>())
            {
                try
                {
                    call(client.Channel);
                }
                catch
                {
                    client.Abort();
                    throw;
                }
            }
        }        
    }
}
