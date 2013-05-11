using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Entities
{
    public static class ProductMovementType
    {
        public const int Receipt = 1;
        public const int Invoice = 2;
        public const int Correction = 3;
    }

    public class ProductMovement
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int MovementType { get; set; }

        public int Quantity { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public Guid? SourceId { get; set; }

        public int? SourceItemNumber { get; set; }
    }
}
