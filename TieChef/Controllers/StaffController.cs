using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TieChef.Models.DTOs;
using TieChef.Models.Enums;
using TieChef.Repositories;
using TieChef.Models.DTOs;
using TieChef.Models.Enums;

namespace TieChef.Controllers
{
    /// <summary>
    /// API контролер для управління персоналом (Staff)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffRepository _repository;

        public StaffController(IStaffRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Отримати всіх співробітників
        /// </summary>
        /// <returns>Список всіх співробітників</returns>
        /// <response code="200">Успішно отриманий список співробітників</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StaffDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StaffDTO>>> GetAllStaff()
        {
            var staff = await _repository.GetAllAsync();
            var dtos = staff.Select(s => new StaffDTO
            {
                staffId = s.StaffId,
                type = s.Type,
                role = s.Role,
                fullName = s.FullName,
                phoneNumber = s.PhoneNumber,
                Email = s.Email,
                startWorkDate = s.StartWorkDate,
                scheduleId = s.ScheduleId,
                salary = s.Salary,
                KPI = s.KPI
            }).ToList();

            return Ok(dtos);
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
        public async Task<ActionResult<StaffDTO>> GetStaff(int id)
        {
            var staff = await _repository.GetByIdAsync(id);
            if (staff == null)
            {
                return NotFound(new { message = $"Співробітник з ID {id} не знайдений" });
            }

            var dto = new StaffDTO
            {
                staffId = staff.StaffId,
                type = staff.Type,
                role = staff.Role,
                fullName = staff.FullName,
                phoneNumber = staff.PhoneNumber,
                Email = staff.Email,
                startWorkDate = staff.StartWorkDate,
                scheduleId = staff.ScheduleId,
                salary = staff.Salary,
                KPI = staff.KPI
            };

            return Ok(dto);
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
        public async Task<ActionResult<StaffDTO>> CreateStaff([FromBody] StaffDTO staffDto)
        {
            // Validation is handled by FluentValidation automatically

            // Перевіряємо унікальність email
            if (await _repository.ExistsAsync(s => s.Email == staffDto.Email))
            {
                return BadRequest(new { message = "Співробітник з таким email вже існує" });
            }

            var staff = new Models.Entities.Staff
            {
                Type = staffDto.type,
                Role = staffDto.role,
                FullName = staffDto.fullName,
                PhoneNumber = staffDto.phoneNumber,
                Email = staffDto.Email,
                StartWorkDate = staffDto.startWorkDate,
                ScheduleId = staffDto.scheduleId,
                Salary = staffDto.salary,
                KPI = staffDto.KPI
            };

            await _repository.AddAsync(staff);
            await _repository.SaveChangesAsync();

            staffDto.staffId = staff.StaffId;

            return CreatedAtAction(nameof(GetStaff), new { id = staffDto.staffId }, staffDto);
        }

        /// <summary>
        /// Оновити дані співробітника
        /// </summary>
        /// <param name="id">ID співробітника</param>
        /// <param name="staffDto">Нові дані співробітника</param>
        /// <returns>Оновлені дані співробітника</returns>
        /// <response code="200">Співробітник успішно оновлений</response>
        /// <response code="400">Некоректні дані</response>
        /// <response code="404">Співробітник не знайдений</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StaffDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StaffDTO>> UpdateStaff(int id, [FromBody] StaffDTO staffDto)
        {
            var existingStaff = await _repository.GetByIdAsync(id);
            if (existingStaff == null)
            {
                return NotFound(new { message = $"Співробітник з ID {id} не знайдений" });
            }

            // Перевіряємо унікальність email (виключаючи поточного співробітника)
            if (await _repository.ExistsAsync(s => s.Email == staffDto.Email && s.StaffId != id))
            {
                return BadRequest(new { message = "Співробітник з таким email вже існує" });
            }

            // Оновлюємо дані
            existingStaff.Type = staffDto.type;
            existingStaff.Role = staffDto.role;
            existingStaff.FullName = staffDto.fullName;
            existingStaff.PhoneNumber = staffDto.phoneNumber;
            existingStaff.Email = staffDto.Email;
            existingStaff.StartWorkDate = staffDto.startWorkDate;
            existingStaff.ScheduleId = staffDto.scheduleId;
            existingStaff.Salary = staffDto.salary;
            existingStaff.KPI = staffDto.KPI;

            await _repository.UpdateAsync(existingStaff);
            await _repository.SaveChangesAsync();

            return Ok(staffDto);
        }

        /// <summary>
        /// Видалити співробітника
        /// </summary>
        /// <param name="id">ID співробітника</param>
        /// <returns>Результат видалення</returns>
        /// <response code="200">Співробітник успішно видалений</response>
        /// <response code="404">Співробітник не знайдений</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteStaff(int id)
        {
            var staff = await _repository.GetByIdAsync(id);
            if (staff == null)
            {
                return NotFound(new { message = $"Співробітник з ID {id} не знайдений" });
            }

            await _repository.DeleteAsync(staff);
            await _repository.SaveChangesAsync();
            return Ok(new { message = $"Співробітник {staff.FullName} успішно видалений" });
        }

        /// <summary>
        /// Отримати співробітників за типом
        /// </summary>
        /// <param name="type">Тип співробітника</param>
        /// <returns>Список співробітників зазначеного типу</returns>
        /// <response code="200">Список співробітників отримано</response>
        [HttpGet("by-type/{type}")]
        [ProducesResponseType(typeof(IEnumerable<StaffDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StaffDTO>>> GetStaffByType(e_StaffType type)
        {
            var staff = await _repository.FindAsync(s => s.Type == type);
            var dtos = staff.Select(s => new StaffDTO
            {
                staffId = s.StaffId,
                type = s.Type,
                role = s.Role,
                fullName = s.FullName,
                phoneNumber = s.PhoneNumber,
                Email = s.Email,
                startWorkDate = s.StartWorkDate,
                scheduleId = s.ScheduleId,
                salary = s.Salary,
                KPI = s.KPI
            }).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Отримати співробітників за роллю
        /// </summary>
        /// <param name="role">Роль співробітника</param>
        /// <returns>Список співробітників зазначеної ролі</returns>
        /// <response code="200">Список співробітників отримано</response>
        [HttpGet("by-role/{role}")]
        [ProducesResponseType(typeof(IEnumerable<StaffDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StaffDTO>>> GetStaffByRole(e_StaffRole role)
        {
            var staff = await _repository.FindAsync(s => s.Role == role);
            var dtos = staff.Select(s => new StaffDTO
            {
                staffId = s.StaffId,
                type = s.Type,
                role = s.Role,
                fullName = s.FullName,
                phoneNumber = s.PhoneNumber,
                Email = s.Email,
                startWorkDate = s.StartWorkDate,
                scheduleId = s.ScheduleId,
                salary = s.Salary,
                KPI = s.KPI
            }).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Ініціалізувати тестові дані
        /// </summary>
        /// <returns>Результат ініціалізації</returns>
        /// <response code="200">Тестові дані створені</response>
        [HttpPost("init-test-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> InitializeTestData()
        {
            if ((await _repository.GetAllAsync()).Any())
            {
                return Ok(new { message = "Тестові дані вже існують" });
            }

            var testStaff = new List<Models.Entities.Staff>
            {
                new Models.Entities.Staff
                {
                    Type = e_StaffType.Manager,
                    Role = e_StaffRole.Manager,
                    FullName = "Иван Петров",
                    PhoneNumber = 123456789,
                    Email = "ivan.petrov@tiechef.com",
                    StartWorkDate = DateTime.Now.AddYears(-2),
                    Salary = 50000,
                    KPI = "95%"
                },
                new Models.Entities.Staff
                {
                    Type = e_StaffType.Trainer,
                    Role = e_StaffRole.Trainer,
                    FullName = "Мария Сидорова",
                    PhoneNumber = 987654321,
                    Email = "maria.sidorova@tiechef.com",
                    StartWorkDate = DateTime.Now.AddYears(-1),
                    Salary = 35000,
                    KPI = "88%"
                },
                new Models.Entities.Staff
                {
                    Type = e_StaffType.Nutritionist,
                    Role = e_StaffRole.Nutritionist,
                    FullName = "Алексей Козлов",
                    PhoneNumber = 555555555,
                    Email = "alexey.kozlov@tiechef.com",
                    StartWorkDate = DateTime.Now.AddMonths(-6),
                    Salary = 40000,
                    KPI = "92%"
                }
            };

            await _repository.AddRangeAsync(testStaff);
            await _repository.SaveChangesAsync();
            return Ok(new { message = $"Створено {testStaff.Count} тестових співробітників" });
        }
    }
}
