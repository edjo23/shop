using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.PointOfSale.Services;

namespace Shop.PointOfSale.ViewModels
{
    public class PayViewModel : Screen
    {
        public PayViewModel(ScreenCoordinator screenCoordinator)
        {
            ScreenCoordinator = screenCoordinator;
            DisplayName = "Pay Account";
        }

        private readonly ScreenCoordinator ScreenCoordinator;

        public decimal Total
        {
            get
            {
                return 0.0m;
            }
        }

        public void CompletePayment()
        {
            ScreenCoordinator.GoToHome();
        }
    }
}
