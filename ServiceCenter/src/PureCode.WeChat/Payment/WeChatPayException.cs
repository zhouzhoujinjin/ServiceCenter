using System;

namespace PureCode.WeChat.Payment
{
  public class WeChatPayException : Exception
  {
    public WeChatPayException()
    {
    }

    public WeChatPayException(string messages) : base(messages)
    {
    }

    public WeChatPayException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}