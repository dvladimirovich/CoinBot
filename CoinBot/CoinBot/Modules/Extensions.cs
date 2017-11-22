using CoinBot.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoinBot.Modules
{
    public static class Extensions
    {
        public static async Task<decimal> ToUSD(this Dictionary<Coin, decimal> portfolio)
        {
            if (portfolio == null)
                throw new ArgumentNullException();
            if (portfolio.Count == 0)
                return 0.0M;
            CoinApiHandler handler = new CoinApiHandler();
            List<Coin> coins = await handler.GetCoins();
            if (handler.HasError)
            {
                throw new Exception(handler.Error);
            }
            decimal result = 0.0M;
            foreach (var coin in coins)
            {
                foreach (var item in portfolio)
                {
                    if (coin.Name == item.Key.Name ||
                        coin.Symbol == item.Key.Symbol)
                    {
                        result += Convert.ToDecimal(coin.PriceUsd) * item.Value;
                        break;
                    }
                }
            }
            return result;
        }
        
    }
}