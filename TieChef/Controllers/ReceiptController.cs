using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TieChef.Models.DTOs;
using TieChef.Repositories;
using TieChef.Models.DTOs;

namespace TieChef.Controllers
{
    /// <summary>
    /// API контролер для управління чеками (Receipt)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _repository;

        public ReceiptController(IReceiptRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Отримати всі чеки
        /// </summary>
        /// <returns>Список всіх чеків</returns>
        /// <response code="200">Успішно отримано список чеків</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReceiptDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetAllReceipts()
        {
            var receipts = await _repository.GetAllAsync();
            var dtos = receipts.Select(r => new ReceiptDTO
            {
                receiptId = r.ReceiptId,
                tableId = r.TableId,
                staffId = r.StaffId,
                checkId = r.CheckId,
                wasPaid = r.WasPaid,
                dishIds = r.DishIds?.Cast<int?>().ToList() ?? new List<int?>(),
                sum = r.Sum,
                paymentDate = r.PaymentDate
            }).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Отримати чек за ID
        /// </summary>
        /// <param name="id">ID чека</param>
        /// <returns>Дані чека</returns>
        /// <response code="200">Чек знайдений</response>
        /// <response code="404">Чек не знайдений</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReceiptDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReceiptDTO>> GetReceipt(int id)
        {
            var receipt = await _repository.GetByIdAsync(id);
            if (receipt == null)
            {
                return NotFound(new { message = $"Чек з ID {id} не знайдений" });
            }

            var dto = new ReceiptDTO
            {
                receiptId = receipt.ReceiptId,
                tableId = receipt.TableId,
                staffId = receipt.StaffId,
                checkId = receipt.CheckId,
                wasPaid = receipt.WasPaid,
                dishIds = receipt.DishIds?.Cast<int?>().ToList() ?? new List<int?>(),
                sum = receipt.Sum,
                paymentDate = receipt.PaymentDate
            };

            return Ok(dto);
        }

        /// <summary>
        /// Створити новий чек
        /// </summary>
        /// <param name="receiptDto">Дані нового чека</param>
        /// <returns>Створений чек</returns>
        /// <response code="201">Чек успішно створений</response>
        /// <response code="400">Некоректні дані</response>
        [HttpPost]
        [ProducesResponseType(typeof(ReceiptDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReceiptDTO>> CreateReceipt([FromBody] ReceiptDTO receiptDto)
        {
            // Validation is handled by FluentValidation automatically

            var receipt = new Models.Entities.Receipt
            {
                TableId = receiptDto.tableId,
                StaffId = receiptDto.staffId,
                CheckId = receiptDto.checkId,
                WasPaid = receiptDto.wasPaid,
                DishIds = receiptDto.dishIds.Where(d => d.HasValue).Select(d => d!.Value).ToList(),
                Sum = receiptDto.sum,
                PaymentDate = receiptDto.paymentDate
            };

            await _repository.AddAsync(receipt);
            await _repository.SaveChangesAsync();

            receiptDto.receiptId = receipt.ReceiptId;

            return CreatedAtAction(nameof(GetReceipt), new { id = receipt.ReceiptId }, receiptDto);
        }

        /// <summary>
        /// Оновити дані чека
        /// </summary>
        /// <param name="id">ID чека</param>
        /// <param name="receiptDto">Нові дані чека</param>
        /// <returns>Оновлені дані чека</returns>
        /// <response code="200">Чек успішно оновлений</response>
        /// <response code="400">Некоректні дані</response>
        /// <response code="404">Чек не знайдений</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ReceiptDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReceiptDTO>> UpdateReceipt(int id, [FromBody] ReceiptDTO receiptDto)
        {
            var existingReceipt = await _repository.GetByIdAsync(id);
            if (existingReceipt == null)
            {
                return NotFound(new { message = $"Чек з ID {id} не знайдений" });
            }

            // Оновлюємо дані
            existingReceipt.TableId = receiptDto.tableId;
            existingReceipt.StaffId = receiptDto.staffId;
            existingReceipt.CheckId = receiptDto.checkId;
            existingReceipt.WasPaid = receiptDto.wasPaid;
            existingReceipt.DishIds = receiptDto.dishIds.Where(d => d.HasValue).Select(d => d.Value).ToList();
            existingReceipt.Sum = receiptDto.sum;
            existingReceipt.PaymentDate = receiptDto.paymentDate;

            await _repository.UpdateAsync(existingReceipt);
            await _repository.SaveChangesAsync();

            receiptDto.receiptId = existingReceipt.ReceiptId;

            return Ok(receiptDto);
        }

        /// <summary>
        /// Видалити чек
        /// </summary>
        /// <param name="id">ID чека</param>
        /// <returns>Результат видалення</returns>
        /// <response code="200">Чек успішно видалений</response>
        /// <response code="404">Чек не знайдений</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteReceipt(int id)
        {
            var receipt = await _repository.GetByIdAsync(id);
            if (receipt == null)
            {
                return NotFound(new { message = $"Чек з ID {id} не знайдений" });
            }

            await _repository.DeleteAsync(receipt);
            await _repository.SaveChangesAsync();
            return Ok(new { message = $"Чек з ID {id} успішно видалений" });
        }

        /// <summary>
        /// Отримати чеки за статусом оплати
        /// </summary>
        /// <param name="wasPaid">Статус оплати</param>
        /// <returns>Список чеків із зазначеним статусом оплати</returns>
        /// <response code="200">Список чеків отримано</response>
        [HttpGet("by-payment/{wasPaid}")]
        [ProducesResponseType(typeof(IEnumerable<ReceiptDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetReceiptsByPaymentStatus(bool wasPaid)
        {
            var receipts = await _repository.FindAsync(r => r.WasPaid == wasPaid);
            var dtos = receipts.Select(r => new ReceiptDTO
            {
                receiptId = r.ReceiptId,
                tableId = r.TableId,
                staffId = r.StaffId,
                checkId = r.CheckId,
                wasPaid = r.WasPaid,
                dishIds = r.DishIds?.Cast<int?>().ToList() ?? new List<int?>(),
                sum = r.Sum,
                paymentDate = r.PaymentDate
            }).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Отримати чеки за співробітником
        /// </summary>
        /// <param name="staffId">ID співробітника</param>
        /// <returns>Список чеків, що обслуговуються зазначеним співробітником</returns>
        /// <response code="200">Список чеків отримано</response>
        [HttpGet("by-staff/{staffId}")]
        [ProducesResponseType(typeof(IEnumerable<ReceiptDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReceiptDTO>>> GetReceiptsByStaff(int staffId)
        {
            var receipts = await _repository.FindAsync(r => r.StaffId == staffId);
            var dtos = receipts.Select(r => new ReceiptDTO
            {
                receiptId = r.ReceiptId,
                tableId = r.TableId,
                staffId = r.StaffId,
                checkId = r.CheckId,
                wasPaid = r.WasPaid,
                dishIds = r.DishIds?.Cast<int?>().ToList() ?? new List<int?>(),
                sum = r.Sum,
                paymentDate = r.PaymentDate
            }).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Оновити статус оплати чека
        /// </summary>
        /// <param name="id">ID чека</param>
        /// <param name="wasPaid">Новий статус оплати</param>
        /// <returns>Оновлені дані чека</returns>
        /// <response code="200">Статус оплати оновлений</response>
        /// <response code="404">Чек не знайдений</response>
        [HttpPatch("{id}/payment")]
        [ProducesResponseType(typeof(ReceiptDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReceiptDTO>> UpdatePaymentStatus(int id, [FromBody] bool wasPaid)
        {
            var receipt = await _repository.GetByIdAsync(id);
            if (receipt == null)
            {
                return NotFound(new { message = $"Чек з ID {id} не знайдений" });
            }

            receipt.WasPaid = wasPaid;
            if (wasPaid)
            {
                receipt.PaymentDate = DateTime.Now;
            }

            await _repository.UpdateAsync(receipt);
            await _repository.SaveChangesAsync();

            var dto = new ReceiptDTO
            {
                receiptId = receipt.ReceiptId,
                tableId = receipt.TableId,
                staffId = receipt.StaffId,
                checkId = receipt.CheckId,
                wasPaid = receipt.WasPaid,
                dishIds = receipt.DishIds?.Cast<int?>().ToList() ?? new List<int?>(),
                sum = receipt.Sum,
                paymentDate = receipt.PaymentDate
            };

            return Ok(dto);
        }

        /// <summary>
        /// Додати страву до чека
        /// </summary>
        /// <param name="id">ID чека</param>
        /// <param name="dishId">ID страви</param>
        /// <returns>Оновлені дані чека</returns>
        /// <response code="200">Страву додано</response>
        /// <response code="404">Чек не знайдений</response>
        [HttpPost("{id}/dishes/{dishId}")]
        [ProducesResponseType(typeof(ReceiptDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReceiptDTO>> AddDishToReceipt(int id, int dishId)
        {
            var receipt = await _repository.GetByIdAsync(id);
            if (receipt == null)
            {
                return NotFound(new { message = $"Чек з ID {id} не знайдений" });
            }

            if (receipt.DishIds == null)
            {
                receipt.DishIds = new List<int>();
            }

            if (!receipt.DishIds.Contains(dishId))
            {
                receipt.DishIds.Add(dishId);
                await _repository.UpdateAsync(receipt);
                await _repository.SaveChangesAsync();
            }

            var dto = new ReceiptDTO
            {
                receiptId = receipt.ReceiptId,
                tableId = receipt.TableId,
                staffId = receipt.StaffId,
                checkId = receipt.CheckId,
                wasPaid = receipt.WasPaid,
                dishIds = receipt.DishIds?.Cast<int?>().ToList() ?? new List<int?>(),
                sum = receipt.Sum,
                paymentDate = receipt.PaymentDate
            };

            return Ok(dto);
        }

        /// <summary>
        /// Видалити страву з чека
        /// </summary>
        /// <param name="id">ID чека</param>
        /// <param name="dishId">ID страви</param>
        /// <returns>Оновлені дані чека</returns>
        /// <response code="200">Страву видалено</response>
        /// <response code="404">Чек не знайдений</response>
        [HttpDelete("{id}/dishes/{dishId}")]
        [ProducesResponseType(typeof(ReceiptDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReceiptDTO>> RemoveDishFromReceipt(int id, int dishId)
        {
            var receipt = await _repository.GetByIdAsync(id);
            if (receipt == null)
            {
                return NotFound(new { message = $"Чек с ID {id} не найден" });
            }

            if (receipt.DishIds != null && receipt.DishIds.Remove(dishId))
            {
                await _repository.UpdateAsync(receipt);
                await _repository.SaveChangesAsync();
            }

            var dto = new ReceiptDTO
            {
                receiptId = receipt.ReceiptId,
                tableId = receipt.TableId,
                staffId = receipt.StaffId,
                checkId = receipt.CheckId,
                wasPaid = receipt.WasPaid,
                dishIds = receipt.DishIds?.Cast<int?>().ToList() ?? new List<int?>(),
                sum = receipt.Sum,
                paymentDate = receipt.PaymentDate
            };

            return Ok(dto);
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

            var testReceipts = new List<Models.Entities.Receipt>
            {
                new Models.Entities.Receipt
                {
                    TableId = 1,
                    StaffId = 1,
                    CheckId = 101,
                    WasPaid = false,
                    DishIds = new List<int> { 1, 2, 3 },
                    Sum = 150.50m,
                    PaymentDate = DateTime.Now
                },
                new Models.Entities.Receipt
                {
                    TableId = 2,
                    StaffId = 2,
                    CheckId = 102,
                    WasPaid = true,
                    DishIds = new List<int> { 4, 5 },
                    Sum = 89.99m,
                    PaymentDate = DateTime.Now.AddHours(-2)
                },
                new Models.Entities.Receipt
                {
                    TableId = 3,
                    StaffId = null,
                    CheckId = null,
                    WasPaid = false,
                    DishIds = new List<int>(),
                    Sum = null,
                    PaymentDate = null
                }
            };

            await _repository.AddRangeAsync(testReceipts);
            await _repository.SaveChangesAsync();
            return Ok(new { message = $"Створено {testReceipts.Count} тестових чеків" });
        }
    }
}
