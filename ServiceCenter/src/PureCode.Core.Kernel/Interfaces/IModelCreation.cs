using Microsoft.EntityFrameworkCore;

namespace PureCode.Core
{
  public interface IModelCreation
  {
    public int Seq { get; }

    abstract static void OnModelCreating(ModelBuilder builder, string? dbType = null);
  }
}