namespace PureCode.Core
{
  public enum ModelCreatingPriority
  {
    First = 1,
    Last = 99,
    AfterDependencies = 90,
    Level1 = 10,
    Level2 = 20,
    Level3 = 30,
    Level4 = 40
  }

  public enum ConfigureLevel
  {
    System = 1,               // 系统级，不可修改
    Configurable = 2          // 可配置的
  }

  public enum ValueSpaceType
  {
    Code = 1,                   // 代码
    Regex = 2,                  // 正则表达式
    Range = 3                   // 范围
  }
}