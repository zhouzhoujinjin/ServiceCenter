using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PureCode.Core.Entities;
using PureCode.Core.SettingFeature;
using PureCode.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PureCode.Core.Managers
{
  public class SettingManager
  {
    private readonly DbSet<SettingEntity> settings;
    private readonly IDistributedCache? cache;
    private readonly PureCodeDbContext context;

    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
      Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      PropertyNameCaseInsensitive = true,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
      Converters =
      {
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
      }
    };

    public SettingManager(PureCodeDbContext context, IDistributedCache? cache)
    {
      this.context = context;
      settings = context.Set<SettingEntity>();
      this.cache = cache;
    }

    public async Task SaveInstanceSettingAsync<T>(string instanceType, long instanceId, T? setting) where T : new()
    {
      var type = typeof(T);
      var entity = await settings.SingleOrDefaultAsync(x => x.ClassName == type.FullName && x.InstanceId == instanceId);
      if (setting == null && entity != null)
      {
        context.Remove(entity);
        await context.SaveChangesAsync();
        return;
      }

      if (entity == null)
      {
        settings.Add(new SettingEntity
        {
          InstanceType = instanceType,
          InstanceId = instanceId,
          ClassName = type.FullName!,
          Value = JsonSerializer.Serialize(setting, jsonSerializerOptions)
        });
      }
      else
      {
        entity.Value = JsonSerializer.Serialize(setting, jsonSerializerOptions);
      }
      if (cache != null)
      {
        await cache.RemoveAsync(string.Format(CacheKeys.SettingInstanceKey, type.FullName, instanceType, instanceId));
      }
      await context.SaveChangesAsync();
    }

    public async Task SaveGlobalSettingAsync<T>(T setting) where T : new()
    {
      var type = typeof(T);
      var entity = await settings.SingleOrDefaultAsync(x => x.InstanceType == null && x.ClassName == type.FullName && x.InstanceId == 0);
      if (entity == null)
      {
        settings.Add(new SettingEntity
        {
          InstanceId = 0,
          ClassName = type.FullName!,
          Value = JsonSerializer.Serialize(setting, jsonSerializerOptions)
        });
      }
      else
      {
        entity.Value = JsonSerializer.Serialize(setting, jsonSerializerOptions);
      }
      if (cache != null)
      {
        await cache.RemoveAsync(string.Format(CacheKeys.SettingGlobalKey, type.FullName));
      }
      await context.SaveChangesAsync();
    }

    public async Task ResetInstanceSettingAsync<T>(string instanceType, long instanceId)
    {
      var type = typeof(T);
      var entity = await settings.SingleOrDefaultAsync(x => x.InstanceType == instanceType && x.InstanceId == instanceId && x.ClassName == type.FullName);
      if (entity != null)
      {
        settings.Remove(entity);
        await context.SaveChangesAsync();
      }
      if (cache != null)
      {
        await cache.RemoveAsync(string.Format(CacheKeys.SettingInstanceKey, type.FullName, instanceType, instanceId));
      }
    }

    public async Task<T> GetGlobalSettingsAsync<T>(T defaultValue) where T : new()
    {
      var type = typeof(T);

      if (cache != null)
      {
        var result = await cache.GetAsync<T>(string.Format(CacheKeys.SettingGlobalKey, type.FullName));
        if (result != null)
        {
          return result;
        }
      }

      var entity = await settings.SingleOrDefaultAsync(x => x.InstanceType == null && x.InstanceId == 0 && x.ClassName == type.FullName);
      if (entity is { Value: { } }) return JsonSerializer.Deserialize<T>(entity.Value, jsonSerializerOptions)!;
      var value = defaultValue ?? new T();
      entity = new SettingEntity()
      {
        InstanceType = null,
        InstanceId = 0,
        ClassName = type.FullName!,
        Value = JsonSerializer.Serialize(value, jsonSerializerOptions)
      };
      settings.Add(entity);
      await context.SaveChangesAsync();

      if (cache != null)
      {
        await cache.SetAsync(
          string.Format(CacheKeys.SettingGlobalKey, type.FullName),
          value,
          new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.MaxValue }
        );
      }
      return value;
    }

    public async Task<T> GetGlobalSettingsAsync<T>() where T : new()
    {
      return await GetGlobalSettingsAsync(new T());
    }

    public async Task<T> GetInstanceSettingAsync<T>(string instanceType, long instanceId) where T : new()
    {
      var type = typeof(T);

      if (cache != null)
      {
        var result = await cache.GetAsync<T>(string.Format(CacheKeys.SettingInstanceKey, type.FullName, instanceType, instanceId));
        if (result != null)
        {
          return result;
        }
      }

      var entity = await settings.SingleOrDefaultAsync(x => x.InstanceType == instanceType && x.InstanceId == instanceId && x.ClassName == type.FullName);
      if (entity is { Value: { } }) return JsonSerializer.Deserialize<T>(entity.Value, jsonSerializerOptions)!;
      var value = new T();
      entity = new SettingEntity
      {
        InstanceType = instanceType,
        InstanceId = instanceId,
        ClassName = type.FullName!,
        Value = JsonSerializer.Serialize(value, jsonSerializerOptions)
      };
      settings.Add(entity);
      await context.SaveChangesAsync();

      if (cache != null)
      {
        await cache.SetAsync(
          string.Format(CacheKeys.SettingInstanceKey, type.FullName, instanceType, instanceId),
          value,
          new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.MaxValue }
        );
      }

      return value;
    }
  }
}