using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace Shop.PointOfSale.Behaviors
{
    public class ElementDeferredVisibilityBehaviour : Behavior<FrameworkElement>
    {
        public ElementDeferredVisibilityBehaviour()
        {
            VisibileDelay = 500;
            AnimationDuration = 500;
        }

        public int VisibileDelay { get; set; }

        public int AnimationDuration { get; set; }

        protected override void OnAttached()
        {
            AssociatedObject.Opacity = 0.0;
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject.IsVisible)
            {
                Action action = () => AssociatedObject.BeginAnimation(FrameworkElement.OpacityProperty, new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(AnimationDuration)));

                if (VisibileDelay > 0)
                {
                    var timer = new Timer { Interval = VisibileDelay, AutoReset = false };
                    timer.Elapsed += (o, a) => Dispatcher.Invoke(action);
                    timer.Start();
                }
                else
                {
                    action();
                }
            }
            else
            {
                AssociatedObject.BeginAnimation(FrameworkElement.OpacityProperty, new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(0)));
            }
        }
    }
}
