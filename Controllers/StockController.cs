using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.Service;

namespace PersonalWebApi.Controllers;
/// --------------------------------------------------------------------------------
/// <summary>
/// 保有株式コントローラー
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// --------------------------------------------------------------------------------
[ApiController]
[Route("api/stocks")]
public class StockController(ILogger<BatchlogController> logger, IOptionsSnapshot<AppSettings> options) : BaseController(logger, options) {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 保有株式情報の取得 <br/>
    /// [GET] /api/stocks <br/>
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [Route("")]
    [HttpGet]
    public IActionResult Get() {
        try {
            var stockService = new StockService(_logger, _options);
            var stocks = stockService.GetAll();
            return Ok(new ApiResponse<List<Stock>>(ApiResponseStatus.Success, "Stocks retrieved successfully.", stocks));
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while retrieving stocks.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 保有株式情報の登録・更新 <br/>
    /// [POST] /api/stocks/upload <br/>
    /// </summary>
    /// <param name="stocks"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [Route("upload")]
    [HttpPost]
    public IActionResult Upload([FromBody] List<Stock> stocks) {
        try {
            var stockService = new StockService(_logger, _options);
            int count = stockService.InsertOrUpdateAll(stocks);
            return Ok(new ApiResponse<int>(ApiResponseStatus.Success, $"{count} stocks registered or updated.", count));
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while registering stocks.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 保有株式情報の削除 <br/>
    /// [DELETE] /api/stocks/{code} <br/>
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [Route("{code}")]
    [HttpDelete]
    public IActionResult Delete(string code) {
        try {
            var stockService = new StockService(_logger, _options);
            int count = stockService.Delete(code);
            if (count > 0)
                return Ok(new ApiResponse<int>(ApiResponseStatus.Success, "Stock deleted.", count));
            else
                return NotFound(new ApiResponse<string>(ApiResponseStatus.Error, "Stock not found.", null));
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while deleting stock.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

}