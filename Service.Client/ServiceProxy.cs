using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Client
{
    public abstract class ServiceProxy<TService> where TService : class
    {
        protected TResult Invoke<TResult>(Func<TService, TResult> call)
        {
            using (var client = new ServiceClient<TService>())
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
            using (var client = new ServiceClient<TService>())
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
