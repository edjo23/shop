using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace PointOfSale.RT.Controls
{
    public class AnimatedButton : Button
    {
        public AnimatedButton()
        {
            this.DefaultStyleKey = typeof(AnimatedButton);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            this.CapturePointer(e.Pointer);

            VisualStateManager.GoToState(this, "PointerDown", true);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerUp", true);

            this.ReleasePointerCapture(e.Pointer);
        }
    }
}
