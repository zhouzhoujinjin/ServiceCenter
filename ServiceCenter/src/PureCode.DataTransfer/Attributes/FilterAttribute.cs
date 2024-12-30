using System;

namespace PureCode.DataTransfer
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class FilterAttribute : Attribute
  {
    public string Title { get; set; }

    public FilterAttribute(string title)
    {
      Title = title;
    }
  }
}