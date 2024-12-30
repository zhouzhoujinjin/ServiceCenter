using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PureCode.Core.AIFeature
{
  public class AIManager
  {
    public AIManager()
    {
    }

    public async Task<AIReponseModel?> Ask(AIRepuestModel request)
    {
      HttpClient client = new HttpClient();
      string aiUri = "http://localhost:11434/api/generate";
      string content = System.Text.Json.JsonSerializer.Serialize(request);
      var resp = await client.PostAsync(aiUri, new StringContent(content, System.Text.Encoding.UTF8));
      string result = await resp.Content.ReadAsStringAsync();
      var answer = JsonSerializer.Deserialize<AIReponseModel>(result);
      return answer;
    }
  }
}
