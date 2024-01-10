using Developist.Core.Persistence;

namespace Developist.Customers.Api.Serialization;

/// <summary>
/// A JSON converter factory for converting instances of <see cref="IPaginatedList{T}"/> to and from JSON.
/// </summary>
/// <example>
/// With default web settings, this JSON converter outputs a paginated list in the following format:
/// <code>
/// {
///   "pageNumber": 1,
///   "pageSize": 25,
///   "pageCount": 2,
///   "totalItemCount": 47,
///   "items": [
///     { "id": 101, "name": "exampleItem1" },
///     { "id": 102, "name": "exampleItem2" },
///     ....
///   ]
/// }
/// </code>
/// </example>
public class PaginatedListJsonConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        var isPaginatedList = typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(IPaginatedList<>);
        var implementsPaginatedList = !isPaginatedList && typeToConvert.ImplementsGenericInterface(typeof(IPaginatedList<>));

        return isPaginatedList || implementsPaginatedList;
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(GenericPaginatedListJsonConverter<>).MakeGenericType(new[] { itemType });
        var converter = (JsonConverter)Activator.CreateInstance(converterType, args: null)!;

        return converter;
    }

    private class GenericPaginatedListJsonConverter<T> : JsonConverter<IPaginatedList<T>>
        where T : class
    {
        private const string PageNumberPropertyName = nameof(IPaginatedList<T>.PageNumber);
        private const string PageSizePropertyName = nameof(IPaginatedList<T>.PageSize);
        private const string PageCountPropertyName = nameof(IPaginatedList<T>.PageCount);
        private const string TotalItemCountPropertyName = "TotalItemCount";
        private const string ItemsPropertyName = "Items";

        /// <inheritdoc/>
        public override IPaginatedList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("JSON parsing error: Expected the start of a JSON object but found a different token type.");
            }

            IReadOnlyList<T>? items = null;
            int? totalItemCount = null;
            int? pageSize = null;
            int? pageNumber = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject
                    && items is not null
                    && totalItemCount.HasValue && pageSize.HasValue && pageNumber.HasValue)
                {
                    return new PaginatedList<T>(pageNumber.Value, pageSize.Value, items, totalItemCount.Value);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case var _ when propertyName == GetPropertyName(ItemsPropertyName, options.PropertyNamingPolicy):
                            items = ReadListProperty(ref reader, propertyName, options);
                            break;
                        case var _ when propertyName == GetPropertyName(TotalItemCountPropertyName, options.PropertyNamingPolicy):
                            totalItemCount = ReadInt32Property(ref reader, propertyName);
                            break;
                        case var _ when propertyName == GetPropertyName(PageCountPropertyName, options.PropertyNamingPolicy):
                            break;
                        case var _ when propertyName == GetPropertyName(PageSizePropertyName, options.PropertyNamingPolicy):
                            pageSize = ReadInt32Property(ref reader, propertyName);
                            break;
                        case var _ when propertyName == GetPropertyName(PageNumberPropertyName, options.PropertyNamingPolicy):
                            pageNumber = ReadInt32Property(ref reader, propertyName);
                            break;
                        default:
                            throw new JsonException($"JSON parsing error: The property '{propertyName}' does not match any expected property of a PaginatedList object.");
                    }
                }
            }

            throw new JsonException("JSON parsing error: Reached the end of the input without finding a complete PaginatedList object.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, IPaginatedList<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // Write metadata
            writer.WriteNumber(GetPropertyName(PageNumberPropertyName, options.PropertyNamingPolicy), value.PageNumber);
            writer.WriteNumber(GetPropertyName(PageSizePropertyName, options.PropertyNamingPolicy), value.PageSize);
            writer.WriteNumber(GetPropertyName(PageCountPropertyName, options.PropertyNamingPolicy), value.PageCount);
            writer.WriteNumber(GetPropertyName(TotalItemCountPropertyName, options.PropertyNamingPolicy), value.TotalCount);

            // Write data
            writer.WritePropertyName(GetPropertyName(ItemsPropertyName, options.PropertyNamingPolicy));
            writer.WriteStartArray();
            foreach (var item in value)
            {
                JsonSerializer.Serialize(writer, item, options);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private static string GetPropertyName(string propertyName, JsonNamingPolicy? propertyNamingPolicy)
        {
            return propertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;
        }

        private static int ReadInt32Property(ref Utf8JsonReader reader, string propertyName)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException($"JSON parsing error: Expected a numeric value for property '{propertyName}', but found a non-numeric token.");
            }

            return reader.GetInt32();
        }

        private static IReadOnlyList<T> ReadListProperty(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException($"JSON parsing error: Expected the start of an array for property '{propertyName}', but found a different token type.");
            }

            return JsonSerializer.Deserialize<IReadOnlyList<T>>(ref reader, options) ?? Array.Empty<T>();
        }
    }
}
