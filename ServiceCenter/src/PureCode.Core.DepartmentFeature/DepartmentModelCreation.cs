

using Microsoft.EntityFrameworkCore;
using PureCode.Core.Utils;

namespace PureCode.Core.DepartmentFeature
{
  public class DepartmentModelCreation : IModelCreation
  {
    public int Seq => (int)ModelCreatingPriority.Level2;

    public static void OnModelCreating(ModelBuilder builder, string? dbType = null)
    {
      builder.Entity<DepartmentEntity>(entity =>
      {
        entity.ToTable("Core_Department").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        //if (dbType == "oracle")
        //{
        //  entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_MENUITEM"));
        //}
      });
      builder.Entity<DepartmentUserEntity>(entity =>
      {
        entity.ToTable("Core_DepartmentUser").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.HasOne(e=>e.Department).WithMany(e=>e.Users).HasForeignKey(e=>e.DepartmentId);
        //if (dbType == "oracle")
        //{
        //  entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_MENUITEM"));
        //}
      });
    }
  }
}
