using PureCode.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureCode.Core.ProfileFeature.Models
{
  public class ProfileKeySettings
  {
    public Dictionary<string, ProfileKeyModel> Values { get; set; } = new Dictionary<string, ProfileKeyModel>();
  }
}