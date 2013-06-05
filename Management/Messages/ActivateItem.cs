using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Shop.Management.Messages
{    
    public abstract class ActivateItem
    {
        public abstract Screen CreateItem();
    }

    public class ActivateItem<T> : ActivateItem
        where T : Screen
    {
        public ActivateItem()
        {
        }

        public ActivateItem(Action<T> action)
        {
            Action = action;
        }

        public Action<T> Action { get; set; }

        public override Screen CreateItem()
        {
            var screen = IoC.Get<T>();

            if (Action != null)
                Action(screen);

            return screen;
        }
    }
}
