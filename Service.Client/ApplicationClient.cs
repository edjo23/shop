using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using System.Collections.Generic;

namespace Shop.Service.Client
{
    public class ApplicationClient : HttpClient, IApplicationService
    {
        public ApplicationClient(IUrlProvider urlProvider)
        {
            BaseUrl = urlProvider.GetBaseUrl();
        }

        private readonly string BaseUrl = "http://localhost:5000/api";

        public IEnumerable<Denomination> GetDenominations()
        {
            return Get<List<Denomination>>($"{BaseUrl}/application/denomination");
        }

        public IEnumerable<string> GetImageList()
        {
            return Get<List<string>>($"{BaseUrl}/application/image");
        }

        public byte[] GetImage(string code)
        {
            return Get<byte[]>($"{BaseUrl}/application/image/{code}");
        }

        public string[] GetText(string code)
        {
            return Get<string[]>($"{BaseUrl}/application/text/{code}");
        }
    }
}
