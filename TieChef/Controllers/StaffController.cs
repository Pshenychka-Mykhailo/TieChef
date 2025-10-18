using Microsoft.AspNetCore.Mvc;
using TieChef.Models.DTOs;
using TieChef.Models.Enums;

namespace TieChef.Controllers
{
    /// <summary>
    /// API контроллер для управления персоналом (Staff)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StaffController : ControllerBase
    {
        // Локальне сховище даних (в пам'яті)
        private static readonly List<StaffDTO> _staffData = new List<StaffDTO>();
        private static int _nextId = 1;

        /// <summary>
        /// Отримати всіх співробітників
        /// </summary>
        /// <returns>Список всіх співробітників</returns>
        /// <response code="200">Успішно отриманий список співробітників</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StaffDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<StaffDTO>> GetAllStaff()
        {
            return Ok(_staffData);
        }

        /// <summary>
        /// Отримати співробітника за ID
        /// </summary>
        /// <param name="id">ID співробітника</param>
        /// <returns>Дані співробітника</returns>
        /// <response code="200">Співробітник знайдений</response>
        /// <response code="404">Співробітник не знайдений</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StaffDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<StaffDTO> GetStaff(int id)
        {
            var staff = _staffData.FirstOrDefault(s => s.staffId == id);
            if (staff == null)
            {
                return NotFound(new { message = $"Сотрудник с ID {id} не найден" });
            }
            return Ok(staff);
        }

        /// <summary>
        /// Створити нового співробітника
        /// </summary>
        /// <param name="staffDto">Дані нового співробітника</param>
        /// <returns>Створений співробітник</returns>
        /// <response code="201">Співробітник успішно створений</response>
        /// <response code="400">Некоректні дані</response>
        [HttpPost]
        [ProducesResponseType(typeof(StaffDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StaffDTO> CreateStaff([FromBody] StaffDTO staffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем уникальность email
            if (_staffData.Any(s => s.Email == staffDto.Email))
            {
                return BadRequest(new { message = "Сотрудник с таким email уже существует" });
            }

            // Генерируем новый ID
            staffDto.staffId = _nextId++;
            _staffData.Add(staffDto);

            return CreatedAtAction(nameof(GetStaff), new { id = staffDto.staffId }, staffDto);
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        /// <param name="id">ID сотрудника</param>
        /// <param name="staffDto">Новые данные сотрудника</param>
        /// <returns>Обновленные данные сотрудника</returns>
        /// <response code="200">Сотрудник успешно обновлен</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Сотрудник не найден</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StaffDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<StaffDTO> UpdateStaff(int id, [FromBody] StaffDTO staffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingStaff = _staffData.FirstOrDefault(s => s.staffId == id);
            if (existingStaff == null)
            {
                return NotFound(new { message = $"Сотрудник с ID {id} не найден" });
            }

            // Проверяем уникальность email (исключая текущего сотрудника)
            if (_staffData.Any(s => s.Email == staffDto.Email && s.staffId != id))
            {
                return BadRequest(new { message = "Сотрудник с таким email уже существует" });
            }

            // Обновляем данные
            existingStaff.type = staffDto.type;
            existingStaff.role = staffDto.role;
            existingStaff.fullName = staffDto.fullName;
            existingStaff.phoneNumber = staffDto.phoneNumber;
            existingStaff.Email = staffDto.Email;
            existingStaff.startWorkDate = staffDto.startWorkDate;
            existingStaff.scheduleId = staffDto.scheduleId;
            existingStaff.salary = staffDto.salary;
            existingStaff.KPI = staffDto.KPI;

            return Ok(existingStaff);
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <param name="id">ID сотрудника</param>
        /// <returns>Результат удаления</returns>
        /// <response code="200">Сотрудник успешно удален</response>
        /// <response code="404">Сотрудник не найден</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteStaff(int id)
        {
            var staff = _staffData.FirstOrDefault(s => s.staffId == id);
            if (staff == null)
            {
                return NotFound(new { message = $"Сотрудник с ID {id} не найден" });
            }

            _staffData.Remove(staff);
            return Ok(new { message = $"Сотрудник {staff.fullName} успешно удален" });
        }

        /// <summary>
        /// Получить сотрудников по типу
        /// </summary>
        /// <param name="type">Тип сотрудника</param>
        /// <returns>Список сотрудников указанного типа</returns>
        /// <response code="200">Список сотрудников получен</response>
        [HttpGet("by-type/{type}")]
        [ProducesResponseType(typeof(IEnumerable<StaffDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<StaffDTO>> GetStaffByType(e_StaffType type)
        {
            var staff = _staffData.Where(s => s.type == type).ToList();
            return Ok(staff);
        }

        /// <summary>
        /// Получить сотрудников по роли
        /// </summary>
        /// <param name="role">Роль сотрудника</param>
        /// <returns>Список сотрудников указанной роли</returns>
        /// <response code="200">Список сотрудников получен</response>
        [HttpGet("by-role/{role}")]
        [ProducesResponseType(typeof(IEnumerable<StaffDTO>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<StaffDTO>> GetStaffByRole(e_StaffRole role)
        {
            var staff = _staffData.Where(s => s.role == role).ToList();
            return Ok(staff);
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
            if (_staffData.Any())
            {
                return Ok(new { message = "Тестовые данные уже существуют" });
            }

            var testStaff = new List<StaffDTO>
            {
                new StaffDTO
                {
                    staffId = _nextId++,
                    type = e_StaffType.Manager,
                    role = e_StaffRole.Manager,
                    fullName = "Иван Петров",
                    phoneNumber = 123456789,
                    Email = "ivan.petrov@tiechef.com",
                    startWorkDate = DateTime.Now.AddYears(-2),
                    salary = 50000,
                    KPI = "95%"
                },
                new StaffDTO
                {
                    staffId = _nextId++,
                    type = e_StaffType.Trainer,
                    role = e_StaffRole.Trainer,
                    fullName = "Мария Сидорова",
                    phoneNumber = 987654321,
                    Email = "maria.sidorova@tiechef.com",
                    startWorkDate = DateTime.Now.AddYears(-1),
                    salary = 35000,
                    KPI = "88%"
                },
                new StaffDTO
                {
                    staffId = _nextId++,
                    type = e_StaffType.Nutritionist,
                    role = e_StaffRole.Nutritionist,
                    fullName = "Алексей Козлов",
                    phoneNumber = 555555555,
                    Email = "alexey.kozlov@tiechef.com",
                    startWorkDate = DateTime.Now.AddMonths(-6),
                    salary = 40000,
                    KPI = "92%"
                }
            };

            _staffData.AddRange(testStaff);
            return Ok(new { message = $"Создано {testStaff.Count} тестовых сотрудников" });
        }
    }
}
