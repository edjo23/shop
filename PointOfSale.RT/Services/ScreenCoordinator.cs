using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Messages;
using PointOfSale.RT.Models;
using PointOfSale.RT.ViewModels;
using Shop.Contracts.Entities;

namespace PointOfSale.RT.Services
{
    public class ScreenCoordinator
    {
        public const int IdleTimeout = 30000;

        public ScreenCoordinator(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            Logger = log4net.LogManager.GetLogger(GetType());
        }

        private readonly IEventAggregator EventAggregator;

        private readonly log4net.ILog Logger;

        private CancellationTokenSource CancellationTokenSource { get; set; }

        public void NavigateToScreen(Screen screen, bool endableIdleTimeout = false)
        {
            StopIdleTimeout();

            EventAggregator.PublishOnCurrentThread(screen);

            if (endableIdleTimeout)
                StartIdleTimeout();
        }

        public void NavigateToHome()
        {
            var screen = IoC.Get<HomeViewModel>();

            NavigateToScreen(screen);
        }

        public void NavigateToCashHome(Customer cashCustomer)
        {
            var screen = IoC.Get<CashHomeViewModel>();
            screen.CashCustomer = cashCustomer;

            NavigateToScreen(screen, true);
        }

        public void NavigateToPinEntry(Customer customer)
        {
            var screen = IoC.Get<PinEntryViewModel>();
            screen.Customer = customer;

            NavigateToScreen(screen, true);
        }

        public void NavigateToCustomer(Customer customer, bool cardSession = false)
        {
            var screen = IoC.Get<CustomerViewModel>();
            screen.Customer = customer;
            screen.CardSession = cardSession;

            NavigateToScreen(screen, true);
        }

        public void ShowPopup(PopupViewModel popup, double width = 800, double height = 400)
        {
            EventAggregator.PublishOnCurrentThread(new ShowPopup { Popup = popup, Width = width, Height = height });
        }

        public void HandleFault(AggregateException ex)
        {
            Logger.Error(ex.Message, ex);

            var message = IoC.Get<MessageBoxViewModel>();
            message.DisplayName = "Error";
            message.Content = new ErrorInfo();
            message.DismissAction = () => NavigateToHome();
            message.DismissTimeout = 10000;

            NavigateToScreen(message);
        }

        public void StartIdleTimeout()
        {
            ResetIdleTimeout();
        }        

        public void StopIdleTimeout()
        {
            ResetIdleTimeout(false);
        }

        public void LogButtonPressed()
        {
            if (CancellationTokenSource != null)
                ResetIdleTimeout();
        }

        public void ResetIdleTimeout(bool restart = true)
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource = null;
            }

            if (restart)
            {
                CancellationTokenSource = new CancellationTokenSource();

                Task.Delay(IdleTimeout, CancellationTokenSource.Token).ContinueWith(t =>
                {
                    if (t.IsCompleted && !t.IsCanceled)
                        Execute.OnUIThread(() => NavigateToHome());
                });
            }
        }
    }
}
