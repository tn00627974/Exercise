using System.Net.Http.Json;

namespace TestDojo.Mocking;

public class PhotoApiCall
{
    private const string Url = "https://jsonplaceholder.typicode.com/photos/";
    private readonly IPhotoHttpClient _client;

    public PhotoApiCall(IPhotoHttpClient client)
    {
        _client = client;
    }

    public async Task<string> GetPhotoUrlAsync(int id)
    {
        var photo = await _client.GetAsync($"{Url}{id}");
        return photo != null ? photo.Url : string.Empty;
    }
}

public interface IPhotoHttpClient
{
    Task<Photo?> GetAsync(string url);
}

public class PhotoHttpClient : IPhotoHttpClient
{
    private readonly HttpClient _client;

    public PhotoHttpClient()
    {
        _client = new HttpClient();
    }

    public async Task<Photo?> GetAsync(string url)
    {
        var response = await _client.GetAsync(url);
        if (response.IsSuccessStatusCode is false)
            throw new Exception("Something went wrong");

        var photo = await response.Content.ReadFromJsonAsync<Photo>();
        return photo;
    }
}

public class Photo
{
    public int AlbumId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Url { get; set; } = "";
    public string ThumbnailUrl { get; set; } = "";
}
