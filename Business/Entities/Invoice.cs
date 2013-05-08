using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public Guid CustomerId { get; set; }
    }
}
