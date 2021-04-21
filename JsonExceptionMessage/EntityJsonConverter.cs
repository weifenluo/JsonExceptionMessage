using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonExceptionMessage
{
    public class EntityJsonConverter : JsonConverter<Entity>
    {
        public override Entity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return EntitySerializer.Deserialize(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, Entity value, JsonSerializerOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}
