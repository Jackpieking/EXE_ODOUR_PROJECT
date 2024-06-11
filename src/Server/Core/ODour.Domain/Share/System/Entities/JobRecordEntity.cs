using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using FastEndpoints;

namespace ODour.Domain.Share.System.Entities;

public sealed class JobRecordEntity : IJobStorageRecord
{
    #region PrimaryKey
    public Guid Id { get; set; }
    #endregion

    public string QueueID { get; set; }

    public DateTime ExecuteAfter { get; set; }

    public DateTime ExpireOn { get; set; }

    public bool IsComplete { get; set; }

    public string CommandJson { get; set; }

    public int FailureCount { get; set; }

    public DateTime CancelledOn { get; set; }

    public string FailureReason { get; set; }

    [NotMapped]
    public object Command { get; set; }

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

        public static class FailureCount
        {
            public const int MinValue = default;
        }

        public static class FailureReason
        {
            public const int MinLength = default;
        }
    }
    #endregion
}
