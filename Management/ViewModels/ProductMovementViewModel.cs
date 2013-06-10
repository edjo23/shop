using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;

namespace Shop.Management.ViewModels
{
    public class ProductMovementViewModel : PropertyChangedBase
    {
        public ProductMovementViewModel(ProductMovement movement)
        {
            Movement = movement;
        }

        public ProductMovement Movement { get; set; }

        public string MovementTypeText
        {
            get
            {
                if (Movement != null)
                {

                    switch (Movement.MovementType)
                    {
                        case ProductMovementType.Receipt:
                            return "Receipt";
                        case ProductMovementType.Correction:
                            return "Correction";
                        case ProductMovementType.Invoice:
                            return "Invoivce";
                        case ProductMovementType.Adjustment:
                            return "Adjustment";
                    }
                }

                return null;
            }
        }
    }
}
