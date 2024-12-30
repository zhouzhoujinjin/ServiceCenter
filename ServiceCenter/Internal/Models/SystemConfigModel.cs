using PureCode.Core.Models;

namespace Internal.Models
{
  public class SystemConfigModel : GlobalSettings
  {
    public DateTime OpenStartDate { get; set; }
    public DateTime OpenEndDate { get; set; }
  }
}
