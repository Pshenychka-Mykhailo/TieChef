using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;
using TieChef.Models.DTOs;
using TieChef.Models.Enums;
using TieChef.Validators;

namespace TieChef.Tests.Validators
{
    public class StaffDTOValidatorTests
    {
        private readonly StaffDTOValidator _validator;

        public StaffDTOValidatorTests()
        {
            _validator = new StaffDTOValidator();
        }

        [Fact]
        public void Validate_Succeeds_WithValidStaffDTO()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                staffId = 1,
                fullName = "John Doe",
                Email = "john.doe@example.com",
                phoneNumber = 123456789,
                salary = 5000.00m,
                type = e_StaffType.Manager,
                role = e_StaffRole.Trainer,
                startWorkDate = DateTime.Now,
                KPI = "Excellent performance"
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_Fails_WhenFullNameIsEmpty()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "",
                Email = "john@example.com",
                phoneNumber = 123456,
                salary = 5000m
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.fullName)
                .WithErrorMessage("Full Name is required");
        }

        [Fact]
        public void Validate_Fails_WhenFullNameIsTooShort()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "A",
                Email = "john@example.com",
                phoneNumber = 123456,
                salary = 5000m
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.fullName)
                .WithErrorMessage("Full Name must be between 2 and 100 characters");
        }

        [Fact]
        public void Validate_Fails_WhenFullNameHasInvalidCharacters()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "John123",
                Email = "john@example.com",
                phoneNumber = 123456,
                salary = 5000m
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.fullName)
                .WithErrorMessage("Full Name must contain only letters and spaces");
        }

        [Fact]
        public void Validate_Fails_WhenEmailIsEmpty()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "John Doe",
                Email = "",
                phoneNumber = 123456,
                salary = 5000m
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.Email)
                .WithErrorMessage("Email is required");
        }

        [Fact]
        public void Validate_Fails_WhenEmailIsInvalid()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "John Doe",
                Email = "invalid-email",
                phoneNumber = 123456,
                salary = 5000m
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.Email)
                .WithErrorMessage("Invalid Email format");
        }

        [Fact]
        public void Validate_Fails_WhenPhoneNumberIsInvalid()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "John Doe",
                Email = "john@example.com",
                phoneNumber = 100,
                salary = 5000m
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.phoneNumber)
                .WithErrorMessage("Phone Number must be valid");
        }

        [Fact]
        public void Validate_Fails_WhenSalaryIsZero()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "John Doe",
                Email = "john@example.com",
                phoneNumber = 123456,
                salary = 0m
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.salary)
                .WithErrorMessage("Salary must be greater than 0");
        }

        [Fact]
        public void Validate_Fails_WhenSalaryIsNegative()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "John Doe",
                Email = "john@example.com",
                phoneNumber = 123456,
                salary = -1000m
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.salary)
                .WithErrorMessage("Salary must be greater than 0");
        }

        [Fact]
        public void Validate_Fails_WhenKPIIsTooLong()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "John Doe",
                Email = "john@example.com",
                phoneNumber = 123456,
                salary = 5000m,
                KPI = new string('A', 501)
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldHaveValidationErrorFor(s => s.KPI)
                .WithErrorMessage("KPI must not exceed 500 characters");
        }

        [Fact]
        public void Validate_Succeeds_WithNullKPI()
        {
            // Arrange
            var staffDto = new StaffDTO
            {
                fullName = "John Doe",
                Email = "john@example.com",
                phoneNumber = 123456,
                salary = 5000m,
                KPI = null
            };

            // Act
            var result = _validator.TestValidate(staffDto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(s => s.KPI);
        }
    }
}
