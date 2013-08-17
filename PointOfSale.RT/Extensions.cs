using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.RT
{
    public static class Extensions
    {
        public static bool IsMatch(this string value, string compare)
        {
            return String.Equals(value, compare, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
