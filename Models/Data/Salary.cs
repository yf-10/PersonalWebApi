using System.Runtime.Serialization;

namespace PersonalWebApi.Models.Data;

/// --------------------------------------------------------------------------------
/// <summary>
/// 給与情報
/// </summary>
/// --------------------------------------------------------------------------------
[DataContract]
public class Salary(
    string month,
    bool deduction,
    string paymentItem,
    Money money,
    string createdBy = "unknown",
    string updatedBy = "unknown",
    DateTime? createdAt = null,
    DateTime? updatedAt = null,
    int exclusiveFlag = 0
) {
    [DataMember]
    public string Month { get; private set; } = month;
    [DataMember]
    public bool Deduction { get; private set; } = deduction;
    [DataMember]
    public string PaymentItem { get; private set; } = paymentItem;
    [DataMember]
    public Money Money { get; private set; } = money;
    [DataMember]
    public string CreatedBy { get; private set; } = createdBy;
    [DataMember]
    public string UpdatedBy { get; private set; } = updatedBy;
    [DataMember]
    public DateTime CreatedAt { get; private set; } = createdAt ?? DateTime.Now;
    [DataMember]
    public DateTime UpdatedAt { get; private set; } = updatedAt ?? DateTime.Now;
    [DataMember]
    public int ExclusiveFlag { get; private set; } = exclusiveFlag;
}

/// --------------------------------------------------------------------------------
/// <summary>
/// 通貨と金額を表すクラス
/// 通常はJPY（日本円）を使用するが、必要に応じて他の通貨コードも指定可能
/// </summary>
/// --------------------------------------------------------------------------------
[DataContract]
public class Money(decimal amount, string? currencyCode) {
    [DataMember]
    public decimal Amount { get; private set; } = amount;
    [DataMember]
    public string CurrencyCode { get; private set; } = currencyCode ?? "JPY";    
}