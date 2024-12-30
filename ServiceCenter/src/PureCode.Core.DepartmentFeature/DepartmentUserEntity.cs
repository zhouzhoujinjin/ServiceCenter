using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.DepartmentFeature
{
  public class DepartmentUserEntity
  {
    public ulong Id { get; set; }

    public ulong DepartmentId { get; set; }

    public ulong UserId { get; set; }

    public string? Position { get; set; }

    public short? Level { get; set; }  

    public bool? IsUserMajorDepartment { get; set; }

    public DepartmentEntity Department { get; set; }



  }
}
