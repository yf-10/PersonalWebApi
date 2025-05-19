using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.Service;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Controllers;

[ApiController]
[Route("api/batchlogs")]
public class BatchlogController(ILogger<BatchlogController> logger, IConfiguration configuration) : BaseAuthenticatedController(logger, configuration)
{
    // [GET] /api/batchlogs/{uuid?}
    [HttpGet]
    [Authorize]
    [Route("{uuid?}")]
    public IActionResult GetBatchlogs(string? uuid, [FromQuery] string? keyword, [FromQuery] string? status)
    {
        try
        {
            var service = new BatchlogService(configuration);
            var batchlogs = service.GetBatchlogs(uuid, keyword, status);
            return Ok(new ApiResponseJson<List<BatchlogMain>>(ApiResponseStatus.Success, "Batchlogs retrieved successfully.", batchlogs));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving batchlogs.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    // [POST] /api/batchlogs/begin
    [HttpPost]
    [Authorize]
    [Route("begin")]
    public IActionResult BeginBatchlog([FromBody] BatchlogBeginRequest request)
    {
        try
        {
            var service = new BatchlogService(configuration);
            var uuid = service.BeginBatchlog(request.ProgramId, request.ProgramName, request.UserName);
            var batchlogs = service.GetBatchlogs(uuid, null, null);
            return Ok(new ApiResponseJson<List<BatchlogMain>>(ApiResponseStatus.Success, "Batchlog started.", batchlogs));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while beginning batchlog.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    // [POST] /api/batchlogs/{uuid}/complete
    [HttpPost]
    [Authorize]
    [Route("{uuid}/complete")]
    public IActionResult CompleteBatchlog(string uuid, [FromQuery] string? userName)
    {
        try
        {
            var service = new BatchlogService(configuration);
            service.CompleteBatchlog(uuid, userName);
            var batchlogs = service.GetBatchlogs(uuid, null, null);
            return Ok(new ApiResponseJson<List<BatchlogMain>>(ApiResponseStatus.Success, "Batchlog completed.", batchlogs));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while completing batchlog.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    // [POST] /api/batchlogs/{uuid}/abort
    [HttpPost]
    [Authorize]
    [Route("{uuid}/abort")]
    public IActionResult AbortBatchlog(string uuid, [FromQuery] string? userName)
    {
        try
        {
            var service = new BatchlogService(configuration);
            service.AbortBatchlog(uuid, userName);
            var batchlogs = service.GetBatchlogs(uuid, null, null);
            return Ok(new ApiResponseJson<List<BatchlogMain>>(ApiResponseStatus.Success, "Batchlog aborted.", batchlogs));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while aborting batchlog.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    // [POST] /api/batchlogs/log
    [HttpPost]
    [Authorize]
    [Route("log")]
    public IActionResult AddBatchlogDetail([FromBody] BatchlogDetail detail, [FromQuery] string? userName)
    {
        try
        {
            var service = new BatchlogService(configuration);
            service.AddBatchlogLog(detail, userName);
            var batchlogs = service.GetBatchlogs(detail.Uuid, null, null);
            return Ok(new ApiResponseJson<List<BatchlogMain>>(ApiResponseStatus.Success, "Batchlog detail added.", batchlogs));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while adding batchlog detail.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }
}

// 必要に応じてリクエスト用DTOを追加
public class BatchlogBeginRequest
{
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string? UserName { get; set; }
}