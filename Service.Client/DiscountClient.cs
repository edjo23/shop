using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Client
{
    public class DiscountClient : HttpClient, IDiscountService
    {
        public DiscountClient(IUrlProvider urlProvider)
        {
            BaseUrl = urlProvider.GetBaseUrl();
        }

        private readonly string BaseUrl = "http://localhost:5000/api";

        public IEnumerable<Discount> GetDiscounts()
        {
            return Get<List<Discount>>($"{BaseUrl}/discount");
        }

        public DiscountModel GetDiscountModel(Guid id)
        {
            return Get<DiscountModel>($"{BaseUrl}/discount/{id.ToString()}/model");
        }

        public DiscountModel InsertDiscountModel(DiscountModel entity)
        {
            return Post<DiscountModel, DiscountModel>($"{BaseUrl}/discount/{entity.Discount.Id.ToString()}/model", entity);
        }

        public DiscountModel UpdateDiscountModel(DiscountModel entity)
        {
            return Put<DiscountModel, DiscountModel>($"{BaseUrl}/discount/{entity.Discount.Id.ToString()}/model", entity);
        }

        public IEnumerable<DiscountProduct> GetDiscountProductsByCustomerId(Guid customerId)
        {
            return Get<List<DiscountProduct>>($"{BaseUrl}/discount/customer/{customerId.ToString()}/product");
        }
    }
}
