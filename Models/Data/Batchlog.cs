using System.Runtime.Serialization;

namespace PersonalWebApi.Models.Data;

/// <summary>
/// Represents the main batch log entity, including its details.
/// </summary>
[DataContract]
public class BatchlogMain(string uuid, string status, string programId, string programName, DateTime? startTime, DateTime? endTime, List<BatchlogDetail> details) {
    /// <summary>
    /// Unique identifier for the batch log.
    /// </summary>
    [DataMember]
    public string Uuid { get; private set; } = uuid;

    /// <summary>
    /// Status of the batch log.
    /// </summary>
    [DataMember]
    public string Status { get; private set; } = status;

    /// <summary>
    /// Program ID associated with the batch log.
    /// </summary>
    [DataMember]
    public string ProgramId { get; private set; } = programId;

    /// <summary>
    /// Program name associated with the batch log.
    /// </summary>
    [DataMember]
    public string ProgramName { get; private set; } = programName;

    /// <summary>
    /// Start time of the batch process.
    /// </summary>
    [DataMember]
    public DateTime? StartTime { get; private set; } = startTime;

    /// <summary>
    /// End time of the batch process.
    /// </summary>
    [DataMember]
    public DateTime? EndTime { get; private set; } = endTime;

    /// <summary>
    /// List of detail logs associated with this batch log.
    /// </summary>
    [DataMember]
    public List<BatchlogDetail> Details { get; set; } = details;
}

/// <summary>
/// Represents a detail record for a batch log.
/// </summary>
[DataContract]
public class BatchlogDetail(string batchlogUuid, int id, string message, string logTime) {
    /// <summary>
    /// The UUID of the parent batch log.
    /// </summary>
    [DataMember]
    public string BatchlogUuid { get; private set; } = batchlogUuid;

    /// <summary>
    /// Unique identifier for the detail record.
    /// </summary>
    [DataMember]
    public int Id { get; private set; } = id;

    /// <summary>
    /// Message or log content for this detail.
    /// </summary>
    [DataMember]
    public string Message { get; private set; } = message;

    /// <summary>
    /// Log time for this detail record.
    /// </summary>
    [DataMember]
    public string LogTime { get; private set; } = logTime;
}
