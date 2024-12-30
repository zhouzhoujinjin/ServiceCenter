using System;

namespace PureCode.Core
{
  public class PureCodeException : Exception
  {
    public PureCodeException() : base()
    {
    }

    public PureCodeException(string message) : base(message)
    {
    }

    public static PureCodeException NonexistException(string name, string id) => new($"没有找到 [{name}] 类型的 [{id}] 的数据");

    public static PureCodeException UnauthorizedException(string permission) => new($"当前用户未登录，或没有 [{permission}] 权限");
  }
}