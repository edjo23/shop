using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Messages;

namespace PointOfSale.RT.ViewModels
{
    public class PinPopupViewModel : PopupViewModel
    {
        public const string EmptyPinState = "enter pin";

        public PinPopupViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        private readonly IEventAggregator EventAggregator;

        private string Pin = "";

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

        public override void Activate()
        {
        }

        public Action<string> OnPinEntered { get; set; }

        public void PinEntered()
        {
            EventAggregator.Publish(new HidePopup());

            if (OnPinEntered != null)
                OnPinEntered(Pin);
        }

        public void Add(char ch)
        {
            lock (Pin)
            {
                if (Pin.Length == 4)
                    return;

                Pin += ch;
                PinState = new string('●', Pin.Length);
            }

            if (PinState.Length == 4)
                Task.Factory.StartNew(() => Task.Delay(50).ContinueWith(t => Execute.OnUIThread(() => PinEntered())));
        }

        public void Remove()
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
}
