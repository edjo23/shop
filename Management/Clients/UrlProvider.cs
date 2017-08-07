using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Client
{
    public class UrlProvider : IUrlProvider
    {
        public string GetBaseUrl()
        {
            return "http://localhost:5000/api";
        }
    }
}
