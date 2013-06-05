using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Shop.Management.Messages
{
    public class DeactivateItem
    {
        public Screen Item { get; set; }
    }
}
