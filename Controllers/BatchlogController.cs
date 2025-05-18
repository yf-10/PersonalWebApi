using Microsoft.AspNetCore.Mvc;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.Service;

namespace PersonalWebApi.Controllers;

[ApiController]
[Route("api/batchlogs")]
public class BatchlogController(ILogger<BatchlogController> logger, IConfiguration configuration) : BaseAuthenticatedController(logger, configuration) {

    [Route("add")]
    [HttpPost]
    public IActionResult AddBatchlog([FromBody] BatchlogMain batchlog) {
        try {
            var service = new BatchlogService(configuration);
            int count = service.RegisterBatchlogWithTransaction(batchlog);
            return Ok(new ApiResponseJson<int>(ApiResponseStatus.Success, $"{count} batchlog registered.", count));
        } catch (Exception ex) {
            logger.LogError(ex, "An error occurred while registering batchlog.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    [Route("add/detail")]
    [HttpPost]
    public IActionResult AddBatchlogDetail([FromBody] BatchlogDetail detail) {
        try {
            var service = new BatchlogService(configuration);
            int count = service.RegisterBatchlogDetailWithTransaction(detail);
            return Ok(new ApiResponseJson<int>(ApiResponseStatus.Success, $"{count} batchlog detail registered.", count));
        } catch (Exception ex) {
            logger.LogError(ex, "An error occurred while registering batchlog detail.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    [Route("update")]
    [HttpPut]
    public IActionResult UpdateBatchlog([FromBody] BatchlogMain batchlog) {
        try {
            var service = new BatchlogService(configuration);
            int count = service.UpdateBatchlogWithTransaction(batchlog);
            return Ok(new ApiResponseJson<int>(ApiResponseStatus.Success, $"{count} batchlog updated.", count));
        } catch (Exception ex) {
            logger.LogError(ex, "An error occurred while updating batchlog.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    [Route("")]
    [HttpGet]
    public IActionResult GetBatchlogs() {
        try {
            var service = new BatchlogService(configuration);
            var batchlogs = service.GetAllBatchlogs();
            return Ok(new ApiResponseJson<List<BatchlogMain>>(ApiResponseStatus.Success, "Batchlogs retrieved successfully.", batchlogs));
        } catch (Exception ex) {
            logger.LogError(ex, "An error occurred while retrieving batchlogs.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }
}