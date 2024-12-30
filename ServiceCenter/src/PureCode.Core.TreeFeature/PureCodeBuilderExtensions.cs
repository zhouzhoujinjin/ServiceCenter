using Microsoft.Extensions.DependencyInjection;
using PureCode.Core.Managers;
using PureCode.Core.SettingFeature;

namespace PureCode.Core.TreeFeature
{
  public static class PureCodeBuilderExtensions
  {
    public static PureCodeServiceCollectionBuilder WithTree(this PureCodeServiceCollectionBuilder builder)
    {
      if (!builder.Services.Where(x => x.ServiceType == typeof(SettingManager)).Any())
      {
        throw new PureCodeException($"{nameof(TreeFeature)} 需要 ${nameof(SettingFeature)} 支持，请添加 WithSetting 支持");
      }
      builder.Services.AddScoped<TreeNodeManager>();
      return builder;
    }

    private static HashSet<string> typeSet = new();

    public static void AddTreeType(this PureCodeServiceCollectionBuilder builder, string type)
    {
      if (typeSet.Contains(type))
      {
        throw new PureCodeException($"在其他模块中已经注册了 [${type}] 类型的树类型，请考虑其他名称");
      }
      typeSet.Add(type);
    }

    public static void AddTreeType<T>(this PureCodeServiceCollectionBuilder builder)
    {
      builder.AddTreeType(typeof(T).FullName!);
    }
  }
}