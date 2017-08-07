using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Contracts.Services
{
    public interface IApplicationService
    {
        IEnumerable<Denomination> GetDenominations();

        IEnumerable<string> GetImageList();

        byte[] GetImage(string code);

        string[] GetText(string code);
    }
}
