using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Contracts.Services
{
    [ServiceContract]
    public interface IApplicationService
    {
        [OperationContract]
        IEnumerable<Denomination> GetDenominations();

        [OperationContract]
        IEnumerable<string> GetImageList();

        [OperationContract]
        byte[] GetImage(string code);

        [OperationContract]
        string[] GetText(string code);
    }
}
