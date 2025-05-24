using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.Service;

namespace PersonalWebApi.Controllers;
/// --------------------------------------------------------------------------------
/// <summary>
/// バッチログコントローラー
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// --------------------------------------------------------------------------------
[ApiController]
[Route("api/batchlogs")]
public class BatchlogController(ILogger<BatchlogController> logger, IOptionsSnapshot<AppSettings> options) : BaseAuthenticatedController(logger, options) {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ取得：全件 <br/>
    /// [GET] /api/batchlogs <br/>
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [HttpGet]
    [Authorize]
    [Route("")]
    public IActionResult GetAll() {
        try {
            var batchlogService = new BatchlogService(_logger, _options);
            var batchlogs = batchlogService.GetAll();
            return Ok(new ApiResponse<List<Batchlog>>(ApiResponseStatus.Success, "Batchlogs retrieved successfully.", batchlogs));
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while retrieving data.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ取得：バッチ実行識別子指定 <br/>
    /// [GET] /api/batchlogs/{uuid} <br/>
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [HttpGet]
    [Authorize]
    [Route("{uuid}")]
    public IActionResult Get(string uuid) {
        try {
            if (string.IsNullOrEmpty(uuid))
                return BadRequest(new ApiResponse<string>(ApiResponseStatus.Error, $"{nameof(uuid)} is required.", null));
            var batchlogService = new BatchlogService(_logger, _options);
            var batchlog = batchlogService.Get(uuid)
                ?? throw new KeyNotFoundException($"Batchlog with UUID '{uuid}' not found.");
            return Ok(new ApiResponse<Batchlog>(ApiResponseStatus.Success, "Batchlogs retrieved successfully.", batchlog));
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while retrieving data.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ取得：条件指定検索 <br/>
    /// [GET] /api/batchlogs/search <br/>
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [HttpGet]
    [Authorize]
    [Route("search")]
    public IActionResult Search([FromQuery] string? keyword, [FromQuery] string? status) {
        try {
            var batchlogService = new BatchlogService(_logger, _options);
            var batchlogs = batchlogService.Search(keyword, status);
            return Ok(new ApiResponse<List<Batchlog>>(ApiResponseStatus.Success, "Batchlogs retrieved successfully.", batchlogs));
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while retrieving data.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチ開始ログ登録 <br/>
    /// [POST] /api/batchlogs/begin <br/>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [HttpPost]
    [Authorize]
    [Route("begin")]
    public IActionResult Begin([FromBody] BatchlogBeginRequest? request) {
        try {
            if (request == null)
                return BadRequest(new ApiResponse<string>(ApiResponseStatus.Error, "Request body is required.", null));
            if (string.IsNullOrEmpty(request.ProgramId))
                return BadRequest(new ApiResponse<string>(ApiResponseStatus.Error, $"{nameof(request.ProgramId)} is required.", null));
            var batchlogService = new BatchlogService(_logger, _options);
            var uuid = batchlogService.Begin(request.ProgramId, request.ProgramName, request.UserName);
            var batchlog = batchlogService.Get(uuid);
            return Ok(new ApiResponse<Batchlog>(ApiResponseStatus.Success, "Batchlog started.", batchlog));
        } catch (Exception ex) {
            _logger.LogError(ex, "Exception occured.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ完了 <br/>
    /// [POST] /api/batchlogs/{uuid}/complete <br/>
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [HttpPost]
    [Authorize]
    [Route("{uuid}/complete")]
    public IActionResult Complete(string uuid, [FromQuery] string? userName) {
        try {
            if (string.IsNullOrEmpty(uuid))
                return BadRequest(new ApiResponse<string>(ApiResponseStatus.Error, $"{nameof(uuid)} is required.", null));
            var batchlogService = new BatchlogService(_logger, _options);
            batchlogService.Complete(uuid, userName);
            var batchlog = batchlogService.Get(uuid);
            return Ok(new ApiResponse<Batchlog>(ApiResponseStatus.Success, "Batchlog completed.", batchlog));
        } catch (Exception ex) {
            _logger.LogError(ex, "Exception occured.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ中止 <br/>
    /// [POST] /api/batchlogs/{uuid}/abort <br/>
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [HttpPost]
    [Authorize]
    [Route("{uuid}/abort")]
    public IActionResult Abort(string uuid, [FromQuery] string? userName) {
        try {
            if (string.IsNullOrEmpty(uuid))
                return BadRequest(new ApiResponse<string>(ApiResponseStatus.Error, $"{nameof(uuid)} is required.", null));
            var batchlogService = new BatchlogService(_logger, _options);
            batchlogService.Abort(uuid, userName);
            var batchlog = batchlogService.Get(uuid);
            return Ok(new ApiResponse<Batchlog>(ApiResponseStatus.Success, "Batchlog completed.", batchlog));
        } catch (Exception ex) {
            _logger.LogError(ex, "Exception occured.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ詳細追加 <br/>
    /// [POST] /api/batchlogs/log <br/>
    /// </summary>
    /// <param name="detail"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    [HttpPost]
    [Authorize]
    [Route("log")]
    public IActionResult AddLog([FromBody] BatchlogAddRequest? detail) {
        try {
            if (detail == null)
                return BadRequest(new ApiResponse<string>(ApiResponseStatus.Error, "Request body is required.", null));
            if (string.IsNullOrEmpty(detail.Uuid))
                return BadRequest(new ApiResponse<string>(ApiResponseStatus.Error, $"{nameof(detail.Uuid)} is required.", null));
            var batchlogService = new BatchlogService(_logger, _options);
            batchlogService.AddDetailLog(detail.Uuid, detail.LogMsg, detail.UserName);
            var batchlog = batchlogService.Get(detail.Uuid);
            return Ok(new ApiResponse<Batchlog>(ApiResponseStatus.Success, "Batchlog detail added.", batchlog));
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while adding batchlog detail.");
            return StatusCode(500, new ApiResponse<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

}