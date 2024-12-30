using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.TreeFeature
{
  public interface ITreeNode<T>
  {
    IEnumerable<T>? Children { get; set; }
  }
}