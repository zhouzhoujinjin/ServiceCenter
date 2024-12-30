using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PureCode.Core.Entities;
using PureCode.Core.Kernel;
using PureCode.Core.Models;
using PureCode.Core.ProfileFeature.Models;
using PureCode.Core.Utils;
using PureCode.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PureCode.Core.UserFeature;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PureCode.Core.Managers;

public class ProfileManager
{
  private readonly JsonSerializerOptions jsonSerializerOptions = JsonOptions.JsonSerializerDefaultOptions;

  private readonly UploadOptions uploadOptions;
  private readonly IDistributedCache cache;
  private readonly PureCodeDbContext context;
  private readonly SettingManager settingManager;
  private readonly DbSet<UserClaimEntity> userClaimSet;
  private readonly DbSet<UserProfileEntity> userProfileSet;
  internal static ProfileKeyMap ProfileKeyMap { get; private set; } = new();

  private static readonly object _locker = new();

  public ProfileManager(
    PureCodeDbContext context,
    SettingManager settingManager,
    IOptions<UploadOptions> uploadOptionsAccessor,
    IDistributedCache cache
  )
  {
    uploadOptions = uploadOptionsAccessor.Value;
    this.cache = cache;
    this.context = context;
    this.settingManager = settingManager;
    this.userProfileSet = context.Set<UserProfileEntity>();
    userClaimSet = context.Set<UserClaimEntity>();
  }

  internal async Task InitializeAsync()
  {
    var settings = await settingManager.GetGlobalSettingsAsync<ProfileKeySettings>();

    lock (_locker)
    {
      ProfileKeyMap.Value.Clear();
      foreach (var kv in settings.Values)
      {
        ProfileKeyMap.Value[kv.Key] = kv.Value;
        ProfileKeyMap.Value[kv.Key].ProfileType = ProfileKeyMap.ParseType(kv.Value.ProfileTypeClassName);
      }
    }
  }

  /// <summary>
  /// 上传用户头像
  /// </summary>
  /// <param name="image"></param>
  /// <param name="imageSaveFolder"></param>
  /// <returns></returns>
  public string? UploadImage(IFormFile image, string? imageSaveFolder = null)
  {
    if (string.IsNullOrEmpty(imageSaveFolder))
    {
      imageSaveFolder = "avatars";
    }

    var rootPath = Path.Combine(uploadOptions.AbsolutePath, imageSaveFolder);
    var fileName = UploadUtils.MoveFile(image, rootPath, false);
    if (string.IsNullOrEmpty(fileName)) return null;
    fileName = UploadUtils.GetUrl(fileName, rootPath, $"{uploadOptions.WebRoot}/{imageSaveFolder}");

    return fileName;
  }

  /// <summary>
  /// 上传用户头像
  /// </summary>
  /// <param name="base64"></param>
  /// <param name="imageSaveFolder"></param>
  /// <returns></returns>
  public string? UploadImage(string base64, string imageSaveFolder = "avatars")
  {
    var rootPath = Path.Combine(uploadOptions.AbsolutePath, imageSaveFolder);
    var fileName = UploadUtils.CreateBase64Image(base64, rootPath, false);
    if (string.IsNullOrEmpty(fileName)) return null;
    fileName = UploadUtils.GetUrl(fileName, rootPath, $"{uploadOptions.WebRoot}/{imageSaveFolder}");

    return fileName;
  }

  private string GetProfileClaimName(string profileKey)
  {
    return $"{ProfileKeys.ClaimTypePrefix}:{profileKey}";
  }

