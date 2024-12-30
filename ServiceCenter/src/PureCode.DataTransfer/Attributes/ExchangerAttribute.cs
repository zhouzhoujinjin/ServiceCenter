using System;

namespace PureCode.DataTransfer
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class ExchangerAttribute : Attribute
  {
    public string Title { get; set; }

    public ExchangerAttribute(string title)
    {
      Title = title;
    }
  }
}