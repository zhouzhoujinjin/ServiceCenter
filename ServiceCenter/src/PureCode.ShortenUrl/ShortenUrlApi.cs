using Microsoft.Extensions.Options;
using PureCode.ShortenUrl.Requests;
using System;
using System.Threading.Tasks;

namespace PureCode.ShortenUrl
{
  public class ShortenUrlApi
  {
    private readonly ShortenUrlClient client;
    private readonly string key;

    public ShortenUrlApi(ShortenUrlClient client, IOptions<ShortenUrlOptions> optionsAccessor)
    {
      this.client = client;
      key = optionsAccessor.Value.SecretKey;
    }

    public async Task<string> CreateAsync(string url, string? title = null, string? keyword = null)
    {
      var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
      var token = Helper.CreateToken(timestamp, key);
      var request = new CreateRequest(url) { Title = title, Keyword = keyword, Timestamp = timestamp, Token = token };
      var response = await client.ExecuteAsync(request);
      return response.ShortUrl;
    }
  }
}