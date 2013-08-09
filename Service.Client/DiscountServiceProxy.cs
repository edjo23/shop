using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Service.Client
{
    public class DiscountServiceProxy : ServiceProxy<IDiscountService>, IDiscountService
    {
        public IEnumerable<Discount> GetDiscounts()
        {
            return Invoke(s => s.GetDiscounts());
        }

        public DiscountModel GetDiscountModel(Guid id)
        {
            return Invoke(s => s.GetDiscountModel(id));
        }

        public DiscountModel InsertDiscountModel(DiscountModel entity)
        {
            return Invoke(s => s.InsertDiscountModel(entity));
        }

        public DiscountModel UpdateDiscountModel(DiscountModel entity)
        {
            return Invoke(s => s.UpdateDiscountModel(entity));
        }

        public IEnumerable<DiscountProduct> GetDiscountProductsByCustomerId(Guid customerId)
        {
            return Invoke(s => s.GetDiscountProductsByCustomerId(customerId));
        }
    }
}
