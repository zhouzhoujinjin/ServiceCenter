using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Data;

namespace PureCode.Core.Utils
{
  public class ULongSequenceValueGenerator : ValueGenerator<ulong>
  {
    private readonly string? _schema;
    private readonly string _sequenceName;

    public ULongSequenceValueGenerator(string sequenceName, string? schema = null)
    {
      _schema = schema;
      _sequenceName = sequenceName;
    }

    public override bool GeneratesTemporaryValues => false;

    public override ulong Next(EntityEntry entry)
    {
      using var command = entry.Context.Database.GetDbConnection().CreateCommand();
      command.CommandText = $"SELECT {(string.IsNullOrEmpty(_schema) ? "" : $"{_schema}.")}{_sequenceName}.NEXTVAL FROM DUAL";
      entry.Context.Database.OpenConnection();
      using var reader = command.ExecuteReader();
      reader.Read();

      return (ulong) reader.GetInt64(0);
    }
  }

  public class LongSequenceValueGenerator : ValueGenerator<long>
  {
    private readonly string? _schema;
    private readonly string _sequenceName;

    public LongSequenceValueGenerator(string sequenceName, string? schema = null)
    {
      _schema = schema;
      _sequenceName = sequenceName;
    }

    public override bool GeneratesTemporaryValues => false;

    public override long Next(EntityEntry entry)
    {
      using var command = entry.Context.Database.GetDbConnection().CreateCommand();
      command.CommandText = $"SELECT {(string.IsNullOrEmpty(_schema) ? "" : $"{_schema}.")}{_sequenceName}.NEXTVAL FROM DUAL";
      entry.Context.Database.OpenConnection();
      using var reader = command.ExecuteReader();
      reader.Read();
      
      return reader.GetInt64(0);
    }
  }

  public class IntSequenceValueGenerator : ValueGenerator<int>
  {
    private readonly string? _schema;
    private readonly string _sequenceName;

    public IntSequenceValueGenerator(string sequenceName, string? schema = null)
    {
      _schema = schema;
      _sequenceName = sequenceName;
    }

    public override bool GeneratesTemporaryValues => false;

    public override int Next(EntityEntry entry)
    {
      using var command = entry.Context.Database.GetDbConnection().CreateCommand();
      command.CommandText = $"SELECT {(string.IsNullOrEmpty(_schema) ? "" : $"{_schema}.")}{_sequenceName}.NEXTVAL FROM DUAL";
      entry.Context.Database.OpenConnection();
      using var reader = command.ExecuteReader();
      reader.Read();

      return reader.GetInt32(0);
    }
  }
}