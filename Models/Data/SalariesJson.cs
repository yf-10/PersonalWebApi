using System.Runtime.Serialization;

namespace PersonalWebApi.Models.Data;

[DataContract]
public class SalariesJson(List<Salary> salaries) {
    [DataMember]
    public List<Salary> Salaries { get; private set; } = salaries;
}
