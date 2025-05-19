using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.Service;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Controllers;

[ApiController]
[Route("api/salaries")]
public class SalaryController : BaseAuthenticatedController
{
    private readonly ILogger<SalaryController> _logger;
    private readonly IOptions<AppSettings> _options;

    public SalaryController(ILogger<SalaryController> logger, IOptions<AppSettings> options)
        : base(logger, options)
    {
        _logger = logger;
        _options = options;
    }

    [Route("add")]
    [HttpPost]
    public IActionResult AddSalaries([FromBody] List<Salary> salaries)
    {
        try
        {
            var service = new SalaryService(_options);
            int count = service.RegisterSalariesWithTransaction(salaries);
            return Ok(new ApiResponseJson<int>(ApiResponseStatus.Success, $"{count} salaries registered.", count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while registering salaries.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }

    [Route("")]
    [HttpGet]
    public IActionResult GetSalaries([FromQuery] string? from, [FromQuery] string? to)
    {
        try
        {
            var service = new SalaryService(_options);

            // 期間指定がある場合はフィルタリング
            if (!string.IsNullOrEmpty(from) || !string.IsNullOrEmpty(to))
            {
                var salaries = service.GetSalariesByPeriod(from, to);
                return Ok(new ApiResponseJson<List<Salary>>(ApiResponseStatus.Success, "Salaries retrieved successfully.", salaries));
            }
            else
            {
                var salaries = service.GetAllSalaries();
                return Ok(new ApiResponseJson<List<Salary>>(ApiResponseStatus.Success, "Salaries retrieved successfully.", salaries));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving salaries.");
            return StatusCode(500, new ApiResponseJson<string>(ApiResponseStatus.Error, "Internal server error: " + ex.Message, null));
        }
    }
}