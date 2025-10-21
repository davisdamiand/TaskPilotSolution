using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TaskPilot.Client.Services;
using Xunit;

public class TodoServiceTests
{
    private class StubHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public StubHandler(HttpResponseMessage response) => _response = response;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }

    [Fact]
    public async Task CreateTodoAsync_ReturnsId_OnSuccess()
    {
        // Arrange
        var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(42)
        };
        var httpClient = new HttpClient(new StubHandler(successResponse))
        {
            BaseAddress = new Uri("http://test")
        };
        var service = new TodoService(httpClient);

        // Act
        var id = await service.CreateTodoAsync(new Shared.DTOs.TodoCreateDto());

        // Assert
        Assert.Equal(42, id);
    }

    [Fact]
    public async Task CreateTodoAsync_Throws_OnError()
    {
        // Arrange
        var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Invalid payload")
        };
        var httpClient = new HttpClient(new StubHandler(errorResponse))
        {
            BaseAddress = new Uri("http://test")
        };
        var service = new TodoService(httpClient);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => service.CreateTodoAsync(new Shared.DTOs.TodoCreateDto()));
        Assert.Contains("Invalid payload", ex.Message);
    }
}