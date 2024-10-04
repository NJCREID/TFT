using ImageMagick;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using TFT_API.Interfaces;
namespace TFT_API.Services
{
    public partial class TFTDataFetchService(HttpClient httpClient) : ITFTDataService
    {
        private readonly HttpClient _httpClient = httpClient;

        [GeneratedRegex(@"fill=""[^""]*""", RegexOptions.IgnoreCase, "en-AU")]
        private static partial Regex MyRegex();

        public async Task<List<T>> FetchDataAsync<T>(string url, string type)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);

                if (doc.RootElement.TryGetProperty(type, out JsonElement itemsElement))
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    return JsonSerializer.Deserialize<List<T>>(itemsElement.GetRawText(), options) ?? [];
                }
        }
            catch (HttpRequestException e)
            {
                Console.Error.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.Error.WriteLine($"Deserialization error: {e.Message}");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Unexpected error: {e.Message}");
            }
            return [];
        }

        public async Task SaveImagesAsync<T>(List<T> items, Func<T, string> getImageUrl, Func<T, string> getFileName, string directory)
        {
            Directory.CreateDirectory(directory);

            foreach(var item in items)
            {
                var imageUrl = getImageUrl(item);
                if (string.IsNullOrEmpty(imageUrl)) continue;
                if (!imageUrl.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
                {
                    imageUrl = "https:" + imageUrl;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, imageUrl);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) continue;

                var fileName = getFileName(item);
                var fileExtension = Path.GetExtension(imageUrl);
                    
                await using var stream = await response.Content.ReadAsStreamAsync();
                try
                {
                    if (fileExtension.Equals(".svg", StringComparison.OrdinalIgnoreCase))
                    {
                        var svgFilePath = Path.Combine(directory, Path.GetFileName(imageUrl));
                        await using var fileStream = new FileStream(svgFilePath, FileMode.Create, FileAccess.Write);
                        await stream.CopyToAsync(fileStream);
                        UpdateSvgFillColors(svgFilePath);
                    }
                    else
                    {
                        var avifFilePath = Path.Combine(directory, $"{fileName}.avif");
                        ConvertAndSaveAvif(stream, avifFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error saving file for URL: {imageUrl}, Exception: {ex.Message}");
                }
            }
        }

        private static void UpdateSvgFillColors(string filePath)
        {
            var svgContent = File.ReadAllText(filePath);
            var updatedSvgContent = MyRegex().Replace(svgContent, @"fill=""#fff""");
            File.WriteAllText(filePath, updatedSvgContent);
        }

        private static void ConvertAndSaveAvif(Stream inputStream, string outputPath)
        {
            using var image = new MagickImage(inputStream);
            image.Format = MagickFormat.Avif;
            image.Write(outputPath);
        }
    }
}
