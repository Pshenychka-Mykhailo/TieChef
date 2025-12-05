using Microsoft.AspNetCore.Mvc;
using TieChef.Models.Entities;
using TieChef.Repositories;

namespace TieChef.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiningTableController : ControllerBase
{
    private readonly IDiningTableRepository _repository;

    public DiningTableController(IDiningTableRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiningTable>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<DiningTable>> Create(DiningTable table)
    {
        await _repository.AddAsync(table);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = table.DiningTableId }, table);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DiningTable table)
    {
        if (id != table.DiningTableId)
        {
            return BadRequest();
        }

        await _repository.UpdateAsync(table);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var table = await _repository.GetByIdAsync(id);
        if (table == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(table);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
    [HttpPost("reset")]
    public async Task<IActionResult> ResetLayout()
    {
        var tables = await _repository.GetAllAsync();
        foreach (var table in tables)
        {
            table.X = null;
            table.Y = null;
            await _repository.UpdateAsync(table);
        }
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
