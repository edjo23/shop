using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.PointOfSale.Models;
using Shop.PointOfSale.Services;

namespace Shop.PointOfSale.ViewModels
{
    public class ProcessViewModel : Screen
    {
        public ProcessViewModel(ScreenCoordinator screenCoordinator)
        {
            ScreenCoordinator = screenCoordinator;
            Logger = log4net.LogManager.GetLogger(GetType());

            ProcessAction = () => { };
            CompleteAction = () => { };
            ErrorAction = ex => { ScreenCoordinator.HandleFault(ex); };
        }

        public ScreenCoordinator ScreenCoordinator { get; set; }

        public System.Action ProcessAction { get; set; }

        public System.Action CompleteAction { get; set; }

        public System.Action<AggregateException> ErrorAction { get; set; }

        private readonly log4net.ILog Logger;

        private object _Content;

        public object Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (value != _Content)
                {
                    _Content = value;

                    NotifyOfPropertyChange(() => Content);
                }
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Task.Factory.StartNew(ProcessAction).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                        Task.Factory.StartNew(() => ErrorAction(task.Exception));
                    else
                        Task.Factory.StartNew(CompleteAction);
                });
        }
    }
}
