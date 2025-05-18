using System.Runtime.Serialization;

namespace PersonalWebApi.Models.Data;

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

[DataContract]
public class ApiResponseJson<T>(ApiResponseStatus status, string message, T? data) {
    [DataMember]
    public ApiResponseStatus Status { get; set; } = status;
    [DataMember]
    public string Message { get; set; } = message;
    [DataMember]
    public T? Data { get; set; } = data;
}
