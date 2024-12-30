using System;

namespace PureCode.DataTransfer
{
  public class FieldAttribute : Attribute
  {
    public string Label { get; set; }

    public FieldAttribute(string label)
    {
      Label = label;
    }
  }
}