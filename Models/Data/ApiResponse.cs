using System.Runtime.Serialization;

namespace PersonalWebApi.Models.Data;

/// --------------------------------------------------------------------------------
/// <summary>
/// APIレスポンスステータス
/// </summary>
/// --------------------------------------------------------------------------------
public enum ApiResponseStatus {
    Success = 0,
    Error = 1,
    NotFound = 2,
    Unauthorized = 3,
    ValidationError = 4,
    BadRequest = 5,
    InternalServerError = 6,
    NotImplemented = 7,
    ServiceUnavailable = 8,
    Forbidden = 9,
}

/// --------------------------------------------------------------------------------
/// <summary>
/// APIレスポンス
/// </summary>
/// --------------------------------------------------------------------------------
[DataContract]
public class ApiResponse<T>(ApiResponseStatus status, string message, T? data) {
    [DataMember]
    public ApiResponseStatus Status { get; set; } = status;
    [DataMember]
    public string Message { get; set; } = message;
    [DataMember]
    public T? Result { get; set; } = data;
}
