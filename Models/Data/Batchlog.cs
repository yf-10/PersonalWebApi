using System.Runtime.Serialization;

namespace PersonalWebApi.Models.Data;

/// --------------------------------------------------------------------------------
/// <summary>
/// バッチログ
/// </summary>
/// --------------------------------------------------------------------------------
[DataContract]
public class Batchlog(BatchlogMain main, List<BatchlogDetail> details) {
    [DataMember]
    public BatchlogMain Main { get; set; } = main;
    [DataMember]
    public List<BatchlogDetail> Details { get; set; } = details;
}

/// --------------------------------------------------------------------------------
/// <summary>
/// バッチログステータス
/// </summary>
/// --------------------------------------------------------------------------------
public enum BatchlogStatus {
    /// <summary>
    /// 実行中
    /// </summary>
    Running = 0,
    /// <summary>
    /// 正常終了
    /// </summary>
    Complete = 3,
    /// <summary>
    /// 異常終了
    /// </summary>
    Abort = 9,
    Undefined = -1
}

/// --------------------------------------------------------------------------------
/// <summary>
/// バッチログメイン
/// </summary>
/// --------------------------------------------------------------------------------
[DataContract]
public class BatchlogMain(
    string uuid,
    BatchlogStatus status,
    string programId,
    string? programName = "undefined",
    DateTime? startTime = null,
    DateTime? endTime = null,
    string? createdBy = "unknown",
    string? updatedBy = "unknown",
    DateTime? createdAt = null,
    DateTime? updatedAt = null
) {
    [DataMember]
    public string Uuid { get; set; } = uuid;
    [DataMember]
    public BatchlogStatus Status { get; set; } = status;
    [DataMember]
    public string ProgramId { get; set; } = programId;
    [DataMember]
    public string? ProgramName { get; set; } = programName ?? "undefined";
    [DataMember]
    public DateTime? StartTime { get; set; } = startTime ?? DateTime.Now;
    [DataMember]
    public DateTime? EndTime { get; set; } = endTime;
    [DataMember]
    public string CreatedBy { get; set; } = createdBy ?? "unknown";
    [DataMember]
    public string UpdatedBy { get; set; } = updatedBy ?? "unknown";
    [DataMember]
    public DateTime CreatedAt { get; set; } = createdAt ?? DateTime.Now;
    [DataMember]
    public DateTime UpdatedAt { get; set; } = updatedAt ?? DateTime.Now;
}

/// --------------------------------------------------------------------------------
/// <summary>
/// バッチログ詳細
/// </summary>
/// --------------------------------------------------------------------------------
[DataContract]
public class BatchlogDetail(
    string uuid,
    int logNo = 0,
    string? logMsg = null,
    DateTime? logTime = null,
    string? createdBy = "unknown",
    string? updatedBy = "unknown",
    DateTime? createdAt = null,
    DateTime? updatedAt = null
) {
    [DataMember]
    public string Uuid { get; private set; } = uuid;
    [DataMember]
    public int LogNo { get; private set; } = logNo;
    [DataMember]
    public string? LogMsg { get; set; } = logMsg;
    [DataMember]
    public DateTime LogTime { get; set; } = logTime ?? DateTime.Now;
    [DataMember]
    public string CreatedBy { get; set; } = createdBy ?? "unknown";
    [DataMember]
    public string UpdatedBy { get; set; } = updatedBy ?? "unknown";
    [DataMember]
    public DateTime? CreatedAt { get; set; } = createdAt ?? DateTime.Now;
    [DataMember]
    public DateTime? UpdatedAt { get; set; } = updatedAt ?? DateTime.Now;
}

/// --------------------------------------------------------------------------------
/// <summary>
/// バッチログ開始リクエスト
/// </summary>
/// --------------------------------------------------------------------------------
[DataContract]
public class BatchlogBeginRequest {
    [DataMember]
    public string ProgramId { get; set; } = "undefined";
    [DataMember]
    public string? ProgramName { get; set; } = "undefined";
    [DataMember]
    public string? UserName { get; set; } = "unknown";
}

/// --------------------------------------------------------------------------------
/// <summary>
/// バッチログ追加リクエスト
/// </summary>
/// --------------------------------------------------------------------------------
[DataContract]
public class BatchlogAddRequest {
    [DataMember]
    public string? Uuid { get; private set; }
    [DataMember]
    public string? LogMsg { get; set; }
    [DataMember]
    public string? UserName { get; set; } = "unknown";
}