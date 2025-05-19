using Microsoft.AspNetCore.Mvc;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.Service;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Controllers;

[ApiController]
[Route("api/salaries")]
public class SalaryController(ILogger<SalaryController> logger, IConfiguration configuration) : BaseAuthenticatedController(logger, configuration) {

    [Route("add")]
    [HttpPost]
    public IActionResult AddSalaries([FromBody] List<Salary> salaries) {
        try {
            var service = new SalaryService(configuration);
            int count = service.RegisterSalariesWithTransaction(salaries);
            return Ok(new ApiResponseJson<int>(ApiResponseStatus.Success, $"{count} salaries registered.", count));
        } catch (Exception ex) {
            logger.LogError(ex, "An error occurred while registering salaries.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    [Route("")]
    [HttpGet]
    public IActionResult GetSalaries([FromQuery] string? from, [FromQuery] string? to) {
        try {
            var service = new SalaryService(configuration);

            // 期間指定がある場合はフィルタリング
            if (!string.IsNullOrEmpty(from) || !string.IsNullOrEmpty(to)) {
                var salaries = service.GetSalariesByPeriod(from, to);
                return Ok(new ApiResponseJson<List<Salary>>(ApiResponseStatus.Success, "Salaries retrieved successfully.", salaries));
            } else {
                var salaries = service.GetAllSalaries();
                return Ok(new ApiResponseJson<List<Salary>>(ApiResponseStatus.Success, "Salaries retrieved successfully.", salaries));
            }
        } catch (Exception ex) {
            logger.LogError(ex, "An error occurred while retrieving salaries.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }
}