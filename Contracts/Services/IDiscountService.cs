using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Contracts.Services
{
    [ServiceContract]
    public interface IDiscountService
    {
        [OperationContract]
        IEnumerable<Discount> GetDiscounts();

        [OperationContract]
        DiscountModel GetDiscountModel(Guid id);

        [OperationContract]
        DiscountModel InsertDiscountModel(DiscountModel entity);

        [OperationContract]
        DiscountModel UpdateDiscountModel(DiscountModel entity);

        [OperationContract]
        IEnumerable<DiscountProduct> GetDiscountProductsByCustomerId(Guid customerId);
    }
}
