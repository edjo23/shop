using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Shop.Management.ViewModels
{
    public class SummaryViewModel : Screen
    {
        private bool _CanRefresh = false;

        public bool CanRefresh
        {
            get
            {
                return _CanRefresh;
            }
            set
            {
                if (value != _CanRefresh)
                {
                    _CanRefresh = value;

                    NotifyOfPropertyChange(() => CanRefresh);
                }
            }
        }
    }
}
