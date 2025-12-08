using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TieChef.Controllers;
using TieChef.Models.DTOs;
using TieChef.Models.Entities;
using TieChef.Repositories;

namespace TieChef.Tests.Controllers
{
    public class ReceiptControllerTests
    {
        private readonly Mock<IReceiptRepository> _mockRepository;
        private readonly ReceiptController _controller;

        public ReceiptControllerTests()
        {
            _mockRepository = new Mock<IReceiptRepository>();
            _controller = new ReceiptController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllReceipts_ReturnsOkWithReceipts_WhenReceiptsExist()
        {
            // Arrange
            var receipts = new List<Receipt>
            {
                new Receipt { ReceiptId = 1, TableId = 1, WasPaid = false, Sum = 50.00m },
                new Receipt { ReceiptId = 2, TableId = 2, WasPaid = true, Sum = 75.50m }
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(receipts);

            // Act
            var result = await _controller.GetAllReceipts();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedReceipts = okResult.Value.Should().BeAssignableTo<IEnumerable<ReceiptDTO>>().Subject;
            returnedReceipts.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetReceipt_ReturnsOkWithReceipt_WhenReceiptExists()
        {
            // Arrange
            var receipt = new Receipt 
            { 
                ReceiptId = 1, 
                TableId = 5, 
                WasPaid = false, 
                Sum = 100.00m,
                DishIds = new List<int> { 1, 2, 3 }
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(receipt);

            // Act
            var result = await _controller.GetReceipt(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedReceipt = okResult.Value.Should().BeOfType<ReceiptDTO>().Subject;
            returnedReceipt.receiptId.Should().Be(1);
            returnedReceipt.tableId.Should().Be(5);
        }

        [Fact]
        public async Task GetReceipt_ReturnsNotFound_WhenReceiptDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Receipt?)null);

            // Act
            var result = await _controller.GetReceipt(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateReceipt_ReturnsCreatedAtAction_WithValidReceipt()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 1,
                staffId = 1,
                wasPaid = false,
                dishIds = new List<int?> { 1, 2 },
                sum = 50.00m
            };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Receipt>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateReceipt(receiptDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(ReceiptController.GetReceipt));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Receipt>()), Times.Once);
        }

        [Fact]
        public async Task UpdateReceipt_ReturnsOkWithUpdatedReceipt_WhenUpdateSucceeds()
        {
            // Arrange
            var existingReceipt = new Receipt
            {
                ReceiptId = 1,
                TableId = 1,
                WasPaid = false,
                Sum = 50.00m
            };
            var receiptDto = new ReceiptDTO
            {
                receiptId = 1,
                tableId = 1,
                wasPaid = true,
                sum = 60.00m
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingReceipt);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Receipt>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateReceipt(1, receiptDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Receipt>()), Times.Once);
        }

        [Fact]
        public async Task UpdateReceipt_ReturnsNotFound_WhenReceiptDoesNotExist()
        {
            // Arrange
            var receiptDto = new ReceiptDTO { receiptId = 999, tableId = 1 };
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Receipt?)null);

            // Act
            var result = await _controller.UpdateReceipt(999, receiptDto);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteReceipt_ReturnsOk_WhenReceiptExists()
        {
            // Arrange
            var receipt = new Receipt { ReceiptId = 1, TableId = 1 };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(receipt);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Receipt>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteReceipt(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.DeleteAsync(receipt), Times.Once);
        }

        [Fact]
        public async Task DeleteReceipt_ReturnsNotFound_WhenReceiptDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Receipt?)null);

            // Act
            var result = await _controller.DeleteReceipt(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetReceiptsByPaymentStatus_ReturnsFilteredReceipts()
        {
            // Arrange
            var receipts = new List<Receipt>
            {
                new Receipt { ReceiptId = 1, WasPaid = true },
                new Receipt { ReceiptId = 2, WasPaid = false }
            };
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Receipt, bool>>>()))
                .ReturnsAsync(receipts.Where(r => r.WasPaid == true));

            // Act
            var result = await _controller.GetReceiptsByPaymentStatus(true);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedReceipts = okResult.Value.Should().BeAssignableTo<IEnumerable<ReceiptDTO>>().Subject;
            returnedReceipts.Should().HaveCount(1);
            returnedReceipts.First().wasPaid.Should().BeTrue();
        }

        [Fact]
        public async Task GetReceiptsByStaff_ReturnsStaffReceipts()
        {
            // Arrange
            var receipts = new List<Receipt>
            {
                new Receipt { ReceiptId = 1, StaffId = 5 },
                new Receipt { ReceiptId = 2, StaffId = 10 }
            };
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Receipt, bool>>>()))
                .ReturnsAsync(receipts.Where(r => r.StaffId == 5));

            // Act
            var result = await _controller.GetReceiptsByStaff(5);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedReceipts = okResult.Value.Should().BeAssignableTo<IEnumerable<ReceiptDTO>>().Subject;
            returnedReceipts.Should().HaveCount(1);
            returnedReceipts.First().staffId.Should().Be(5);
        }

        [Fact]
        public async Task UpdatePaymentStatus_UpdatesStatus_WhenReceiptExists()
        {
            // Arrange
            var receipt = new Receipt 
            { 
                ReceiptId = 1, 
                TableId = 1, 
                WasPaid = false,
                DishIds = new List<int> { 1, 2 },
                Sum = 50.00m
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(receipt);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Receipt>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePaymentStatus(1, true);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<Receipt>(r => r.WasPaid == true)), Times.Once);
        }

        [Fact]
        public async Task UpdatePaymentStatus_ReturnsNotFound_WhenReceiptDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Receipt?)null);

            // Act
            var result = await _controller.UpdatePaymentStatus(999, true);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
