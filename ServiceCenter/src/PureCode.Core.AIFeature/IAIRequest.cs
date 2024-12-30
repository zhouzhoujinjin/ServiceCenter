using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.AIFeature
{
  public interface IAIRequest
  {
    //模型
    string Model { get; set; }
    //请求主动方
    string Prompt { get; set; }
    //返回内容是否为stream
    bool? Stream { get; set; }
    //返回内容格式：json|xml
    string? Format { get; set; }

  }

  public interface IAIResponse
  {
    string Model { get; set; }
    string Response { get; set; }

  }


}
