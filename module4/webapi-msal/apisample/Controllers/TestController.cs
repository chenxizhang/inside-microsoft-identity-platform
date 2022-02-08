
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

[ApiController]
[Authorize]
[Route("/test")]
public class TestController : ControllerBase
{
    private readonly IDownstreamWebApi _downstreamWebApi;
    private readonly ILogger<string> _logger;


    public TestController(IDownstreamWebApi downstreamWebApi, ILogger<string> logger)
    {
        _downstreamWebApi = downstreamWebApi;
        _logger = logger;
    }

    public async Task<object> Get()
    {

        using var response = await _downstreamWebApi.CallWebApiForUserAsync("DownstreamApi", op => op.RelativePath = "/me/messages").ConfigureAwait(false);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var apiResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            // Do something
            return apiResult;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}: {error}");
        }
    }
}