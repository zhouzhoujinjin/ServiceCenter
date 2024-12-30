using System;
using System.Collections.Generic;
using System.Linq;

namespace PureCode.MultiTenants
{
  public class TenantsContainer
  {
    private readonly Dictionary<string, TenantContext> contexts;

    public TenantsContainer()
    {
      contexts = new Dictionary<string, TenantContext>();
    }

    public TenantContext GetContext(string id)
    {
      if (id == null)
      {
        throw new ArgumentNullException(nameof(id), $"{nameof(id)} is null");
      }
      var exists = contexts.TryGetValue(id, out var context);
      if (!exists)
      {
        return null;
      }
      return context;
    }

    public void Clear()
    {
      contexts.Clear();
    }

    public TenantContext GetContextByValue(string value)
    {
      if (value == null)
      {
        throw new ArgumentNullException(nameof(value), $"{nameof(value)} is null");
      }
      value = value.ToLower();
      var context = contexts.Values.Where(x => value == x.Host?.ToLower() || value == x.Id.ToLower() || x.Code?.ToLower() == value || x.Name?.ToLower() == value).FirstOrDefault();

      return context;
    }

    public void Register(TenantContext context)
    {
      contexts[context.Id] = context;
    }

    public void Remove(string contextId)
    {
      contexts.Remove(contextId);
    }
  }
}