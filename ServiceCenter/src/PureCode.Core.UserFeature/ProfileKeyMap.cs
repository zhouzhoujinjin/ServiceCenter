using PureCode.Core.Kernel;
using PureCode.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PureCode.Core;

public class ProfileKeyMap
{
  public static SortedDictionary<string, ProfileKeyModel> Value { get; set; } = [];

  internal static Type ParseType(string className)
  {
    return className switch
    {
      DefaultProfileKeyClassTypes.Bool => typeof(bool),
      DefaultProfileKeyClassTypes.String => typeof(string),
      DefaultProfileKeyClassTypes.Number => typeof(double),
      DefaultProfileKeyClassTypes.Date => typeof(DateTime),
      _ => Type.GetType(className) ?? typeof(object)
    };
  }

  public ProfileKeyModel Get(string name)
  {
    return Value[name];
  }

  public IEnumerable<ProfileKeyModel> GetAllProfileKeys()
  {
    return Value.Values;
  }

  public IEnumerable<ProfileKeyModel> GetBriefKeys()
  {
    return Value.Values.Where(x => x.IsBrief);
  }

  public IEnumerable<string> GetPublicKeyNames()
  {
    return Value.Values.Where(x => x.IsPublic).Select(x => x.Name!);
  }

  public IEnumerable<string> GetSearchableKeyNames()
  {
    return Value.Values.Where(x => x.Searchable).Select(x => x.Name!);
  }

  public IEnumerable<string> GetBriefKeyNames()
  {
    return Value.Values.Where(x => x.IsBrief).Select(x => x.Name!);
  }

  public IEnumerable<ProfileKeyModel> GetFilteredProfileKeys(IEnumerable<string>? profileKeysOrCategories)
  {
    if (profileKeysOrCategories == null)
    {
      return GetBriefKeys();
    }

    return Value.Values.Where(x =>
    {
      var keysOrCategories = profileKeysOrCategories as string[] ?? profileKeysOrCategories.ToArray();
      return keysOrCategories.Contains(x.Name, StringComparer.OrdinalIgnoreCase) ||
             keysOrCategories.Contains(x.CategoryCode, StringComparer.OrdinalIgnoreCase);
    });
  }

  public IEnumerable<string> GetFilteredProfileKeyNames(IEnumerable<string>? profileKeysOrCategories)
  {
    if (profileKeysOrCategories == null)
    {
      return GetBriefKeyNames();
    }

    var pkoc = profileKeysOrCategories.Select(x => x.ToUpper()).ToArray();
    return Value.Values.Where(x =>
        pkoc.Contains(x.Name, StringComparer.OrdinalIgnoreCase) ||
        pkoc.Contains(x.CategoryCode, StringComparer.OrdinalIgnoreCase))
      .Select(x => x.Name!);
  }
}