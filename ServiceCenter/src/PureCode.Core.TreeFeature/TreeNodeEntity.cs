using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.Entities
{
  public class TreeNodeEntity
  {
    public ulong Id { get; set; }
    public string InstanceType { get; set; } = string.Empty;
    public ulong InstanceId { get; set; }
    public ulong? ParentId { get; set; }
    public string ParentIds { get; set; } = string.Empty;
    public int Seq { get; set; }

    public Dictionary<string, object>? ExtendData { get; set; } = new Dictionary<string, object>();

    public TreeNodeEntity? Parent { get; set; }

    public List<TreeNodeEntity> Children { get; set; } = new List<TreeNodeEntity>();
  }
}