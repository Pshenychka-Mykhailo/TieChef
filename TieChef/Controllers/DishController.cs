using Microsoft.AspNetCore.Mvc;
using TieChef.Models.Entities;
using TieChef.Repositories;

namespace TieChef.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishController : ControllerBase
    {
        private readonly IDishRepository _repository;

        public DishController(IDishRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
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
            return CreatedAtAction(nameof(Get), new { id = dish.DishId }, dish);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Dish dish)
        {
            if (id != dish.DishId) return BadRequest();
            await _repository.UpdateAsync(dish);
            await _repository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dish = await _repository.GetByIdAsync(id);
            if (dish == null) return NotFound();
            await _repository.DeleteAsync(dish);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
    }
}
