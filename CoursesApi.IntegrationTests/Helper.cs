using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CoursesApi.IntegrationTests
{
    internal class Helper
    {
        public static StringContent GetStringContent<TValue>(TValue obj)
            => new(JsonSerializer.Serialize(obj), Encoding.Default, "application/json");

        public static async Task<Dictionary<string, IEnumerable<string>>> GetValidationErrors(HttpResponseMessage response)
        {
            var stringResult = await response.Content.ReadAsStringAsync();

            return GetValidationErrors(stringResult);
        }

        public static Dictionary<string, IEnumerable<string>> GetValidationErrors(string json)
        {
            JsonNode? rootNode = JsonNode.Parse(json);
            var errorsNode = rootNode?["errors"]?.AsObject();
            if (errorsNode == null)
            {
                return new Dictionary<string, IEnumerable<string>>();
            }

            return errorsNode.ToDictionary(
                x => x.Key, 
                x => x.Value?.AsArray().Select(n => n?.GetValue<string>() ?? string.Empty) ?? new List<string>());
        }
    }
}
