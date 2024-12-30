using System;
using System.Text.Json.Serialization;

namespace PureCode.Utils
{
  [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
  public class JsonInterfaceConverterAttribute : JsonConverterAttribute
  {
    public JsonInterfaceConverterAttribute(Type converterType)
        : base(converterType)
    {
    }
  }
}