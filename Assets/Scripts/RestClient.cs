using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Diagnostics;
using System.Text.Json;


namespace Adapt
{
    public class RestClient : HttpClient
    {
        public static async Task<Dictionary<string, object>> GetJsonAsDictionary(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Configure client
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // Make request
                    var response = await client.GetAsync(url);

                    // Read content
                    var jsonString = await response.Content.ReadAsStringAsync();

                    // Check if content is empty
                    if (string.IsNullOrEmpty(jsonString))
                    {
                        return new Dictionary<string, object>();
                    }

                    // Configure JSON options
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    // Deserialize to Dictionary<string, object>
                    var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, options);
                    return dictionary ?? new Dictionary<string, object>();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Request failed: {ex.Message}");
                    throw;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Parsing failed: {ex.Message}");
                    throw;
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine($"Request timed out: {ex.Message}");
                    throw;
                }
            }
        }

        static void PrintDictionaryContents(Dictionary<string, object> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                // Handle different value types
                string valueStr = kvp.Value switch
                {
                    JsonElement element => FormatJsonElement(element),
                    null => "null",
                    _ => kvp.Value.ToString()
                };

                Console.WriteLine($"Key: {kvp.Key}, Value: {valueStr} (Type: {kvp.Value?.GetType().Name ?? "null"})");
            }
        }

        static string FormatJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Array => $"[Array with {element.GetArrayLength()} elements]",
                JsonValueKind.Object => "[Object]",
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => "null",
                _ => element.ToString()
            };
        }

        // Example of how to access specific value types
        static void ProcessDictionaryValues(Dictionary<string, object> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                if (kvp.Value is JsonElement element)
                {
                    switch (element.ValueKind)
                    {
                        case JsonValueKind.Number:
                            if (element.TryGetInt32(out int intValue))
                            {
                                Console.WriteLine($"{kvp.Key} is an integer: {intValue}");
                            }
                            else if (element.TryGetDouble(out double doubleValue))
                            {
                                Console.WriteLine($"{kvp.Key} is a double: {doubleValue}");
                            }
                            break;

                        case JsonValueKind.Array:
                            Console.WriteLine($"{kvp.Key} is an array:");
                            foreach (JsonElement item in element.EnumerateArray())
                            {
                                Console.WriteLine($"  {FormatJsonElement(item)}");
                            }
                            break;

                        case JsonValueKind.Object:
                            var nestedDict = element.Deserialize<Dictionary<string, object>>();
                            Console.WriteLine($"{kvp.Key} is a nested object:");
                            if (nestedDict != null)
                            {
                                PrintDictionaryContents(nestedDict);
                            }
                            break;
                    }
                }
            }
        }
    }
}
