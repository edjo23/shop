using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Contracts.Services
{
    public interface IDiscountService
    {
        IEnumerable<Discount> GetDiscounts();
        DiscountModel GetDiscountModel(Guid id);
        DiscountModel InsertDiscountModel(DiscountModel entity);
        DiscountModel UpdateDiscountModel(DiscountModel entity);
        IEnumerable<DiscountProduct> GetDiscountProductsByCustomerId(Guid customerId);
    }
}
