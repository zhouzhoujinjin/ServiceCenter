using System;

namespace PureCode.Core.Entities
{
  public class ProfileKeyEntity
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public string CategoryCode { get; set; }
    public Type? ProfileType { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsBrief { get; set; }
    public bool IsVisible { get; set; }
    public long ValueSpaceId { get; set; }
    //public ValueSpaceEntity ValueSpace { get; set; }
  }
}