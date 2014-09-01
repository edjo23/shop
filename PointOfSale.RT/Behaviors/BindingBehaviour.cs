using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PointOfSale.RT.Behaviors
{
    /// <summary>
    /// Work arounds required to allow bindings in styles.
    /// Had to pass property as a string and look up DependencyProperty, WinRT does support statics in XAML.
    /// Had to write some thing the looks for properties in object heirachy, WinRT does seem to have anything.
    /// </summary>

    public class StyleBinding
    {
        public string Property { get; set; }

        public BindingBase Binding { get; set; }
    }

    public class StyleBindings : List<StyleBinding>
    {
    }

    public static class StyleBindingSetter
    {
        public static StyleBindings GetStyleBindings(DependencyObject obj)
        {
            return (StyleBindings)obj.GetValue(StyleBindingsProperty);
        }

        public static void SetStyleBindings(DependencyObject obj, StyleBindings value)
        {
            obj.SetValue(StyleBindingsProperty, value);
        }

        public static readonly DependencyProperty StyleBindingsProperty =
            DependencyProperty.RegisterAttached("StyleBindings", typeof(StyleBindings), typeof(StyleBindingSetter), new PropertyMetadata(new StyleBindings(), OnPropertyStyleBindingsChanged));

        private static void OnPropertyStyleBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = d as FrameworkElement;
            var bindings = e.NewValue as StyleBindings;

            foreach (var binding in bindings)
            {
                var properties = d.GetType().GetTypeInfo();

                var property = FindProperty(binding.Property + "Property", d.GetType());
                if (property != null) 
                {
                    var value  = property.GetValue(null);
                    if (value is DependencyProperty)
                        frameworkElement.SetBinding(value as DependencyProperty, binding.Binding);
                }
            }
        }

        private static PropertyInfo FindProperty(string propertyName, Type type)
        {
            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);

            return property ?? (type.GetTypeInfo().BaseType != null ? FindProperty(propertyName, type.GetTypeInfo().BaseType) : null);
        }
    }
}
