using PureCode.Core.TreeFeature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.Models
{
  public class TreeNodeModel<TData> : ITreeNode<TreeNodeModel<TData>>
  {
    public TData? Data { get; set; }
    public Dictionary<string, object>? ExtendData { get; set; }
    public IEnumerable<TreeNodeModel<TData>>? Children { get; set; }
  }
}