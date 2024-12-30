using PureCode.DataTransfer.Models;
using System.Collections.Generic;

namespace PureCode.DataTransfer.Entities
{
  public class FormEntity
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public bool IsBuiltIn { get; set; }
    public string TemplatePath { get; set; }

    /// <summary>
    /// 记录的类型
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// 过滤器
    /// </summary>
    public List<string> Filters { get; set; }

    /// <summary>
    /// 域
    /// </summary>
    public List<Field> Fields { get; set; }

    /// <summary>
    /// 附加内容
    /// </summary>
    public Dictionary<string, object?> AddonInfos { get; set; }

    /// <summary>
    /// 是否支持导入
    /// </summary>
    public bool IsSupportImport { get; set; }

    /// <summary>
    /// 导入时列是否有序，或者只有部分列
    /// </summary>
    public bool OutOfOrder { get; set; }

    public virtual ICollection<CacheEntity> Caches { get; set; }

    public FormEntity()
    {
      Caches = new List<CacheEntity>();
      Filters = new List<string>();
      AddonInfos = new Dictionary<string, object?>();
    }
  }
}