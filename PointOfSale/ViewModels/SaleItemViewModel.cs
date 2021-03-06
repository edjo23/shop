﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Shop.Contracts.Entities;

namespace Shop.PointOfSale.ViewModels
{
    public class SaleItemViewModel : PropertyChangedBase
    {
        public SaleItemViewModel()
        {

        }
        public SaleItemViewModel(Product product)
        {

        }

        public Product Product { get; set; }

        public string Description
        {
            get
            {
                return Product.Description;
            }
        }

        public decimal Discount { get; set; }

        public BitmapImage ImageSource
        {
            get
            {
                var fileInfo = new FileInfo(String.Format(@"Images\{0}.png", Product.Code));
                if (fileInfo.Exists)
                    return new BitmapImage(new Uri(fileInfo.FullName));
                else
                    return null;
            }
        }

        private int _Quantity;

        public int Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                if (value != _Quantity)
                {
                    _Quantity = value;
                    NotifyOfPropertyChange(() => Quantity);
                    NotifyOfPropertyChange(() => Total);
                    NotifyOfPropertyChange(() => QuantityVisibility);
                }
            }
        }

        public decimal Price
        {
            get
            {
                return Math.Round(Product.Price * (100 - Discount) / 100, 2, MidpointRounding.AwayFromZero);
            }
        }

        public decimal Total
        {
            get
            {
                return Quantity * Price;
            }
        }

        public Visibility QuantityVisibility
        {
            get
            {
                return Quantity > 0 ? Visibility.Visible : Visibility.Hidden;
            }
        }        
    }
}
