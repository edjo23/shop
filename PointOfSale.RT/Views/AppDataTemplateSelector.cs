using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PointOfSale.RT.Views
{
    public class AppDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultDataTemplate { get; set; }

        protected Dictionary<string, DataTemplate> Cache = new Dictionary<string, DataTemplate>();

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item == null)
                return base.SelectTemplateCore(item, container);

            var key = item.GetType().Name;

            if (Cache.ContainsKey(key))
                return Cache[key];

            var template = Find(Application.Current.Resources, key);
            if (template != null)
                Cache.Add(key, template);
            else
                template = DefaultDataTemplate;

             return template;
        }

        protected DataTemplate Find(ResourceDictionary resource, string key)
        {
            var template = resource.FirstOrDefault(o => o.Key.ToString() == key).Value as DataTemplate;
            if (template != null)
                return template;
            
            foreach (var merge in resource.MergedDictionaries)
            {
                template = Find(merge, key);
                if (template != null)
                    return template;
            }

            return null;
        }
    }
}
