using System.Data.Common;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using Npgsql;

using PersonalWebApi.Utilities;

namespace PersonalWebApi.Controllers;

[ApiController]
[Route("api/sample")]
public class SampleController(ILogger<SampleController> logger, IConfiguration configuration) : BaseAuthenticatedController(logger, configuration) {
    private readonly ILogger<PostgresqlWorker> pglogger = new Logger<PostgresqlWorker>(new LoggerFactory());

    [Route("test")]
    [HttpGet]
    public IActionResult Get() {
        try {
            var worker = new PostgresqlWorker(configuration, pglogger);
            string sql = "SELECT * FROM public.batchlog_main";

            var results = worker.ExecuteSqlGetList(sql);
            logger.LogDebug(JsonSerializer.Serialize(results));

            return Ok(results); // ここでリストを返す
        } catch (Exception ex) {
            logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

}
