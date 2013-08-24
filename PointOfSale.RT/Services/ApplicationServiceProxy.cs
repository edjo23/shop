using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Service.Client
{
    public class ApplicationServiceProxy : ServiceProxy<IApplicationService>, IApplicationService
    {
        public ApplicationServiceProxy(IServiceClientConfiguration configuration)
            : base(configuration)
        {
        }

        public IEnumerable<Denomination> GetDenominations()
        {
            return Invoke(s => s.GetDenominations());
        }

        public IEnumerable<string> GetImageList()
        {
            return Invoke(s => s.GetImageList());
        }

        public byte[] GetImage(string code)
        {
            return Invoke(s => s.GetImage(code));
        }
    }
}
