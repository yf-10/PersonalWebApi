using System.Text.RegularExpressions;

using PersonalWebApi.Models.Data;

namespace PersonalWebApi.Models.Service;
/// --------------------------------------------------------------------------------
/// <summary>
/// 給与メール本文から給与情報リストを生成するパーサ
/// </summary>
/// --------------------------------------------------------------------------------
public static partial class SalaryMailParser {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// メール本文から給与情報を解析し給与情報リストを生成する
    /// </summary>
    /// <param name="mailBody">メール本文</param>
    /// <param name="createdBy">作成者</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// --------------------------------------------------------------------------------
    public static List<Salary> Parse(string mailBody, string createdBy = "unknown", string updatedBy = "unknown") {
        var result = new List<Salary>();

        // [ 振込日 ] から年月を抽出し"YYYYMM"形式に変換
        var dateMatch = PaymentDateRegx().Match(mailBody);
        string month = dateMatch.Success
            ? DateTime.Parse(dateMatch.Groups["date"].Value).ToString("yyyyMM")
            : throw new ArgumentException("メール本文に[振込日]が見つかりません。");

        // [ 支　給：円 ] セクション抽出
        var paySection = PaymentRegx().Match(mailBody).Groups["pay"].Value;
        // [ 控　除：円 ] セクション抽出
        var deductionSection = DeductionRegx().Match(mailBody).Groups["deduction"].Value;

        // 支給明細の抽出
        foreach (Match m in PaymentItemRegx().Matches(paySection)) {
            var item = m.Groups["item"].Value.Trim();
            var amount = decimal.Parse(m.Groups["amount"].Value.Replace(",", ""));
            result.Add(new Salary(
                month: month,
                deduction: false,
                paymentItem: item,
                money: new Money(amount, "JPY"),
                createdBy: createdBy,
                updatedBy: updatedBy
            ));
        }

        // 控除明細の抽出
        foreach (Match m in PaymentItemRegx().Matches(deductionSection)) {
            var item = m.Groups["item"].Value.Trim();
            var amount = decimal.Parse(m.Groups["amount"].Value.Replace(",", ""));
            result.Add(new Salary(
                month: month,
                deduction: true,
                paymentItem: item,
                money: new Money(amount, "JPY"),
                createdBy: createdBy,
                updatedBy: updatedBy
            ));
        }

        return result;
    }

    [GeneratedRegex(@"\[ 支　給：円 \](?<pay>[\s\S]*?)(?=\[|$)")]
    private static partial Regex PaymentRegx();
    [GeneratedRegex(@"\[ 控　除：円 \](?<deduction>[\s\S]*?)(?=\[|$)")]
    private static partial Regex DeductionRegx();
    [GeneratedRegex(@"\[ 振込日 \]\s*(?<date>\d{4}/\d{2}/\d{2})")]
    private static partial Regex PaymentDateRegx();
    [GeneratedRegex(@"(?<item>[\S ]+?)\s*:\s*(?<amount>[\d,]+)")]
    private static partial Regex PaymentItemRegx();
}