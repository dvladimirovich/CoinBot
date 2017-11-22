using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace CoinBot.Entity
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Coin
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("rank")]
        public string Rank { get; set; }

        [JsonProperty("price_usd")]
        public string PriceUsd { get; set; }

        //[JsonProperty("price_btc")]
        //public string PriceBtc { get; set; }
        //
        //[JsonProperty(PropertyName = "24h_volume_usd")]   
        //public string VolumeUsd24h { get; set; }
        //
        //[JsonProperty("market_cap_usd")]
        //public string MarketCapUsd { get; set; }
        //
        //[JsonProperty("available_supply")]
        //public string AvailableSupply { get; set; }
        //
        //[JsonProperty("total_supply")]
        //public string TotalSupply { get; set; }
        //
        //[JsonProperty("max_supply")]
        //public string MaxSupply { get; set; }
        //
        //[JsonProperty("percent_change_1h")]
        //public string PercentChange1h { get; set; }
        //
        //[JsonProperty("percent_change_24h")]
        //public string PercentChange24h { get; set; }
        //
        //[JsonProperty("percent_change_7d")]
        //public string PercentChange7d { get; set; }
        //
        //[JsonProperty("last_updated")]
        //public string LastUpdated { get; set; }
    }

    public class CoinComparer : IEqualityComparer<Coin>
    {
        public bool Equals(Coin x, Coin y)
        {
            return x.Id.Equals(y.Id, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Coin obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}