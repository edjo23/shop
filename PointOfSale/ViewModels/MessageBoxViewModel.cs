using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;

namespace Shop.PointOfSale.ViewModels
{
    public class MessageBoxViewModel : Screen
    {
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

        private Brush _Background;

        public Brush Background
        {
            get
            {
                return _Background;
            }
            set
            {
                if (value != _Background)
                {
                    _Background = value;

                    NotifyOfPropertyChange(() => Background);
                }
            }
        }
    }
}
