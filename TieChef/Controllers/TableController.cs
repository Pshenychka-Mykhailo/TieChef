using Microsoft.AspNetCore.Mvc;
using TieChef.Models.DTOs;

namespace TieChef.Controllers
{
    /// <summary>
    /// API контроллер для управления столами (Table)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TableController : ControllerBase
    {
        // Локальне сховище даних (в пам'яті)
        private static readonly List<TableDTO> _tableData = new List<TableDTO>();
        private static int _nextId = 1;

        /// <summary>
        /// Получить все столы
        /// </summary>
        /// <returns>Список всех столов</returns>
        /// <response code="200">Успешно получен список столов</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TableDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TableDTO>> GetAllTables()
        {
            return Ok(_tableData);
        }

        /// <summary>
        /// Получить стол по ID
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <returns>Данные стола</returns>
        /// <response code="200">Стол найден</response>
        /// <response code="404">Стол не найден</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TableDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TableDTO> GetTable(int id)
        {
            var table = _tableData.FirstOrDefault(t => t.tableId == id);
            if (table == null)
            {
                return NotFound(new { message = $"Стол с ID {id} не найден" });
            }
            return Ok(table);
        }

        /// <summary>
        /// Создать новый стол
        /// </summary>
        /// <param name="tableDto">Данные нового стола</param>
        /// <returns>Созданный стол</returns>
        /// <response code="201">Стол успешно создан</response>
        /// <response code="400">Некорректные данные</response>
        [HttpPost]
        [ProducesResponseType(typeof(TableDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TableDTO> CreateTable([FromBody] TableDTO tableDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Генерируем новый ID
            tableDto.tableId = _nextId++;
            _tableData.Add(tableDto);

            return CreatedAtAction(nameof(GetTable), new { id = tableDto.tableId }, tableDto);
        }

        /// <summary>
        /// Обновить данные стола
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <param name="tableDto">Новые данные стола</param>
        /// <returns>Обновленные данные стола</returns>
        /// <response code="200">Стол успешно обновлен</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Стол не найден</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TableDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TableDTO> UpdateTable(int id, [FromBody] TableDTO tableDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTable = _tableData.FirstOrDefault(t => t.tableId == id);
            if (existingTable == null)
            {
                return NotFound(new { message = $"Стол с ID {id} не найден" });
            }

            // Обновляем данные
            existingTable.staffId = tableDto.staffId;
            existingTable.checkId = tableDto.checkId;
            existingTable.wasPaid = tableDto.wasPaid;
            existingTable.dishId = tableDto.dishId;
            existingTable.sum = tableDto.sum;
            existingTable.paymentDate = tableDto.paymentDate;

            return Ok(existingTable);
        }

        /// <summary>
        /// Удалить стол
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <returns>Результат удаления</returns>
        /// <response code="200">Стол успешно удален</response>
        /// <response code="404">Стол не найден</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteTable(int id)
        {
            var table = _tableData.FirstOrDefault(t => t.tableId == id);
            if (table == null)
            {
                return NotFound(new { message = $"Стол с ID {id} не найден" });
            }

            _tableData.Remove(table);
            return Ok(new { message = $"Стол с ID {id} успешно удален" });
        }

        /// <summary>
        /// Получить столы по статусу оплаты
        /// </summary>
        /// <param name="wasPaid">Статус оплаты</param>
        /// <returns>Список столов с указанным статусом оплаты</returns>
        /// <response code="200">Список столов получен</response>
        [HttpGet("by-payment/{wasPaid}")]
        [ProducesResponseType(typeof(IEnumerable<TableDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TableDTO>> GetTablesByPaymentStatus(bool wasPaid)
        {
            var tables = _tableData.Where(t => t.wasPaid == wasPaid).ToList();
            return Ok(tables);
        }

        /// <summary>
        /// Получить столы по сотруднику
        /// </summary>
        /// <param name="staffId">ID сотрудника</param>
        /// <returns>Список столов, обслуживаемых указанным сотрудником</returns>
        /// <response code="200">Список столов получен</response>
        [HttpGet("by-staff/{staffId}")]
        [ProducesResponseType(typeof(IEnumerable<TableDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TableDTO>> GetTablesByStaff(int staffId)
        {
            var tables = _tableData.Where(t => t.staffId == staffId).ToList();
            return Ok(tables);
        }

        /// <summary>
        /// Обновить статус оплаты стола
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <param name="wasPaid">Новый статус оплаты</param>
        /// <returns>Обновленные данные стола</returns>
        /// <response code="200">Статус оплаты обновлен</response>
        /// <response code="404">Стол не найден</response>
        [HttpPatch("{id}/payment")]
        [ProducesResponseType(typeof(TableDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TableDTO> UpdatePaymentStatus(int id, [FromBody] bool wasPaid)
        {
            var table = _tableData.FirstOrDefault(t => t.tableId == id);
            if (table == null)
            {
                return NotFound(new { message = $"Стол с ID {id} не найден" });
            }

            table.wasPaid = wasPaid;
            if (wasPaid)
            {
                table.paymentDate = DateTime.Now;
            }

            return Ok(table);
        }

        /// <summary>
        /// Добавить блюдо к столу
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <param name="dishId">ID блюда</param>
        /// <returns>Обновленные данные стола</returns>
        /// <response code="200">Блюдо добавлено</response>
        /// <response code="404">Стол не найден</response>
        [HttpPost("{id}/dishes/{dishId}")]
        [ProducesResponseType(typeof(TableDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TableDTO> AddDishToTable(int id, int dishId)
        {
            var table = _tableData.FirstOrDefault(t => t.tableId == id);
            if (table == null)
            {
                return NotFound(new { message = $"Стол с ID {id} не найден" });
            }

            if (!table.dishId.Contains(dishId))
            {
                table.dishId.Add(dishId);
            }

            return Ok(table);
        }

        /// <summary>
        /// Удалить блюдо из стола
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <param name="dishId">ID блюда</param>
        /// <returns>Обновленные данные стола</returns>
        /// <response code="200">Блюдо удалено</response>
        /// <response code="404">Стол не найден</response>
        [HttpDelete("{id}/dishes/{dishId}")]
        [ProducesResponseType(typeof(TableDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TableDTO> RemoveDishFromTable(int id, int dishId)
        {
            var table = _tableData.FirstOrDefault(t => t.tableId == id);
            if (table == null)
            {
                return NotFound(new { message = $"Стол с ID {id} не найден" });
            }

            table.dishId.Remove(dishId);
            return Ok(table);
        }

        /// <summary>
        /// Инициализировать тестовые данные
        /// </summary>
        /// <returns>Результат инициализации</returns>
        /// <response code="200">Тестовые данные созданы</response>
        [HttpPost("init-test-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult InitializeTestData()
        {
            if (_tableData.Any())
            {
                return Ok(new { message = "Тестовые данные уже существуют" });
            }

            var testTables = new List<TableDTO>
            {
                new TableDTO
                {
                    tableId = _nextId++,
                    staffId = 1,
                    checkId = 101,
                    wasPaid = false,
                    dishId = new List<int?> { 1, 2, 3 },
                    sum = 150.50m,
                    paymentDate = DateTime.Now
                },
                new TableDTO
                {
                    tableId = _nextId++,
                    staffId = 2,
                    checkId = 102,
                    wasPaid = true,
                    dishId = new List<int?> { 4, 5 },
                    sum = 89.99m,
                    paymentDate = DateTime.Now.AddHours(-2)
                },
                new TableDTO
                {
                    tableId = _nextId++,
                    staffId = null,
                    checkId = null,
                    wasPaid = false,
                    dishId = new List<int?>(),
                    sum = null,
                    paymentDate = null
                }
            };

            _tableData.AddRange(testTables);
            return Ok(new { message = $"Создано {testTables.Count} тестовых столов" });
        }
    }
}
