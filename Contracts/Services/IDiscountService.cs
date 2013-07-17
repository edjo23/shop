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
        Tuple<IEnumerable<Discount>, IEnumerable<DiscountProduct>, IEnumerable<DiscountCustomer>> GetDiscounts();

        IEnumerable<DiscountModel> GetDiscountModels();
        DiscountModel GetDiscountModel(Guid id);
        DiscountModel UpdateDiscountModel(DiscountModel entity);
    }
}
