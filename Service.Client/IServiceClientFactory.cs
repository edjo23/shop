﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Client
{
    public interface IServiceClientFactory
    {
        ServiceClient<TChannel> GetClient<TChannel>()
            where TChannel : class;

        DuplexServiceClient<TChannel> GetDuplexClient<TChannel>(InstanceContext callbackInstance)
            where TChannel : class;
    }

    public class ServiceClientFactory : IServiceClientFactory
    {
        public ServiceClient<TChannel> GetClient<TChannel>()
            where TChannel : class
        {
            return new ServiceClient<TChannel>();
        }

        public DuplexServiceClient<TChannel> GetDuplexClient<TChannel>(InstanceContext callbackInstance)
            where TChannel : class
        {
            return new DuplexServiceClient<TChannel>(callbackInstance);
        }
    }    
}
