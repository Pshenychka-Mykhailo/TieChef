using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
// using TieChef.Data;

namespace TieChef.Controllers
{
    /// <summary>
    /// API контроллер для тестирования функциональности TieChef
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ApiController : ControllerBase
    {
        // private readonly IDistributedCache _cache;
        // private readonly ApplicationDbContext _context;

        // public ApiController(IDistributedCache cache, ApplicationDbContext context)
        // {
        //     _cache = cache;
        //     _context = context;
        // }

        public ApiController()
        {
            // Конструктор без залежностей для тестування CRUD без БД та кешу
        }

        [HttpGet("cache-test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CacheTest()
        {
            return Ok(new { 
                message = "Redis кеш відключений для тестування CRUD операцій", 
                data = new { 
                    message = "Hello from TieChef API!", 
                    random = new Random().Next(1, 1000),
                    generated_at = DateTime.UtcNow
                },
                timestamp = DateTime.UtcNow
            });

        }

        [HttpDelete("cache-test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearCache()
        {
            return Ok(new { message = "Redis кеш відключений для тестування CRUD операцій" });
        }

        [HttpGet("database-test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DatabaseTest()
        {
            // PostgreSQL БД - закоментовано для тестування без БД
            return Ok(new
            {
                message = "PostgreSQL БД відключена для тестування CRUD операцій",
                canConnect = false,
                provider = "None (тестування без БД)",
                timestamp = DateTime.UtcNow
            });

            // try
            // {
            //     var canConnect = await _context.Database.CanConnectAsync();
            //     var providerName = _context.Database.ProviderName;
            //     ... решта коду БД закоментована
            // }
        }


        [HttpGet("system-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSystemInfo()
        {
            // БД та кеш відключені для тестування CRUD операцій
            return Ok(new
            {
                application = "TieChef",
                version = "1.0.0",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                database = new
                {
                    status = "Disabled (тестування без БД)",
                    provider = "None"
                },
                cache = new
                {
                    status = "Disabled (тестування без кешу)",
                    provider = "None"
                },
                swagger = new
                {
                    enabled = true,
                    url = "/swagger"
                },
                crud = new
                {
                    staff = "Доступний (локальне зберігання)",
                    table = "Доступний (локальне зберігання)",
                    tableview = "Доступний (локальне зберігання)"
                }
            });
        }
    }
}
