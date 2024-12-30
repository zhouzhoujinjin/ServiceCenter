using Microsoft.EntityFrameworkCore;
using PureCode.Core.Entities;
using PureCode.Core.Utils;

namespace PureCode.Core.ModelCreations
{
  public class ValueSpaceModelCreation : IModelCreation
  {
    public int Seq => (int)ModelCreatingPriority.Level1;

    public static void OnModelCreating(ModelBuilder builder, string? dbType = null)
    {
      builder.Entity<ValueSpaceEntity>(entity =>
      {
        entity.ToTable("Core_ValueSpace").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Ignore(e => e.ItemList);
        entity.Property(e => e.ValueSpaceType).HasColumnName("Type").HasConversion<string>();
        entity.Property(e => e.ConfigureLevel).HasColumnName("ConfigureLevelCode").HasConversion<string>();

        if(dbType == "oracle")
        {
          entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_VALUESPACE"));
        }
      });
    }
  }
}