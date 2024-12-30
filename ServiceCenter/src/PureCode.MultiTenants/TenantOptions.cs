using System.Collections.Generic;

namespace PureCode.MultiTenants
{
  public enum TenantDiscoveryType
  {
    Domain,
    RefererUrl,
    Header
  }

  public class TenantContext
  {
    public string Id { get; set; }
    public string Host { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
  }

  public class TenantOptions
  {
    public TenantDiscoveryType DiscoveryType { get; set; }
    public string HeaderKey { get; set; }
    public string RefererRegexPattern { get; set; }
    public List<TenantContext> Tenants { get; set; }
  }
}