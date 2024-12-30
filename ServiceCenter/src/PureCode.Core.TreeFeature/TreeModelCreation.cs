using Microsoft.EntityFrameworkCore;
using PureCode.Core.Entities;
using PureCode.Core.Extensions;
using PureCode.Core.Utils;

namespace PureCode.Core.Modules
{
  public class TreeModelCreation : IModelCreation
  {
    public int Seq => (int)ModelCreatingPriority.Level1;

    public static void OnModelCreating(ModelBuilder builder, string? dbType = null)
    {
      builder.Entity<TreeNodeEntity>(entity =>
      {
        entity.ToTable("Core_TreeNode").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.ExtendData).HasJsonConversion();
        entity.HasOne(e => e.Parent).WithMany(e => e.Children).HasForeignKey(e => e.ParentId);

        if (dbType == "oracle")
        {
          entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_TREENODE"));
        }
      });
    }
  }
}