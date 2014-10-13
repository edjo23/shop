using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Messages;

namespace Shop.Contracts.Services
{   
    public interface ICardHandler
    {
        [OperationContract(IsOneWay = true)]
        void HandleCardInserted(CardInserted message);

        [OperationContract(IsOneWay = true)]
        void HandleInvalidCardInserted(InvalidCardInserted message);
    }

    [ServiceContract(CallbackContract = typeof(ICardHandler))]    
    public interface ICardReadService
    {
        [OperationContract(IsOneWay = true)]
        void Connect();
    }

    [ServiceContract]
    public interface ICardWriteService
    {
        [OperationContract]
        string Write();
    }
}
