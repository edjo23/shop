using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Business.Managers;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.Business.Services
{
    public class DiscountService : IDiscountService
    {
        public DiscountService (DiscountManager manager)
        {
            Manager = manager;
        }

        private DiscountManager Manager { get; set; }

        public Tuple<IEnumerable<Contracts.Entities.Discount>, IEnumerable<Contracts.Entities.DiscountProduct>, IEnumerable<Contracts.Entities.DiscountCustomer>> GetDiscounts()
        {
            return Manager.GetDiscounts();
        }

        public IEnumerable<DiscountModel> GetDiscountModels()
        {
            return Manager.GetDiscountModels();
        }

        public DiscountModel GetDiscountModel(Guid id)
        {
            return Manager.GetDiscountModel(id);
        }

        public DiscountModel InsertDiscountModel(DiscountModel entity)
        {
            return Manager.InsertDiscountModel(entity);
        }

        public DiscountModel UpdateDiscountModel(DiscountModel entity)
        {
            return Manager.UpdateDiscountModel(entity);
        }
    }
}
