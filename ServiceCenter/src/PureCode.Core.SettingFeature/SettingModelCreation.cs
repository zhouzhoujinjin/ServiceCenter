using Microsoft.EntityFrameworkCore;
using PureCode.Core.Entities;
using PureCode.Core.Utils;

namespace PureCode.Core.ModelCreations
{
  public class SettingModelCreation : IModelCreation
  {
    public int Seq => (int)ModelCreatingPriority.Level1;

    public static void OnModelCreating(ModelBuilder builder, string? dbType = null)
    {
      builder.Entity<SettingEntity>(entity =>
      {
        entity.ToTable("Core_Setting").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Ignore(e => e.IsGlobal);
        if(dbType != "oracle")
        {
          entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_SETTING"));
        }
      });
    }
  }
}
