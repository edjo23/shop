using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Windows.System.Threading;

namespace PointOfSale.RT.ViewModels
{
    public class MessageBoxViewModel : Screen
    {
        public MessageBoxViewModel()
        {
            DisplayName = "Information";
        }

        public System.Action DismissAction { get; set; }

        public int DismissTimeout { get; set; }

        private ThreadPoolTimer DismissTimer;

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
                DismissTimer = ThreadPoolTimer.CreateTimer(t => { if (IsActive) Dismiss(); }, TimeSpan.FromMilliseconds(DismissTimeout));
            }
        }

        public void Dismiss()
        {
            if (DismissAction != null)
            {
                DismissTimer.Cancel();
                DismissAction();
            }
        }
    }
}
