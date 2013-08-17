using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Windows.System.Threading;
using Windows.UI.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace PointOfSale.RT.Behaviors
{
    public class ElementDeferredVisibilityBehaviour : Behavior<FrameworkElement>
    {
        public ElementDeferredVisibilityBehaviour()
        {
            VisibileDelay = 250;
            AnimationDuration = 250;
        }

        public int VisibileDelay { get; set; }

        public int AnimationDuration { get; set; }

        protected override void OnAttached()
        {
            AnimateOpactiy();
        }

        private void AnimateOpactiy()
        {
            AssociatedObject.Opacity = 0.0;

            var doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0.0;
            doubleAnimation.To = 1.0;
            doubleAnimation.BeginTime = TimeSpan.FromMilliseconds(VisibileDelay);
            doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(AnimationDuration));

            Storyboard.SetTarget(doubleAnimation, AssociatedObject);
            Storyboard.SetTargetProperty(doubleAnimation, "Opacity");

            var storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }
    }
}
