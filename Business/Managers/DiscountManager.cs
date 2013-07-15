using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Business.Database;
using Shop.Contracts.Entities;

namespace Shop.Business.Managers
{
    public class DiscountManager
    {
        public Tuple<IEnumerable<Discount>, IEnumerable<DiscountProduct>, IEnumerable<DiscountCustomer>> GetDiscounts()
        {
            return Tuple.Create(Extensions.SelectAll<Discount>(), Extensions.SelectAll<DiscountProduct>(), Extensions.SelectAll<DiscountCustomer>());
        }
    }
}
