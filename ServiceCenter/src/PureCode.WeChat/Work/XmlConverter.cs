using System.IO;
using System.Xml.Serialization;

namespace PureCode.WeChat.Work
{
  public class XmlConverter
  {
    public static T Deserialize<T>(string xml)
    {
      XmlSerializer serializer = new XmlSerializer(typeof(T));
      StringReader reader = new StringReader(xml);
      T result = (T)(serializer.Deserialize(reader));
      reader.Close();
      reader.Dispose();
      return result;
    }
  }
}