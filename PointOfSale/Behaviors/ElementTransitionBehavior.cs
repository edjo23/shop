using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Shop.PointOfSale.Behaviors
{
    public class ElementTransitionBehaviour : Behavior<FrameworkElement>
    {
        public ElementTransitionBehaviour()
        {
            VisibileDelay = 500;
            AnimationDuration = 500;
        }

        public int VisibileDelay { get; set; }

        public int AnimationDuration { get; set; }

        protected override void OnAttached()
        {
            AssociatedObject.RenderTransform = new TranslateTransform(0, 0);
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
                Action action = () => AssociatedObject.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(25.0, 0.0, TimeSpan.FromMilliseconds(300)));

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
        }
    }
}
