using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.DepartmentFeature
{
  public class DepartmentEntity:IEntity
  {
    public ulong Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ulong CreatorId { get; set; }
    public DateTime? CreatedTime { get; set; }
    public ICollection<DepartmentUserEntity> Users { get; set; }
  }
}
