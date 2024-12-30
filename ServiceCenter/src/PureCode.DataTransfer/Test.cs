using PureCode.DataTransfer.Models;
using PureCode.DataTransfer.Providers;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PureCode.DataTransfer
{
  public class Contact
  {
    [Field("姓名")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
  }

  public class Student
  {
    [Field("学校代码")]
    [JsonPropertyName("school")]
    public string School { get; set; } = "";

    [Field("班级代码")]
    [JsonPropertyName("classCode")]
    public string ClassCode { get; set; } = "";

    [Field("学籍号")]
    [JsonPropertyName("studentNo")]
    public string StudentNo { get; set; } = "";

    [Field("联系人")]
    [JsonPropertyName("contacts")]
    public List<Contact>? Contacts { get; set; }
  }

  public class Entry
  {
    [Field("学生")]
    public Student? Student { get; set; }
  }

  public class Test
  {
    public static void Main()
    {
      var provider = new ExcelProvider();
      // provider.LoadTemplate("TestTemplates\\复杂表头左边有行头.xlsx", out var a, out var b);
      // provider.LoadTemplate("TestTemplates\\复杂表头.xlsx", out a, out b);
      provider.LoadTemplate("TestTemplates\\3列.xlsx", out var fieldTitles, out var addonInfos);
      if (fieldTitles == null) return;
      var props = new List<Field>
      {
        new Field
        {
          Title = "中学代码",
          Source = "Student.School"
        },
        new Field
        {
          Title = "班级代码",
          Source = "Student.Class"
        },
        new Field
        {
          Title = "学籍号",
          Source = "Student.StudentNo"
        },
        new Field
        {
          Title = "联系人2姓名",
          Source = "Student.Contacts.1.Name"
        },
        new Field
        {
          Title = "联系人1姓名",
          Source = "Student.Contacts.0.Name"
        }
      };
      //var random = new Random();
      //var entries = Enumerable.Repeat(0, 10).Select(i => new Entry
      //{
      //  Student = new Student
      //  {
      //    School = "a",
      //    Class = "b",
      //    StudentNo = "c",
      //    Contacts = new List<Contact> { new Contact { Name = "d" }, new Contact { Name = "e" } }
      //  }
      //});

      //var stream = provider.FillData("TestTemplates\\5列带数组.xlsx", entries, props, addonInfos);
      //using (var file = File.Open("TestTemplates\\output.xlsx", FileMode.Create))
      //{
      //  stream.Seek(0, SeekOrigin.Begin);
      //  stream.CopyTo(file);
      //}

      //stream = provider.FillData(entries);
      //using (var file = File.Open("TestTemplates\\output1.xlsx", FileMode.Create))
      //{
      //  stream.Seek(0, SeekOrigin.Begin);
      //  stream.CopyTo(file);
      //}
      provider.LoadData<Dictionary<string, string>>("TestTemplates\\output.xlsx", props, addonInfos);
    }
  }
}