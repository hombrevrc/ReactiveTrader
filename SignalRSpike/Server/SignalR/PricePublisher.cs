﻿using System;
using System.Threading.Tasks;
using Dto.Pricing;
using log4net;

namespace Server.SignalR
{
    class PricePublisher : IPricePublisher
    {
        private readonly IContextHolder _contextHolder;
        private static readonly ILog Log = LogManager.GetLogger(typeof(PricePublisher));
        
        public PricePublisher(IContextHolder contextHolder)
        {
            _contextHolder = contextHolder;
        }

        public async Task Publish(SpotPrice price)
        {
            var context = _contextHolder.Context;
            if (context == null) return;

            var groupName = string.Format(TradingServiceHub.PriceStreamGroupPattern, price.Symbol);
            try
            {
                await context.Group(groupName).OnNewPrice(price);
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Published price to group {0}: {1}", groupName, price);
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("An error occured while publishing price to group {0}: {1}", groupName, price), e);
            }
        }
    }
}