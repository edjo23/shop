using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Services;

namespace Shop.Service.Client
{
    public class CardReadServiceProxy : ServiceProxy<ICardReadService>, ICardReadService
    {
        public CardReadServiceProxy(IServiceClientFactory factory, ICardHandler callbackHandler)
            : base(factory, new InstanceContext(callbackHandler))
        {
        }

        public void Connect()
        {
            InvokeDuplex(s => s.Connect());
        }
    }
}
