using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Services;

namespace Shop.Service.Client
{
    public class CardWriteServiceProxy: ServiceProxy<ICardWriteService>, ICardWriteService
    {
        public CardWriteServiceProxy(IServiceClientFactory factory)
            : base(factory)
        {
        }

        public string Write()
        {
            return Invoke(o => o.Write());
        }
    }
}
