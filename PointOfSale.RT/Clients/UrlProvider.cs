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
            return Windows.Storage.ApplicationData.Current.LocalSettings.Values["HostAddress"] as string;
        }
    }
}