  /// <summary>
  /// 获得用户全部资料
  /// </summary>
  /// <param name="userId"></param>
  /// <returns></returns>
  public async Task<Dictionary<string, object?>> GetUserProfilesAsync(ulong userId)
  {
    var result = await cache.GetAsync(string.Format(CacheKeys.UserProfiles, userId), async () =>
    {
      var sql = userClaimSet
        .Where(uc => uc.UserId == userId && uc.ClaimType!.StartsWith($"{ProfileKeys.ClaimTypePrefix}:"))
        .Select(uc => new ProfileModel
        {
          Name = uc.ClaimType!,
          Value = uc.ClaimValue
        });
      var s = sql.ToQueryString();
      var profiles = await sql.ToListAsync();

      return profiles.ToDictionary(p => p.Name.Split(":")[1], DeserializeValue);
    }, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
    return result ?? new Dictionary<string, object?>();
  }

  public async Task<T?> GetUserProfileAsync<T>(ulong userId, string profileKey)
  {
    var profiles = await GetUserProfilesAsync(userId);
    var kv = profiles.FirstOrDefault(p => profileKey.Equals(p.Key, StringComparison.CurrentCultureIgnoreCase));
    return (T?)(kv.Value ?? default(T));
  }

  public async Task<object?> GetUserProfileAsync(ulong userId, string profileKey)
  {
    var profiles = await GetUserProfilesAsync(userId);
    return profiles.FirstOrDefault(p => profileKey.Equals(p.Key, StringComparison.CurrentCultureIgnoreCase));
  }

  public async Task<Dictionary<string, object?>> GetUserProfilesAsync(long userId)
  {
    var result = await cache.GetAsync(string.Format(CacheKeys.UserProfiles, userId), async () =>
    {
      var sql = userProfileSet.Include(up => up.ProfileKey)
        //.ThenInclude(p => p.ValueSpace)
        .Where(up => up.UserId == userId && up.ProfileKey.IsVisible)
        .Select(up => new ProfileModel
        {
          Id = up.Id,
          Name = up.ProfileKey.Name,
          CategoryCode = up.ProfileKey.CategoryCode,
          //ValueSpaceName = up.ProfileKey.ValueSpace.Name,
          //ProfileKeyName = up.ProfileKey.Name,
          Value = up.Value
        });
      var s = sql.ToQueryString();
      var profiles = await sql.ToListAsync();
      return profiles.ToDictionary(p => $"{p.Name}/{p.CategoryCode}", DeserializeValue);      
    }, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
    return result ?? new Dictionary<string, object?>();
  }

  private IEnumerable<string> TransformKeys(IEnumerable<string> profileKeys)
  {
    var pks = profileKeys.ToList();
    var pi = pks.IndexOf(SystemProfileKeyCategory.Public);
    if (pi > -1)
    {
      pks.RemoveAt(pi);
      pks.AddRange(ProfileKeyMap.GetPublicKeyNames());
    }

    var pb = pks.IndexOf(SystemProfileKeyCategory.Brief);
    if (pb > -1)
    {
      pks.RemoveAt(pb);
      pks.AddRange(ProfileKeyMap.GetBriefKeyNames());
    }

    var ps = pks.IndexOf(SystemProfileKeyCategory.Searchable);
    if (ps > -1)
    {
      pks.RemoveAt(ps);
      pks.AddRange(ProfileKeyMap.GetSearchableKeyNames());
    }

    return pks;
  }

  public async Task<Dictionary<string, object?>> GetProfilesAsync(ulong userId, IEnumerable<string> profileKeys)
  {
    var profiles = await GetUserProfilesAsync(userId);
    var pks = TransformKeys(profileKeys);

    return profiles.Where(
      p => pks.Contains(p.Key, StringComparer.CurrentCultureIgnoreCase)
    ).ToDictionary(p => p.Key.Split("/").First(), p => p.Value);
  }

  public async Task AddProfilesAsync(ulong userId, Dictionary<string, JsonElement?> profiles)
  {
    var dict = new Dictionary<string, object?>();
    foreach (var kv in profiles)
    {
      if (kv.Value == null)
      {
        continue;
      }

      var value = kv.Value.Value;
      switch (value.ValueKind)
      {
        case JsonValueKind.String:
          dict[kv.Key] = value.ToString();
          break;

        case JsonValueKind.Number:
          var result = value.TryGetInt32(out var v);
          dict[kv.Key] = result ? v : value.GetDouble();
          break;

        case JsonValueKind.True:
          dict[kv.Key] = true;
          break;

        case JsonValueKind.False:
          dict[kv.Key] = false;
          break;

        default:
          dict[kv.Key] = JsonSerializer.Serialize(value, jsonSerializerOptions);
          break;
      }
    }

    await AddProfilesAsync(userId, dict);
  }

  public async Task AddProfilesAsync(ulong userId, Dictionary<string, object?> profiles)
  {
    var dictProfileKeys = profiles.Keys.ToDictionary(k => k, GetProfileClaimName);
    var claimMap = await userClaimSet
      .Where(x => x.UserId == userId &&
                  dictProfileKeys.Values.Contains(x.ClaimType))
      .ToDictionaryAsync(x => x.ClaimType!, x => x);
    var cleanedProfiles = new Dictionary<string, object>();
    var allKeys = ProfileKeyMap.GetAllProfileKeys();

    foreach (var key in dictProfileKeys)
    {
      var profileKey = allKeys.FirstOrDefault(
        x => x.Name != null && x.Name.Equals(key.Key)
      ) ?? throw new PureCodeException($"资料信息中不存在 [{key.Key}]");
      cleanedProfiles[profileKey.Name!] = profiles[key.Key]!;
    }

    foreach (var (key, value) in cleanedProfiles)
    {
      if (claimMap.TryGetValue(GetProfileClaimName(key), out var v))
      {
        v.ClaimValue = ProfileSerialize(value);
      }
      else
      {
        claimMap[key] = new UserClaimEntity()
        {
          ClaimValue = ProfileSerialize(value),
          ClaimType = $"{ProfileKeys.ClaimTypePrefix}:{key}",
          UserId = userId
        };
        userClaimSet.Add(claimMap[key]);
      }
    }

    await context.SaveChangesAsync();

    await cache.RemoveAsync(string.Format(CacheKeys.UserProfiles, userId));
  }

  private object? DeserializeValue(ProfileModel profile)
  {
    if (profile.Value is string value)
    {
      var name = profile.Name.Split(":")[1];
      var valueType = ProfileKeyMap.Get(name).ProfileType;
      return valueType!.IsPrimitive || valueType == typeof(string) ||
             valueType == typeof(DateTime)
        ? Convert.ChangeType(profile.Value, valueType)
        : JsonSerializer.Deserialize(value, valueType);
    }
    else
    {
      return profile.Value;
    }
  }

  private string ProfileSerialize(object? value)
  {
    if (value == null)
    {
      return "";
    }

    var valueType = value.GetType();

    if (valueType.IsPrimitive || valueType == typeof(string) || valueType == typeof(DateTime))
    {
      return value.ToString() ?? string.Empty;
    }

    return JsonSerializer.Serialize(value, jsonSerializerOptions);
  }

  public async Task<ulong?> FindUserIdByUniqueProfileAsync(string profileKeyName, object? value)
  {
    var s = value?.ToString();
    if (s == null)
    {
      return null;
    }

    var fullName = $"{ProfileKeys.ClaimTypePrefix}:{profileKeyName}";

    return await userClaimSet
      .Where(p => p.ClaimType == fullName && p.ClaimValue != null &&
                  p.ClaimValue.Contains(s))
      .Select(p => p.UserId).FirstOrDefaultAsync();
  }
}