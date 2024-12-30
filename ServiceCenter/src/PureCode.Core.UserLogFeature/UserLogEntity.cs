using System;

namespace PureCode.Core.Entities
{
  public class UserLogEntity
  {
    public ulong Id { get; set; }
    public ulong UserId { get; set; }

    public string Url { get; set; }

    public string Ip { get; set; }

    public string Method { get; set; }

    public string Device { get; set; }

    public string UserAgent { get; set; }

    public string Data { get; set; }

    public DateTime CreatedTime { get; set; }

    public int Duration { get; set; }
  }
}