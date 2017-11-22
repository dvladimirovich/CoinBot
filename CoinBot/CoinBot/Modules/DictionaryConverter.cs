using CoinBot.Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CoinBot.Modules
{
    public class DictionaryConverter : JsonConverter
    {
        public override bool CanRead {
            get { return true;  }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<Coin, decimal>);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return null;
                case JsonToken.StartArray:
                    reader.Read();
                    if (reader.TokenType == JsonToken.EndArray)
                        return new Dictionary<Coin, decimal>();
                    var result = new Dictionary<Coin, decimal>();
                    Coin coin = new Coin();
                    decimal amount = 0M;
                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        reader.Read();
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            if (reader.Value.ToString() == "coin")
                            {
                                reader.Read();
                                coin = serializer.Deserialize<Coin>(reader);
                            }
                            else if (reader.Value.ToString() == "amount")
                            {
                                reader.Read();
                                amount = Convert.ToDecimal(reader.Value);
                            }
                        }
                        else if (reader.TokenType == JsonToken.EndObject)
                        {
                            result.Add(coin, amount);
                        }
                    }
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (DictionaryEntry item in (IDictionary)value)
            {
                writer.WriteStartObject();
                // coin
                writer.WritePropertyName("coin");
                serializer.Serialize(writer, item.Key);
                //amount
                writer.WritePropertyName("amount");
                writer.WriteValue(item.Value);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}