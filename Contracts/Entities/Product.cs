using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Group { get; set; }

        public string Description { get; set; }

        public decimal Cost { get; set; }

        public decimal Price { get; set; }

        public int QuantityOnHand { get; set; }
    }
}
