using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;
using TieChef.Models.DTOs;
using TieChef.Validators;

namespace TieChef.Tests.Validators
{
    public class ReceiptDTOValidatorTests
    {
        private readonly ReceiptDTOValidator _validator;

        public ReceiptDTOValidatorTests()
        {
            _validator = new ReceiptDTOValidator();
        }

        [Fact]
        public void Validate_Succeeds_WithValidReceiptDTO()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                receiptId = 1,
                tableId = 5,
                staffId = 10,
                wasPaid = true,
                dishIds = new List<int?> { 1, 2, 3 },
                sum = 150.50m,
                paymentDate = DateTime.Now
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_Fails_WhenTableIdIsZero()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 0,
                sum = 100m
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.tableId)
                .WithErrorMessage("Table ID must be greater than 0");
        }

        [Fact]
        public void Validate_Fails_WhenTableIdIsNegative()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = -1,
                sum = 100m
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.tableId)
                .WithErrorMessage("Table ID must be greater than 0");
        }

        [Fact]
        public void Validate_Fails_WhenStaffIdIsZeroButProvided()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 1,
                staffId = 0,
                sum = 100m
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.staffId)
                .WithErrorMessage("Staff ID must be greater than 0");
        }

        [Fact]
        public void Validate_Succeeds_WhenStaffIdIsNull()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 1,
                staffId = null,
                sum = 100m
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.staffId);
        }

        [Fact]
        public void Validate_Fails_WhenSumIsNegative()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 1,
                sum = -50m
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.sum)
                .WithErrorMessage("Sum cannot be negative");
        }

        [Fact]
        public void Validate_Succeeds_WhenSumIsZero()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 1,
                sum = 0m
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.sum);
        }

        [Fact]
        public void Validate_Fails_WhenSumHasTooManyDecimals()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 1,
                sum = 100.999m
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.sum)
                .WithErrorMessage("Sum cannot have more than 2 decimal places");
        }

        [Fact]
        public void Validate_Fails_WhenPaidReceiptHasNoDishes()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 1,
                wasPaid = true,
                dishIds = new List<int?>(),
                sum = 100m
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.dishIds)
                .WithErrorMessage("Paid receipt must contain dishes");
        }

        [Fact]
        public void Validate_Succeeds_WhenUnpaidReceiptHasNoDishes()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 1,
                wasPaid = false,
                dishIds = new List<int?>(),
                sum = 0m
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.dishIds);
        }

        [Fact]
        public void Validate_Succeeds_WithNullSum()
        {
            // Arrange
            var receiptDto = new ReceiptDTO
            {
                tableId = 1,
                sum = null
            };

            // Act
            var result = _validator.TestValidate(receiptDto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.sum);
        }
    }
}
