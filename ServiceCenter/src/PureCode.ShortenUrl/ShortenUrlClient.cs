using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PureCode.ShortenUrl
{
  public class ShortenUrlClient
  {
    public const string Prefix = nameof(ShortenUrlClient) + ".";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ShortenUrlClient> logger;
    private readonly ShortenUrlOptions options;

    public ShortenUrlClient(IHttpClientFactory httpClientFactory, ILogger<ShortenUrlClient> logger, IOptions<ShortenUrlOptions> optionsAccessor)
    {
      _httpClientFactory = httpClientFactory;
      this.logger = logger;
      options = optionsAccessor.Value;
    }

    public async Task<T> ExecuteAsync<T>(ShortenUrlRequest<T> request) where T : ShortenUrlResponse, new()
    {
      var client = _httpClientFactory.CreateClient(nameof(ShortenUrlClient));

      var postData = request.GetPostParameters();
      var body = await client.PostAsync(options.ApiUrl, new FormUrlEncodedContent(postData));
      var str = await body.Content.ReadAsStringAsync();
      var response = JsonSerializer.Deserialize<T>(str);

      return response;
    }
  }
}