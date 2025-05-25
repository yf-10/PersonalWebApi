using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Service;

namespace PersonalWebApi.Controllers;
/// --------------------------------------------------------------------------------
/// <summary>
/// 株探コントローラー
/// </summary>
/// --------------------------------------------------------------------------------
[ApiController]
[Route("api/kabutan")]
public class KabutanController(ILogger<BatchlogController> logger, IOptionsSnapshot<AppSettings> options, IHttpClientFactory httpClientFactory) : BaseController(logger, options) {
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 証券コードを指定して株価・指標情報を取得する
    /// </summary>
    /// <param name="code">証券コード（4桁または5桁）</param>
    /// <returns>株価・指標情報のJSON</returns>
    /// --------------------------------------------------------------------------------
    [HttpGet("stock/{code}")]
    public async Task<IActionResult> GetStockData(string code) {
        if (string.IsNullOrWhiteSpace(code) || !Regex.IsMatch(code, @"^\d{4,5}$"))
            return BadRequest("証券コードは4桁または5桁の数字で指定してください。");
        try {
            var kabutanService = new KabutanScrapingService(_logger, _options, _httpClientFactory);
            var result = await kabutanService.GetStockInfoAsync(code);
            if (result == null || string.IsNullOrEmpty(result.Price))
                return NotFound("株価データが取得できませんでした。");
            return Ok(result);
        } catch (Exception ex) {
            return StatusCode(500, "株探からデータ取得中にエラーが発生しました: " + ex.Message);
        }
    }

}