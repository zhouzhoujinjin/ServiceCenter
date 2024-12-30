using Microsoft.EntityFrameworkCore;
using PureCode.Core.Utils;
using PureCode.DataTransfer.Entities;

namespace PureCode.DataTransfer
{
  public class SheetContext : DbContext
  {
    public DbSet<FormEntity> FormSet { get; set; }
    public DbSet<CacheEntity> DataSet { get; set; }

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

    public SheetContext(DbContextOptions<SheetContext> options) : base(options)
    {
    }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<FormEntity>(entity =>
      {
        entity.ToTable("Sheet_Form").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.HasMany(e => e.Caches).WithOne(e => e.Form).HasForeignKey(e => e.FormId);
        entity.Property(e => e.Fields).HasJsonConversion();
        entity.Property(e => e.Filters).HasJsonConversion();
        entity.Property(e => e.AddonInfos).HasJsonConversion();
      });

      builder.Entity<CacheEntity>(entity =>
      {
        entity.ToTable("Sheet_Cache").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Filters).HasJsonConversion();
        entity.Property(e => e.Data).HasJsonConversion();
      });
    }
  }
}