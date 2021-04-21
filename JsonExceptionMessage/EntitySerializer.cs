using System;
using System.Text;
using System.Text.Json;

namespace JsonExceptionMessage
{
    public static class EntitySerializer
    {
        public static Entity Deserialize(string json, JsonSerializerOptions? options = null)
        {
            if (options == null)
                options = new();

            var utf8Json = Encoding.UTF8.GetBytes(json);
            var jsonReader = GetJsonReader(utf8Json, options, isFinalBlock: true);
            return Read(ref jsonReader, options, isTopLevel: true);
        }

        private static Utf8JsonReader GetJsonReader(ReadOnlySpan<byte> jsonData, JsonSerializerOptions options, bool isFinalBlock)
        {
            var readerState = new JsonReaderState(GetReaderOptions(options));
            return new(jsonData, isFinalBlock, readerState);
        }

        private static JsonReaderOptions GetReaderOptions(JsonSerializerOptions options)
        {
            return new JsonReaderOptions
            {
                AllowTrailingCommas = options.AllowTrailingCommas,
                CommentHandling = options.ReadCommentHandling,
                MaxDepth = options.MaxDepth
            };
        }

        public static Entity Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions? options = null)
        {
            if (options == null)
                options = new();

            return Read(ref reader, options, isTopLevel: false);
        }

        private static Entity Read(ref Utf8JsonReader reader, JsonSerializerOptions options, bool isTopLevel)
        {
            var currentPath = "";

            try
            {

                if (isTopLevel)
                    reader.Read();

                VerifyCurrentToken(ref reader, JsonTokenType.StartObject, currentPath, isTopLevel);

                Entity result = new Entity();

                reader.Read();
                VerifyCurrentToken(ref reader, JsonTokenType.PropertyName, currentPath, isTopLevel);

                var name = reader.GetString();
                if (name != nameof(Entity.IntValue))
                    throw new JsonException($"Unknown property name {name}.");

                currentPath = "." + name;

                reader.Read();
                VerifyCurrentToken(ref reader, JsonTokenType.Number, currentPath, isTopLevel);

                result.IntValue = reader.GetInt32();

                currentPath = "";
                reader.Read();
                VerifyCurrentToken(ref reader, JsonTokenType.EndObject, currentPath, isTopLevel);

                if (isTopLevel)
                    reader.Read();

                return result;
            }
            catch (Exception ex)
            {
                if (isTopLevel)
                    throw GetJsonExceptionToRethrow(ex, ref reader, currentPath);
                else
                    throw new JsonException(null, currentPath, null, null, ex);
            }
        }

        private static void VerifyCurrentToken(ref Utf8JsonReader reader, JsonTokenType expectedTokenType, string path, bool isTopLevel)
        {
            if (reader.TokenType != expectedTokenType)
                throw new JsonException($"Expect JSON token type {expectedTokenType}, actual token type is {reader.TokenType}.");
        }

        private static JsonException GetJsonExceptionToRethrow(Exception? innerException, ref Utf8JsonReader reader, string path)
        {
            path = "$" + path;

            long lineNumber = 0;    // should get this value from reader.CurrentState
            long bytePositionInLine = 0;    // should get this value from reader.CurrentState

            string message = $"An error occurred while deserializing Entity. See inner exception for details. | Path: {path} | LineNumber: {lineNumber} | BytePositionInLine {bytePositionInLine}.";
            return new JsonException(message, path, lineNumber, bytePositionInLine, innerException);
        }
    }
}
