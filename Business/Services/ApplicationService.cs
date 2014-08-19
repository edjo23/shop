using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Shop.Business.Managers;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.Business.Services
{
    public class ApplicationService : IApplicationService
    {
        public ApplicationService(ApplicationManager manager)
        {
            Manager = manager;
        }

        private ApplicationManager Manager { get; set; }

        public IEnumerable<Denomination> GetDenominations()
        {
            return Manager.GetDenominations();
        }

        public IEnumerable<string> GetImageList()
        {
            return Manager.GetImageList();
        }

        public byte[] GetImage(string code)
        {
            return Manager.GetImage(code);
        }

        public string[] GetText(string code)
        {
            return Manager.GetText(code);
        }
    }
}
