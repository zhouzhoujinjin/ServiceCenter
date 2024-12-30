using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.AIFeature
{
  [ApiController]
  [Route("/api/ai", Name = "人工智能")]
  public class AIController(AIManager aiManager) : ControllerBase
  {
    public async Task<AjaxResponse> IndexAsync()
    {
      return await Task.FromResult(new AjaxResponse { Message = "fuck" });
    }

    [HttpPost("ask", Name = "提问")]
    public async Task<AjaxResponse<AIReponseModel>> AskAsync([FromBody] string askContent)
    {
      var request = new AIRepuestModel()
      {
        Model = ModelInstance.Gemma7b,
        Prompt = askContent,
        Stream=false,
      };

      var response = await aiManager.Ask(request);
      return new AjaxResponse<AIReponseModel>
      {       
        Data = response,
      };
    }
  }
}
