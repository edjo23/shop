using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Entities
{
    public class DiscountModel
    {
        public Discount Discount { get; set; }

        public IEnumerable<DiscountProduct> Products { get; set; }

        public IEnumerable<DiscountCustomer> Customers { get; set; }
    }
}
