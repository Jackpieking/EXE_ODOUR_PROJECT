using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using FastEndpoints;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.System.Entities;

public sealed class JobRecordEntity : IEntity, IJobStorageRecord
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public string QueueID { get; set; } = default;

    public DateTime ExecuteAfter { get; set; }

    public DateTime ExpireOn { get; set; }

    public bool IsComplete { get; set; }

    public string CommandJson { get; set; } = default;

    [NotMapped]
    public object Command { get; set; } = default;

    TCommand IJobStorageRecord.GetCommand<TCommand>()
    {
        return JsonSerializer.Deserialize<TCommand>(json: CommandJson);
    }

    void IJobStorageRecord.SetCommand<TCommand>(TCommand command)
    {
        CommandJson = JsonSerializer.Serialize(value: command);
    }

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "JobRecords";

        public static class QueueID
        {
            public const int MinLength = default;
        }

        public static class CommandJson
        {
            public const int MinLength = default;
        }
    }
    #endregion
}
