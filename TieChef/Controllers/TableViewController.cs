using Microsoft.AspNetCore.Mvc;
using TieChef.Models.DTOs;
using TieChef.Models.Enums;

namespace TieChef.Controllers
{
    /// <summary>
    /// API контроллер для управления представлением столов (TableView)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TableViewController : ControllerBase
    {
        // Локальне сховище даних (в пам'яті)
        private static readonly List<TableViewDTO> _tableViewData = new List<TableViewDTO>();

        /// <summary>
        /// Получить все представления столов
        /// </summary>
        /// <returns>Список всех представлений столов</returns>
        /// <response code="200">Успешно получен список представлений столов</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TableViewDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TableViewDTO>> GetAllTableViews()
        {
            return Ok(_tableViewData);
        }

        /// <summary>
        /// Получить представление стола по ID
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <returns>Данные представления стола</returns>
        /// <response code="200">Представление стола найдено</response>
        /// <response code="404">Представление стола не найдено</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TableViewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TableViewDTO> GetTableView(int id)
        {
            var tableView = _tableViewData.FirstOrDefault(t => t.tableId == id);
            if (tableView == null)
            {
                return NotFound(new { message = $"Представление стола с ID {id} не найдено" });
            }
            return Ok(tableView);
        }

        /// <summary>
        /// Создать новое представление стола
        /// </summary>
        /// <param name="tableViewDto">Данные нового представления стола</param>
        /// <returns>Созданное представление стола</returns>
        /// <response code="201">Представление стола успешно создано</response>
        /// <response code="400">Некорректные данные</response>
        [HttpPost]
        [ProducesResponseType(typeof(TableViewDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TableViewDTO> CreateTableView([FromBody] TableViewDTO tableViewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем уникальность ID стола
            if (_tableViewData.Any(t => t.tableId == tableViewDto.tableId))
            {
                return BadRequest(new { message = "Представление стола с таким ID уже существует" });
            }

            // Генерируем displayText на основе данных
            tableViewDto.displayText = GenerateDisplayText(tableViewDto);
            
            _tableViewData.Add(tableViewDto);

            return CreatedAtAction(nameof(GetTableView), new { id = tableViewDto.tableId }, tableViewDto);
        }

        /// <summary>
        /// Обновить данные представления стола
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <param name="tableViewDto">Новые данные представления стола</param>
        /// <returns>Обновленные данные представления стола</returns>
        /// <response code="200">Представление стола успешно обновлено</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Представление стола не найдено</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TableViewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TableViewDTO> UpdateTableView(int id, [FromBody] TableViewDTO tableViewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTableView = _tableViewData.FirstOrDefault(t => t.tableId == id);
            if (existingTableView == null)
            {
                return NotFound(new { message = $"Представление стола с ID {id} не найдено" });
            }

            // Обновляем данные
            existingTableView.staffName = tableViewDto.staffName;
            existingTableView.wasPaid = tableViewDto.wasPaid;
            existingTableView.dishCount = tableViewDto.dishCount;
            existingTableView.sum = tableViewDto.sum;
            existingTableView.paymentDate = tableViewDto.paymentDate;
            existingTableView.status = tableViewDto.status;
            existingTableView.displayText = GenerateDisplayText(existingTableView);

            return Ok(existingTableView);
        }

        /// <summary>
        /// Удалить представление стола
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <returns>Результат удаления</returns>
        /// <response code="200">Представление стола успешно удалено</response>
        /// <response code="404">Представление стола не найдено</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteTableView(int id)
        {
            var tableView = _tableViewData.FirstOrDefault(t => t.tableId == id);
            if (tableView == null)
            {
                return NotFound(new { message = $"Представление стола с ID {id} не найдено" });
            }

            _tableViewData.Remove(tableView);
            return Ok(new { message = $"Представление стола с ID {id} успешно удалено" });
        }

        /// <summary>
        /// Получить представления столов по статусу
        /// </summary>
        /// <param name="status">Статус стола</param>
        /// <returns>Список представлений столов с указанным статусом</returns>
        /// <response code="200">Список представлений столов получен</response>
        [HttpGet("by-status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<TableViewDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TableViewDTO>> GetTableViewsByStatus(e_TableViewStatus status)
        {
            var tableViews = _tableViewData.Where(t => t.status == status).ToList();
            return Ok(tableViews);
        }

        /// <summary>
        /// Получить представления столов по сотруднику
        /// </summary>
        /// <param name="staffName">Имя сотрудника</param>
        /// <returns>Список представлений столов, обслуживаемых указанным сотрудником</returns>
        /// <response code="200">Список представлений столов получен</response>
        [HttpGet("by-staff/{staffName}")]
        [ProducesResponseType(typeof(IEnumerable<TableViewDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TableViewDTO>> GetTableViewsByStaff(string staffName)
        {
            var tableViews = _tableViewData.Where(t => 
                t.staffName != null && t.staffName.Equals(staffName, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(tableViews);
        }

        /// <summary>
        /// Обновить статус представления стола
        /// </summary>
        /// <param name="id">ID стола</param>
        /// <param name="status">Новый статус</param>
        /// <returns>Обновленные данные представления стола</returns>
        /// <response code="200">Статус обновлен</response>
        /// <response code="404">Представление стола не найдено</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(TableViewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TableViewDTO> UpdateStatus(int id, [FromBody] e_TableViewStatus status)
        {
            var tableView = _tableViewData.FirstOrDefault(t => t.tableId == id);
            if (tableView == null)
            {
                return NotFound(new { message = $"Представление стола с ID {id} не найдено" });
            }

            tableView.status = status;
            tableView.displayText = GenerateDisplayText(tableView);

            return Ok(tableView);
        }

        /// <summary>
        /// Получить доступные столы (статус "Available")
        /// </summary>
        /// <returns>Список доступных столов</returns>
        /// <response code="200">Список доступных столов получен</response>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<TableViewDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TableViewDTO>> GetAvailableTables()
        {
            var availableTables = _tableViewData.Where(t => t.status == e_TableViewStatus.Available).ToList();
            return Ok(availableTables);
        }

        /// <summary>
        /// Получить занятые столы (статус "Occupied")
        /// </summary>
        /// <returns>Список занятых столов</returns>
        /// <response code="200">Список занятых столов получен</response>
        [HttpGet("occupied")]
        [ProducesResponseType(typeof(IEnumerable<TableViewDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TableViewDTO>> GetOccupiedTables()
        {
            var occupiedTables = _tableViewData.Where(t => t.status == e_TableViewStatus.Occupied).ToList();
            return Ok(occupiedTables);
        }

        /// <summary>
        /// Получить оплаченные столы (статус "Paid")
        /// </summary>
        /// <returns>Список оплаченных столов</returns>
        /// <response code="200">Список оплаченных столов получен</response>
        [HttpGet("paid")]
        [ProducesResponseType(typeof(IEnumerable<TableViewDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TableViewDTO>> GetPaidTables()
        {
            var paidTables = _tableViewData.Where(t => t.status == e_TableViewStatus.Paid).ToList();
            return Ok(paidTables);
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
            if (_tableViewData.Any())
            {
                return Ok(new { message = "Тестовые данные уже существуют" });
            }

            var testTableViews = new List<TableViewDTO>
            {
                new TableViewDTO
                {
                    tableId = 1,
                    staffName = "Иван Петров",
                    wasPaid = false,
                    dishCount = 3,
                    sum = 150.50m,
                    paymentDate = DateTime.Now,
                    status = e_TableViewStatus.Occupied
                },
                new TableViewDTO
                {
                    tableId = 2,
                    staffName = "Мария Сидорова",
                    wasPaid = true,
                    dishCount = 2,
                    sum = 89.99m,
                    paymentDate = DateTime.Now.AddHours(-2),
                    status = e_TableViewStatus.Paid
                },
                new TableViewDTO
                {
                    tableId = 3,
                    staffName = null,
                    wasPaid = false,
                    dishCount = 0,
                    sum = null,
                    paymentDate = null,
                    status = e_TableViewStatus.Available
                }
            };

            // Генерируем displayText для каждого элемента
            foreach (var tableView in testTableViews)
            {
                tableView.displayText = GenerateDisplayText(tableView);
            }

            _tableViewData.AddRange(testTableViews);
            return Ok(new { message = $"Создано {testTableViews.Count} тестовых представлений столов" });
        }

        /// <summary>
        /// Генерирует текст для отображения на основе данных представления стола
        /// </summary>
        /// <param name="tableView">Данные представления стола</param>
        /// <returns>Строка для отображения</returns>
        private string GenerateDisplayText(TableViewDTO tableView)
        {
            var staffInfo = string.IsNullOrEmpty(tableView.staffName) ? "Свободен" : $"Сотрудник: {tableView.staffName}";
            var paymentInfo = tableView.wasPaid ? "Оплачен" : "Не оплачен";
            var dishInfo = tableView.dishCount > 0 ? $"{tableView.dishCount} блюд" : "Нет заказов";
            var sumInfo = tableView.sum.HasValue ? $"Сумма: {tableView.sum:C}" : "Сумма не указана";
            
            return $"Стол {tableView.tableId} - {staffInfo}, {dishInfo}, {paymentInfo}, {sumInfo}";
        }
    }
}
