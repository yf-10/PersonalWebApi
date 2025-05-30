using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.Service;

namespace PersonalWebApi.Controllers;
/// --------------------------------------------------------------------------------
/// <summary>
/// 給与コントローラー
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// --------------------------------------------------------------------------------
[ApiController]
[Route("api/salaries")]
public class SalaryController(ILogger<BatchlogController> logger, IOptionsSnapshot<AppSettings> options) : BaseController(logger, options) {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 給与情報の取得 <br/>
    /// [GET] /api/salaries <br/>
    /// </summary>
    /// <param name="startYm"></param>
    /// <param name="endYm"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [Route("")]
    [HttpGet]
    public IActionResult Get([FromQuery] string? startYm, [FromQuery] string? endYm) {
        try {
            var salaryService = new SalaryService(_logger, _options);
            if (!string.IsNullOrEmpty(startYm) || !string.IsNullOrEmpty(endYm)) {
                var salaries = salaryService.GetByMonthBetween(startYm, endYm);
                return Ok(new ApiResponse<List<Salary>>(ApiResponseStatus.Success, "Salaries retrieved successfully.", salaries));
            } else {
                var salaries = salaryService.GetAll();
                return Ok(new ApiResponse<List<Salary>>(ApiResponseStatus.Success, "Salaries retrieved successfully.", salaries));
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while retrieving salaries.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 給与情報の登録 <br/>
    /// [POST] /api/salaries/upload <br/>
    /// </summary>
    /// <param name="salaries"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [Route("upload")]
    [HttpPost]
    public IActionResult Upload([FromBody] List<Salary> salaries) {
        try {
            var salaryService = new SalaryService(_logger, _options);
            int count = salaryService.InsertOrUpdateAll(salaries);
            return Ok(new ApiResponse<int>(ApiResponseStatus.Success, $"{count} salaries registered.", count));
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while registering salaries.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

}