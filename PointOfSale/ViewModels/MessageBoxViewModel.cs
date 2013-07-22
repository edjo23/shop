using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using Caliburn.Micro;

namespace Shop.PointOfSale.ViewModels
{
    public class MessageBoxViewModel : Screen
    {
        public MessageBoxViewModel()
        {
            DisplayName = "Information";
        }

        public System.Action DismissAction { get; set; }

        public int DismissTimeout { get; set; }

        private Timer DismissTimer = new Timer() { AutoReset = false };

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

            if (DismissTimeout > 0)
            {
                DismissTimer.Interval = DismissTimeout;
                DismissTimer.Elapsed += (o, a) => { if (IsActive) Dismiss(); };
                DismissTimer.Start();
            }
        }

        public void Dismiss()
        {
            DismissTimer.Stop();

            if (DismissAction != null)
                DismissAction();
        }
    }
}
