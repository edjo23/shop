using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Shop.PointOfSale.Behaviors
{
    public class FullScreenBehaviour : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.WindowState = WindowState.Maximized;
            AssociatedObject.WindowStyle = WindowStyle.None;

            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.WindowState = WindowState.Normal;
            AssociatedObject.WindowStyle = WindowStyle.SingleBorderWindow;

            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (AssociatedObject.WindowState == WindowState.Maximized)
                {
                    AssociatedObject.WindowState = WindowState.Normal;
                    AssociatedObject.WindowStyle = WindowStyle.SingleBorderWindow;
                }
                else
                {
                    AssociatedObject.WindowState = WindowState.Maximized;
                    AssociatedObject.WindowStyle = WindowStyle.None;
                }
            }            
        }
    }
}
