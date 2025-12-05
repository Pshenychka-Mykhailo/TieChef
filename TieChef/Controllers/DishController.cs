using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using TieChef.Models.Entities;
using TieChef.Repositories;

namespace TieChef.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishController : ControllerBase
    {
        private readonly IDishRepository _repository;
        private readonly IDistributedCache _cache;
        private readonly ILogger<DishController> _logger;

        public DishController(IDishRepository repository, IDistributedCache cache, ILogger<DishController> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetAll()
        {
            string cacheKey = "dishes_list";
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Fetching dishes from cache");
                return Ok(Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Dish>>(cachedData));
            }

            _logger.LogInformation("Fetching dishes from DB");
            var dishes = await _repository.GetAllAsync();
            
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            
            await _cache.SetStringAsync(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(dishes), cacheOptions);

            return Ok(dishes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dish>> Get(int id)
        {
            var dish = await _repository.GetByIdAsync(id);
            if (dish == null) return NotFound();
            return Ok(dish);
        }

        [HttpPost]
        public async Task<ActionResult<Dish>> Create(Dish dish)
        {
            await _repository.AddAsync(dish);
            await _repository.SaveChangesAsync();
            await _cache.RemoveAsync("dishes_list");
            return CreatedAtAction(nameof(Get), new { id = dish.DishId }, dish);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Dish dish)
        {
            if (id != dish.DishId) return BadRequest();
            await _repository.UpdateAsync(dish);
            await _repository.SaveChangesAsync();
            await _cache.RemoveAsync("dishes_list");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dish = await _repository.GetByIdAsync(id);
            if (dish == null) return NotFound();
            await _repository.DeleteAsync(dish);
            await _repository.SaveChangesAsync();
            await _cache.RemoveAsync("dishes_list");
            return NoContent();
        }
    }
}
