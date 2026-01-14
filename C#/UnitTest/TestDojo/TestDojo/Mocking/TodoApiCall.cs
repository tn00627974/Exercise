using System.Net.Http.Json;

namespace TestDojo.Mocking;

public class TodoApiCall
{
    private readonly IMyHttpClient _client;
    public TodoApiCall(IMyHttpClient client)
    {
        _client = client;
    }

    public async Task<string> GetTodoTitleAsync(int id)
    {
        var todo = await _client.GetAsync($"https://jsonplaceholder.typicode.com/todos/{id}");

        return (todo != null) ? todo.Title : string.Empty;
    }
}

public interface IMyHttpClient
{
    Task<Todo?> GetAsync(string url);
}


public class MyHttpClient : IMyHttpClient
{
    private readonly HttpClient _client;

    public MyHttpClient()
    {
        _client = new HttpClient();
    }

    public async Task<Todo?> GetAsync(string url)
    {
        var response = await _client.GetAsync(url);
        var todo = await response.Content.ReadFromJsonAsync<Todo>();
        return todo;
    }
}

public class Todo
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool Completed { get; set; }
}