using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.AIFeature
{
  public class ModelInstance
  {
    public const string Gemma7b = "gemma:7b";
    public const string Gemma7b_all = "gemma:7b-instruct-fp16";
    public const string Gemma2b = "gemma:2b";
  }

  public class AIType
  {
    public const string Generate = "generate";
    public const string Chat = "chat";
  }


}
