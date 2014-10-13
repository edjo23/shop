using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Messages
{
    public class CardInserted
    {
        public string CardId { get; set; }
    }

    public class InvalidCardInserted
    {        
    }
}
