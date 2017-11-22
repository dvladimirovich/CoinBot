using System.Collections.Generic;
using Newtonsoft.Json;
using CoinBot.Modules;

namespace CoinBot.Entity
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UserData
    {
        [JsonProperty]
        public string Id { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty] [JsonConverter(typeof(DictionaryConverter))]
        public Dictionary<Coin, decimal> Portfolio { get; }

        public UserData(string id, string name)
        {
            Id = id;
            Name = name;
            Portfolio = new Dictionary<Coin, decimal>(new CoinComparer());
        }

        public void AddCoins(Coin coin, decimal amount)
        {
            if (Portfolio.ContainsKey(coin))
            {
                Portfolio[coin] += amount;
            }
            else
            {
                Portfolio.Add(coin, amount);
            }
        }

        public void RemoveCoins(Coin coin, decimal amount)
        {
            if (Portfolio.ContainsKey(coin))
            {
                if (amount >= Portfolio[coin])
                    Portfolio[coin] = 0;
                else
                    Portfolio[coin] -= amount;
            }
            else
            {
                throw new KeyNotFoundException($"Your portfolio doesn't contain such currency.");
            }
        }
    }
}