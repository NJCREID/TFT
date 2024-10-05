using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace TFT_API.Helpers
{
    // Converter for serializing and deserializing dictionaries to and from JSON strings
    public class DictionaryToJsonConverter : ValueConverter<Dictionary<string, string>, string>
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public DictionaryToJsonConverter()
            : base(
                v => v == null ? "null" : JsonSerializer.Serialize(v, JsonSerializerOptions),
                v => string.IsNullOrEmpty(v) || v == "null" ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions) ?? new Dictionary<string, string>()
            )
        {
        }
    }

    // Comparer for comparing dictionaries
    public class DictionaryComparer : ValueComparer<Dictionary<string, string>>
    {
        public DictionaryComparer() : base(
        (d1, d2) => DictionaryEquals(d1, d2),
            d => DictionaryHashCode(d),
            d => new Dictionary<string, string>(d ?? new Dictionary<string, string>())
        )
        {
        }
        // Check if two dictionaries are equal
        private static bool DictionaryEquals(Dictionary<string, string>? d1, Dictionary<string, string>? d2)
        {
            if (d1 == null || d2 == null)
            {
                return d1 == d2;
            }

            return d1.Count == d2.Count && !d1.Except(d2).Any();
        }

        // Generate a hash code for a dictionary
        private static int DictionaryHashCode(Dictionary<string, string>? d)
        {
            if (d == null)
            {
                return 0;
            }
            return d.Aggregate(0, (acc, kvp) => acc ^ kvp.Key.GetHashCode() ^ kvp.Value.GetHashCode());
        }
    }
}
