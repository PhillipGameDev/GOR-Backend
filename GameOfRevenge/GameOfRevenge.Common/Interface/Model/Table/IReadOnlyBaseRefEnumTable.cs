using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameOfRevenge.Common.Interface.Model.Table
{
    public interface IReadOnlyBaseRefEnumTable<T>// where T : Enum
    {
        [JsonProperty(Order = -3)]
        int Id { get; }

        [JsonProperty(Order = -2)]
        [JsonConverter(typeof(StringEnumConverter))]
        T Code { get; }

        [JsonProperty(Order = -1)]
        string Name { get; }
    }
}
