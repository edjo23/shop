using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Messages;

namespace Shop.Contracts.Services
{
    public interface ICardHandler
    {
        void HandleKeepAlive();

        void HandleCardInserted(CardInserted message);

        void HandleInvalidCardInserted(InvalidCardInserted message);
    }

    public interface ICardReadService
    {
        void Connect();
    }

    public interface ICardWriteService
    {
        string Write();
    }
}