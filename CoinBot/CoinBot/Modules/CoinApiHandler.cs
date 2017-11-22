using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CoinBot.Entity;
using System.IO;
using System.Net.Http;
using System.Linq;
using System;

namespace CoinBot.Modules
{
    public class CoinApiHandler
    {
        string apiKey = "https://api.coinmarketcap.com/v1/ticker";

        public string Error { get; private set; } = "";
        public bool HasError { get { return !string.IsNullOrWhiteSpace(Error); } }

        public async Task<List<Coin>> GetCoins()
        {
            HttpClient hc = new HttpClient();
            HttpResponseMessage response = await hc.GetAsync(apiKey);
            if (response.IsSuccessStatusCode)
            {
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        var serializer = new JsonSerializer();
                        return (List<Coin>)serializer.Deserialize(reader, typeof(List<Coin>));
                    }
                }
            }
            else
            {
                Error = "Incorrect info specified";
            }
            return null;
        }

        public async Task<Coin> GetCoin(string coin)
        {
            //List<Coin> coins = await this.GetCoinsFromFile();
            List<Coin> coins = await this.GetCoins();
            return coins?.FirstOrDefault(c => c.Name.Equals(coin, StringComparison.InvariantCultureIgnoreCase) ||
                                              c.Symbol.Equals(coin, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<bool> IsCurrencyExist(string coin)
        {
            List<Coin> coins = await this.GetCoins();
            if (coins == null)
            {
                return false;
            }
            return coins.Any(c => c.Name.Equals(coin, StringComparison.InvariantCultureIgnoreCase) ||
                                  c.Symbol.Equals(coin, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<List<Coin>> GetCoinsFromFile()
        {
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/bin/api.coinmarketcap.com.json"), System.Text.Encoding.UTF8))
            {
                var serializer = new JsonSerializer();
                return (List<Coin>)serializer.Deserialize(reader, typeof(List<Coin>));
            }
        }
    }
}