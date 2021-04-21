using System.Text.Json.Serialization;

namespace JsonExceptionMessage
{
    [JsonConverter(typeof(EntityJsonConverter))]
    public class Entity
    {
        public int IntValue { get; set; }
    }
}
