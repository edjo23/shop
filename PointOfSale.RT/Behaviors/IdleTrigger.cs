using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace PointOfSale.RT.Behaviors
{
    public class IdleTrigger : Windows.UI.Interactivity.TriggerBase<FrameworkElement>
    {
        public override void Attach(FrameworkElement frameworkElement)
        {
            base.Attach(frameworkElement);

            frameworkElement.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
        }
       
        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            InvokeActions(EventArgs.Empty);
        }
    }
}
