using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.Models
{
  /// <summary>
  /// 资料项
  /// </summary>
  public class ProfileModel
  {
    public ulong Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public object? Value { get; set; }

    public string CategoryCode { get; set; }
  }
}