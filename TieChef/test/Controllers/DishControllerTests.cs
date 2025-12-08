using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using TieChef.Controllers;
using TieChef.Models.Entities;
using TieChef.Repositories;

namespace TieChef.Tests.Controllers
{
    public class DishControllerTests
    {
        private readonly Mock<IDishRepository> _mockRepository;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<ILogger<DishController>> _mockLogger;
        private readonly DishController _controller;

        public DishControllerTests()
        {
            _mockRepository = new Mock<IDishRepository>();
            _mockCache = new Mock<IDistributedCache>();
            _mockLogger = new Mock<ILogger<DishController>>();
            _controller = new DishController(_mockRepository.Object, _mockCache.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithDishes_WhenDishesExist()
        {
            // Arrange
            var dishes = new List<Dish>
            {
                new Dish { DishId = 1, Name = "Pizza", Price = 10.99m, Description = "Delicious pizza" },
                new Dish { DishId = 2, Name = "Pasta", Price = 8.99m, Description = "Fresh pasta" }
            };
            
            _mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(dishes);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedDishes = okResult.Value.Should().BeAssignableTo<IEnumerable<Dish>>().Subject;
            returnedDishes.Should().HaveCount(2);
            returnedDishes.Should().Contain(d => d.Name == "Pizza");
        }

        [Fact]
        public async Task GetAll_CachesData_WhenCacheMiss()
        {
            // Arrange
            var dishes = new List<Dish>
            {
                new Dish { DishId = 1, Name = "Pizza", Price = 10.99m }
            };
            
            _mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(dishes);

            // Act
            await _controller.GetAll();

            // Assert
            _mockCache.Verify(c => c.SetAsync(
                "dishes_list",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_ReturnsOkWithDish_WhenDishExists()
        {
            // Arrange
            var dish = new Dish { DishId = 1, Name = "Pizza", Price = 10.99m };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dish);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedDish = okResult.Value.Should().BeOfType<Dish>().Subject;
            returnedDish.Name.Should().Be("Pizza");
            returnedDish.DishId.Should().Be(1);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenDishDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Dish?)null);

            // Act
            var result = await _controller.Get(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidDish()
        {
            // Arrange
            var newDish = new Dish { DishId = 0, Name = "Burger", Price = 12.99m };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Dish>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(newDish);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(DishController.Get));
            var returnedDish = createdResult.Value.Should().BeOfType<Dish>().Subject;
            returnedDish.Name.Should().Be("Burger");
        }

        [Fact]
        public async Task Create_InvalidatesCache_AfterCreation()
        {
            // Arrange
            var newDish = new Dish { DishId = 0, Name = "Burger", Price = 12.99m };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Dish>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _controller.Create(newDish);

            // Assert
            _mockCache.Verify(c => c.RemoveAsync("dishes_list", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
        {
            // Arrange
            var dish = new Dish { DishId = 1, Name = "Updated Pizza", Price = 11.99m };
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Dish>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, dish);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.UpdateAsync(dish), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var dish = new Dish { DishId = 2, Name = "Pizza", Price = 10.99m };

            // Act
            var result = await _controller.Update(1, dish);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task Update_InvalidatesCache_AfterUpdate()
        {
            // Arrange
            var dish = new Dish { DishId = 1, Name = "Updated Pizza", Price = 11.99m };
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Dish>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _controller.Update(1, dish);

            // Assert
            _mockCache.Verify(c => c.RemoveAsync("dishes_list", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDishExists()
        {
            // Arrange
            var dish = new Dish { DishId = 1, Name = "Pizza", Price = 10.99m };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dish);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Dish>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.DeleteAsync(dish), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenDishDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Dish?)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task Delete_InvalidatesCache_AfterDeletion()
        {
            // Arrange
            var dish = new Dish { DishId = 1, Name = "Pizza", Price = 10.99m };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dish);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Dish>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _controller.Delete(1);

            // Assert
            _mockCache.Verify(c => c.RemoveAsync("dishes_list", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
