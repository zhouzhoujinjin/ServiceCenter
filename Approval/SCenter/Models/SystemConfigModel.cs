using CyberStone.Core.Models;

namespace SCenter.Models
{
  public class SystemConfigModel : GlobalSettings
  {
    public DateTime OpenStartDate { get; set; }
    public DateTime OpenEndDate { get; set; }
  }
}
