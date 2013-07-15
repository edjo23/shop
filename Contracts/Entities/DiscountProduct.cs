using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Entities
{
    public class DiscountProduct
    {
        public Guid Id { get; set; }

        public Guid DiscountId { get; set; }

        public Guid ProductId { get; set; }

        public decimal Discount { get; set; }
    }
}
