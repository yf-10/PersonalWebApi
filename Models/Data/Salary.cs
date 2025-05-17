using System.Runtime.Serialization;

namespace PersonalWebApi.Models.Data;

[DataContract]
public class Salary(string month, bool deduction, string paymentItem, Money money) {
    [DataMember]
    public string Month { get; private set; } = month;
    [DataMember]
    public bool Deduction { get; private set; } = deduction;
    [DataMember]
    public string PaymentItem { get; private set; } = paymentItem;
    [DataMember]
    public Money Money { get; private set; } = money;
}
