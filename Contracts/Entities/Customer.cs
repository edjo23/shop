using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }
    }
}
