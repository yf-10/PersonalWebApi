using System.Data.Common;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using Npgsql;

using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.DataAccess;
using PersonalWebApi.Models.Service;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Controllers;

[ApiController]
[Route("api/sample")]
public class SampleController(ILogger<SampleController> logger, IConfiguration configuration) : BaseAuthenticatedController(logger, configuration) {

    [Route("test")]
    [HttpGet]
    public IActionResult Get() {
        try {
            // ここではテスト用の簡易レスポンスなどを返す
            return Ok("Sample API is working.");
        } catch (Exception ex) {
            logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}
