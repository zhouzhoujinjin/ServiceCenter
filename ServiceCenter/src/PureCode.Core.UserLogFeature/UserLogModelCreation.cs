using Microsoft.EntityFrameworkCore;
using PureCode.Core.Entities;
using PureCode.Core.Utils;

namespace PureCode.Core.ModelCreations
{
  public class UserLogModelCreation : IModelCreation
  {
    public int Seq => (int)ModelCreatingPriority.Level3;

    public static void OnModelCreating(ModelBuilder builder, string? dbType = null)
    {
      builder.Entity<UserLogEntity>(entity =>
      {
        entity.ToTable("Core_UserLog").HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        if(dbType == "oracle")
        {
          entity.Property(e => e.Id).HasValueGenerator((_, __) => new LongSequenceValueGenerator("S_CORE_USERLOG"));
        }
      });
    }
  }
}
