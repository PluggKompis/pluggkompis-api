using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Test.IntegrationTests.Extensions
{
    public static class HttpContentExtensions
    {
        private static readonly JsonSerializerOptions _options;

        static HttpContentExtensions()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            _options.Converters.Add(new JsonStringEnumConverter());
        }

        public static Task<T?> ReadFromJsonWithEnumAsync<T>(this HttpContent content)
        {
            return content.ReadFromJsonAsync<T>(_options);
        }
    }
}
