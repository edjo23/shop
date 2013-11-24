using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Services;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace PointOfSale.RT.ViewModels
{
    public class PinEntryViewModel : Screen
    {
        public const string EmptyPinState = "enter your pin";
        public const string IncorrectPinState = "incorrect pin";

        public PinEntryViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ICustomerService CustomerService;

        private bool Working = false;

        public Customer Customer { get; set; }

        private string _PinState = EmptyPinState;

        public string PinState
        {
            get
            {
                return _PinState;
            }
            set 
            {
                if (value != _PinState)
                {
                    _PinState = value;

                    NotifyOfPropertyChange(() => PinState);
                }
            }
        }

        private string _Pin = "";

        public string Pin
        {
            get
            {
                return _Pin;
            }
            set
            {
                if (value != _Pin)
                {
                    _Pin = value;

                    NotifyOfPropertyChange(() => Pin);
                }
            }
        }

        public int FailureCount { get; set; }

        public void CheckPin()
        {
            Working = true;

            Task.Factory
                .StartNew(() =>
                {
                    var pinValid = CustomerService.CheckPin(Customer.Id, Pin);
                    if (!pinValid)
                    {
                        Execute.OnUIThread(() =>
                        {
                            FailureCount++;

                            lock (Pin)
                            {
                                PinState = IncorrectPinState;
                                Pin = "";
                            }

                            Working = false;
                        });

                        // Reset text or go home if too many failures...
                        Task.Delay(1000).ContinueWith(t => Execute.OnUIThread(() =>
                        {
                            if (FailureCount < 3)
                            {
                                lock (Pin)
                                {
                                    if (Pin.Length == 0)
                                        PinState = EmptyPinState;
                                }
                            }
                            else
                            {
                                if (IsActive)
                                    GoHome();
                            }
                        }));
                    }
                    else
                    {
                        Execute.OnUIThread(() => ScreenCoordinator.NavigateToCustomer(Customer));
                    }
                })
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        ScreenCoordinator.HandleFault(task.Exception);
                    }
                });
        }

        public void Add(char ch)
        {
            if (Working)
                return;

            if (FailureCount < 3)
            {
                lock (Pin)
                {
                    if (Pin.Length == 4)
                        return;

                    Pin += ch;
                    PinState = new string('●', Pin.Length);
                }

                if (PinState.Length == 4)
                    Task.Factory.StartNew(() => Task.Delay(50).ContinueWith(t => Execute.OnUIThread(() => CheckPin())));
            }
        }

        public void Remove()
        {
            if (Working)
                return;

            if (FailureCount < 3)
            {
                lock (Pin)
                {
                    if (Pin.Length == 0)
                        return;

                    Pin = Pin.Substring(0, Pin.Length - 1);
                    PinState = Pin.Length == 0 ? EmptyPinState : new string('●', Pin.Length);
                }
            }
        }

        public void GoHome()
        {
            ScreenCoordinator.NavigateToHome();
        }
    }
}
