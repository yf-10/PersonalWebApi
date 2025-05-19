using System.Data.Common;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Npgsql;

using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.DataAccess;
using PersonalWebApi.Models.Service;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Controllers;

[ApiController]
[Route("api/sample")]
public class SampleController : BaseAuthenticatedController
{
    private readonly ILogger<SampleController> _logger;
    private readonly IOptions<AppSettings> _options;

    public SampleController(ILogger<SampleController> logger, IOptions<AppSettings> options)
        : base(logger, options)
    {
        _logger = logger;
        _options = options;
    }

    [Route("test")]
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            // ここではテスト用の簡易レスポンスなどを返す
            return Ok("Sample API is working.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}
