using Newtonsoft.Json;
using System;

namespace HOI4ModBuilder.src.utils.json
{
    public class EnumArrayToStringConverter<T> : JsonConverter<T[]>
    {
        public override void WriteJson(JsonWriter writer, T[] value, JsonSerializer serializer)
        {
            var stringValues = Array.ConvertAll(value, x => x.ToString());
            serializer.Serialize(writer, stringValues);
        }

        public override T[] ReadJson(JsonReader reader, Type objectType, T[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var stringValues = serializer.Deserialize<string[]>(reader);
                var enumValues = new T[stringValues.Length];

                for (int i = 0; i < stringValues.Length; i++)
                    enumValues[i] = (T)Enum.Parse(typeof(T), stringValues[i]);

                return enumValues;
            }

            throw new JsonSerializationException($"Can't deserialize {typeof(T[]).Name}.");
        }
    }
}
