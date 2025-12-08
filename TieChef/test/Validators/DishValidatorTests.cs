using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;
using TieChef.Models.Entities;
using TieChef.Validators;

namespace TieChef.Tests.Validators
{
    public class DishValidatorTests
    {
        private readonly DishValidator _validator;

        public DishValidatorTests()
        {
            _validator = new DishValidator();
        }

        [Fact]
        public void Validate_Succeeds_WithValidDish()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                Name = "Pizza Margherita",
                Price = 12.99m,
                Description = "Classic Italian pizza"
            };

            // Act
            var result = _validator.TestValidate(dish);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_Fails_WhenNameIsEmpty()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                Name = "",
                Price = 12.99m
            };

            // Act
            var result = _validator.TestValidate(dish);

            // Assert
            result.ShouldHaveValidationErrorFor(d => d.Name)
                .WithErrorMessage("Dish Name is required");
        }

        [Fact]
        public void Validate_Fails_WhenNameIsTooShort()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                Name = "A",
                Price = 12.99m
            };

            // Act
            var result = _validator.TestValidate(dish);

            // Assert
            result.ShouldHaveValidationErrorFor(d => d.Name)
                .WithErrorMessage("Dish Name must be between 2 and 100 characters");
        }

        [Fact]
        public void Validate_Fails_WhenNameIsTooLong()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                Name = new string('A', 101),
                Price = 12.99m
            };

            // Act
            var result = _validator.TestValidate(dish);

            // Assert
            result.ShouldHaveValidationErrorFor(d => d.Name)
                .WithErrorMessage("Dish Name must be between 2 and 100 characters");
        }

        [Fact]
        public void Validate_Fails_WhenPriceIsZero()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                Name = "Pizza",
                Price = 0m
            };

            // Act
            var result = _validator.TestValidate(dish);

            // Assert
            result.ShouldHaveValidationErrorFor(d => d.Price)
                .WithErrorMessage("Price must be greater than 0");
        }

        [Fact]
        public void Validate_Fails_WhenPriceIsNegative()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                Name = "Pizza",
                Price = -5.99m
            };

            // Act
            var result = _validator.TestValidate(dish);

            // Assert
            result.ShouldHaveValidationErrorFor(d => d.Price)
                .WithErrorMessage("Price must be greater than 0");
        }

        [Fact]
        public void Validate_Fails_WhenPriceHasTooManyDecimals()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                Name = "Pizza",
                Price = 12.999m
            };

            // Act
            var result = _validator.TestValidate(dish);

            // Assert
            result.ShouldHaveValidationErrorFor(d => d.Price)
                .WithErrorMessage("Price cannot have more than 2 decimal places");
        }

        [Fact]
        public void Validate_Fails_WhenDescriptionIsTooLong()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                Name = "Pizza",
                Price = 12.99m,
                Description = new string('A', 501)
            };

            // Act
            var result = _validator.TestValidate(dish);

            // Assert
            result.ShouldHaveValidationErrorFor(d => d.Description)
                .WithErrorMessage("Description must not exceed 500 characters");
        }

        [Fact]
        public void Validate_Succeeds_WithNullDescription()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                Name = "Pizza",
                Price = 12.99m,
                Description = null
            };

            // Act
            var result = _validator.TestValidate(dish);

            // Assert
            result.ShouldNotHaveValidationErrorFor(d => d.Description);
        }
    }
}
