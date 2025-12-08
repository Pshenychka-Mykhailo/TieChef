using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TieChef.Controllers;
using TieChef.Models.DTOs;
using TieChef.Models.Entities;
using TieChef.Models.Enums;
using TieChef.Repositories;
using System.Linq.Expressions;

namespace TieChef.Tests.Controllers
{
    public class StaffControllerTests
    {
        private readonly Mock<IStaffRepository> _mockRepository;
        private readonly StaffController _controller;

        public StaffControllerTests()
        {
            _mockRepository = new Mock<IStaffRepository>();
            _controller = new StaffController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllStaff_ReturnsOkWithStaff_WhenStaffExist()
        {
            // Arrange
            var staffList = new List<Staff>
            {
                new Staff { StaffId = 1, FullName = "John Doe", Email = "john@test.com", PhoneNumber = 1234567890, Salary = 50000, Type = e_StaffType.Manager, Role = e_StaffRole.Trainer },
                new Staff { StaffId = 2, FullName = "Jane Smith", Email = "jane@test.com", PhoneNumber = 987654321, Salary = 55000, Type = e_StaffType.Manager, Role = e_StaffRole.Nutritionist }
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(staffList);

            // Act
            var result = await _controller.GetAllStaff();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStaff = okResult.Value.Should().BeAssignableTo<IEnumerable<StaffDTO>>().Subject;
            returnedStaff.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetStaff_ReturnsOkWithStaff_WhenStaffExists()
        {
            // Arrange
            var staff = new Staff 
            { 
                StaffId = 1, 
                FullName = "John Doe", 
                Email = "john@test.com", 
                PhoneNumber = 123456, 
                Salary = 5000m,
                Type = e_StaffType.Manager,
                Role = e_StaffRole.Trainer,
                StartWorkDate = DateTime.Now
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(staff);

            // Act
            var result = await _controller.GetStaff(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStaff = okResult.Value.Should().BeOfType<StaffDTO>().Subject;
            returnedStaff.fullName.Should().Be("John Doe");
            returnedStaff.staffId.Should().Be(1);
        }

        [Fact]
        public async Task GetStaff_ReturnsNotFound_WhenStaffDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Staff?)null);

            // Act
            var result = await _controller.GetStaff(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateStaff_ReturnsCreatedAtAction_WithValidStaff()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "New Employee",
                Email = "new@test.com",
                phoneNumber = 111222,
                salary = 4500m,
                type = e_StaffType.Cleaner,
                role = e_StaffRole.Receptionist,
                startWorkDate = DateTime.Now
            };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Staff>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateStaff(staffDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(StaffController.GetStaff));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Staff>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStaff_ReturnsOkWithUpdatedStaff_WhenUpdateSucceeds()
        {
            // Arrange
            var existingStaff = new Staff
            {
                StaffId = 1,
                FullName = "John Doe",
                Email = "john@test.com",
                PhoneNumber = 123456,
                Salary = 5000m,
                Type = e_StaffType.Manager,
                Role = e_StaffRole.Trainer,
                StartWorkDate = DateTime.Now
            };
            var staffDto = new StaffDTO
            {
                staffId = 1,
                fullName = "John Updated",
                Email = "john.updated@test.com",
                phoneNumber = 123456,
                salary = 5500m,
                type = e_StaffType.Manager,
                role = e_StaffRole.Trainer,
                startWorkDate = DateTime.Now
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingStaff);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Staff>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateStaff(1, staffDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Staff>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStaff_ReturnsNotFound_WhenStaffDoesNotExist()
        {
            // Arrange
            var staffDto = new StaffDTO { staffId = 999, fullName = "Test" };
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Staff?)null);

            // Act
            var result = await _controller.UpdateStaff(999, staffDto);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteStaff_ReturnsOk_WhenStaffExists()
        {
            // Arrange
            var staff = new Staff { StaffId = 1, FullName = "John Doe" };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(staff);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Staff>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteStaff(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.DeleteAsync(staff), Times.Once);
        }

        [Fact]
        public async Task DeleteStaff_ReturnsNotFound_WhenStaffDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Staff?)null);

            // Act
            var result = await _controller.DeleteStaff(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetStaffByType_ReturnsFilteredStaff()
        {
            // Arrange
            var staffList = new List<Staff>
            {
                new Staff { StaffId = 1, FullName = "John Doe", Type = e_StaffType.Manager },
                new Staff { StaffId = 2, FullName = "Jane Smith", Type = e_StaffType.Cleaner }
            };
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Staff, bool>>>()))
                .ReturnsAsync((Expression<Func<Staff, bool>> predicate) => staffList.Where(predicate.Compile()));

            // Act
            var result = await _controller.GetStaffByType(e_StaffType.Manager);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStaff = okResult.Value.Should().BeAssignableTo<IEnumerable<StaffDTO>>().Subject;
            returnedStaff.Should().HaveCount(1);
            returnedStaff.First().type.Should().Be(e_StaffType.Manager);
        }

        [Fact]
        public async Task GetStaffByRole_ReturnsFilteredStaff()
        {
            // Arrange
            var staffList = new List<Staff>
            {
                new Staff { StaffId = 1, FullName = "John Doe", Role = e_StaffRole.Manager },
                new Staff { StaffId = 2, FullName = "Jane Smith", Role = e_StaffRole.Trainer }
            };
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Staff, bool>>>()))
                .ReturnsAsync((Expression<Func<Staff, bool>> predicate) => staffList.Where(predicate.Compile()));

            // Act
            var result = await _controller.GetStaffByRole(e_StaffRole.Manager);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStaff = okResult.Value.Should().BeAssignableTo<IEnumerable<StaffDTO>>().Subject;
            returnedStaff.Should().HaveCount(1);
            returnedStaff.First().fullName.Should().Be("John Doe");
        }
    }
}
