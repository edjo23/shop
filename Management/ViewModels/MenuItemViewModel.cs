using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Shop.Management.ViewModels
{
    public interface IMenuItem
    {
        string Name { get; }
        Screen Instance { get; }

        bool IsSelected { get; set; }
    }

    public class MenuItemViewModel<T> : PropertyChangedBase, IMenuItem
        where T : Screen
    {
        public MenuItemViewModel(T instance)
        {
            Instance = instance;
        }

        public MenuItemViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        private bool _IsSelected = false;

        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (value != _IsSelected)
                {
                    _IsSelected = value;

                    NotifyOfPropertyChange(() => IsSelected);
                }
            }
        }

        private Screen _Instance = null;

        public Screen Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = IoC.Get<T>();

                return _Instance;
            }
            private set
            {
                _Instance = value;
            }
        }
    }
}
