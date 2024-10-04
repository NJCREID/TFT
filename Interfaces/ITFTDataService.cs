namespace TFT_API.Interfaces
{
    public interface ITFTDataService
    {
        Task SaveImagesAsync<T>(List<T> items, Func<T, string> getImageUrl, Func<T, string> getFileName,  string directory);
        Task<List<T>> FetchDataAsync<T>(string url, string type);
    }
}
