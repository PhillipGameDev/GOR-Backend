using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GameOfRevenge.Common.Models
{
    public class IncludeJsonIgnoredProperties : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.Ignored = false;

            return property;
        }

        public static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new IncludeJsonIgnoredProperties()
        };
    }
}
