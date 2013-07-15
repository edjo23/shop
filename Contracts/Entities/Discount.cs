using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Entities
{
    public class Discount
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
    }
}
