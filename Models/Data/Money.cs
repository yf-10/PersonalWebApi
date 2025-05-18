using System.Runtime.Serialization;

namespace PersonalWebApi.Models.Data;

[DataContract]
public class Money(decimal amount, string? currencyCode) {
    [DataMember]
    public decimal Amount { get; private set; } = amount;
    [DataMember]
    public string CurrencyCode { get; private set; } = currencyCode ?? "JPY";
}