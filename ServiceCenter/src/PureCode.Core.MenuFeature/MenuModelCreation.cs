using Microsoft.EntityFrameworkCore;
using PureCode.Core.Entities;
using PureCode.Core.Utils;

namespace PureCode.Core.MenuFeature
{
  public class MenuModelCreation : IModelCreation
  {
    public int Seq => (int)ModelCreatingPriority.Level2;

    public static void OnModelCreating(ModelBuilder builder, string? dbType = null)
    {
      builder.Entity<MenuItemEntity>(entity =>
      {
        entity.ToTable("Core_MenuItem").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Type).HasConversion<string>();
        entity.Property(e => e.IconType).HasConversion<string>();

        if (dbType == "oracle")
        {
          entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_MENUITEM"));
        }
      });
    }
  }
}