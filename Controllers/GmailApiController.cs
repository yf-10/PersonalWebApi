using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.Service;

namespace PersonalWebApi.Controllers;
/// --------------------------------------------------------------------------------
/// <summary>
/// Gmail APIコントローラー
/// </summary>
/// --------------------------------------------------------------------------------
[ApiController]
[Route("api/gmail")]
public class GmailApiController(ILogger<GmailApiController> logger, IOptionsSnapshot<AppSettings> options) : BaseController(logger, options) {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// メールから給与データを取得する
    /// </summary>
    [HttpGet("salaries/latest")]
    /// --------------------------------------------------------------------------------
    public async Task<IActionResult> GetLatestSalary() {
        try {
            GmailApiService gmailApiService = new(_logger, _options);
            var salaries = await gmailApiService.GetLatestSalaryFromGmailAsync();
            if (salaries == null || salaries.Count == 0)
                return NotFound("給与データが見つかりませんでした。");
            return Ok(new ApiResponse<List<Salary>>(ApiResponseStatus.Success, "Salaries retrieved from gmal successfully.", salaries));
        } catch (Exception ex) {
            _logger.LogError(ex, "給与データ取得中にエラーが発生しました。");
            return StatusCode(500, "給与データ取得中にエラーが発生しました: " + ex.Message);
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// メールから給与データを取得しDBに登録する
    /// </summary>
    [HttpPost("salaries/import")]
    /// --------------------------------------------------------------------------------
    public async Task<IActionResult> ImportLatestSalary() {
        try {
            // 最新の給与データをGmailから取得
            GmailApiService gmailApiService = new(_logger, _options);
            var salaries = await gmailApiService.GetLatestSalaryFromGmailAsync();
            if (salaries == null || salaries.Count == 0)
                return NotFound("給与データが見つかりませんでした。");

            // データ登録（主キー重複時は更新）
            var salaryService = new SalaryService(_logger, _options);
            int count = salaryService.InsertOrUpdateAll(salaries);

            return Ok(new ApiResponse<int>(ApiResponseStatus.Success, $"{count} 件の給与データを登録・更新しました。", count));
        } catch (Exception ex) {
            _logger.LogError(ex, "給与データの登録中にエラーが発生しました。");
            return StatusCode(500, "給与データの登録中にエラーが発生しました: " + ex.Message);
        }
    }

}
