using ImageMagick;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using TFT_API.Interfaces;
namespace TFT_API.Services
{
    /// <summary>
    /// A service to fetch data and images for TFT (Teamfight Tactics).
    /// Implements ITFTDataService.
    /// </summary>
    public partial class TFTDataFetchService(HttpClient httpClient) : ITFTDataService
    {
        private readonly HttpClient _httpClient = httpClient;

        /// <summary>
        /// A compiled regular expression to match the 'fill' attribute in SVG files.
        /// </summary>
        [GeneratedRegex(@"fill=""[^""]*""", RegexOptions.IgnoreCase, "en-AU")]
        private static partial Regex MyRegex();

        /// <summary>
        /// Fetches data from the specified URL, deserializes it into a list of the specified type, and returns it.
        /// </summary>
        /// <typeparam name="T">The type of data to fetch.</typeparam>
        /// <param name="url">The URL to fetch the data from.</param>
        /// <param name="type">The type of property to extract from the JSON response.</param>
        /// <returns>A list of deserialized objects of type <typeparamref name="T"/>.</returns>
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

        /// <summary>
        /// Saves images from the given list of items using the provided functions to get the image URL and file name.
        /// Supports both SVG and other image formats.
        /// </summary>
        /// <typeparam name="T">The type of items to save images for.</typeparam>
        /// <param name="items">The list of items containing image data.</param>
        /// <param name="getImageUrl">A function to get the image URL from an item.</param>
        /// <param name="getFileName">A function to get the file name from an item.</param>
        /// <param name="directory">The directory to save images in.</param>
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

        /// <summary>
        /// Updates the fill colors of SVG images to white.
        /// </summary>
        /// <param name="filePath">The path to the SVG file to update.</param>
        private static void UpdateSvgFillColors(string filePath)
        {
            var svgContent = File.ReadAllText(filePath);
            var updatedSvgContent = MyRegex().Replace(svgContent, @"fill=""#fff""");
            File.WriteAllText(filePath, updatedSvgContent);
        }

        /// <summary>
        /// Converts an input stream to an AVIF image and saves it to the specified output path.
        /// </summary>
        /// <param name="inputStream">The input stream containing image data.</param>
        /// <param name="outputPath">The path to save the AVIF image to.</param>
        private static void ConvertAndSaveAvif(Stream inputStream, string outputPath)
        {
            using var image = new MagickImage(inputStream);
            image.Format = MagickFormat.Avif;
            image.Write(outputPath);
        }
    }
}
