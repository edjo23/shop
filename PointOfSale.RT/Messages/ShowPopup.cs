using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.ViewModels;

namespace PointOfSale.RT.Messages
{   
    public class ShowPopup
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public PopupViewModel Popup { get; set; }
    }
}
