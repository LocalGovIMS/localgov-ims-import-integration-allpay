using Application.Commands;
using Application.Extensions;
using FluentAssertions;
using LocalGovImsApiClient.Model;
using System;
using Xunit;

namespace Application.UnitTests.Extensions.DecimalTests
{
    public class DecimalTests
    {
        [Fact]
        public void Return_true_when_totals_are_valid()
        {
            // Arrange
            decimal fileTotalAmount = (decimal)350.12;
            int fileTotalTransactionCount = (int)2;
            ImportFileModel importFileModel = new()
            {
                TransactionImportTypeId = 1
            };
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);
            ProcessedTransactionModel processedTransactionModel = new()
            {
                Amount = (decimal)100.00,
            };
            transactionImportModel.Rows.Add(processedTransactionModel);
            ProcessedTransactionModel processedTransactionModel2= new()
            {
                Amount = (decimal)250.12
            };
            transactionImportModel.Rows.Add(processedTransactionModel2);
            // Act
            var result = fileTotalAmount.CheckFileTotalsAreCorrect(fileTotalTransactionCount,transactionImportModel);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Throws_error_when_amounts_are_not_equal_valid()
        {
            // Arrange
            decimal fileTotalAmount = (decimal)350.10;
            int fileTotalTransactionCount = (int)2;
            ImportFileModel importFileModel = new()
            {
                TransactionImportTypeId = 1
            };
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);
            ProcessedTransactionModel processedTransactionModel = new()
            {
                Amount = 100,
            };
            transactionImportModel.Rows.Add(processedTransactionModel);
            processedTransactionModel.Amount = (decimal)250.12;
            transactionImportModel.Rows.Add(processedTransactionModel);

            // Act

            // Assert
            Exception ex = Assert.Throws<InvalidOperationException>(() => fileTotalAmount.CheckFileTotalsAreCorrect(fileTotalTransactionCount, transactionImportModel));
            ex.Message.Should().Contain("The files Total Amount of");
        }

        [Fact]
        public void Throws_error_when_counts_are_not_equal_valid()
        {
            // Arrange
            decimal fileTotalAmount = (decimal)100.01;
            int fileTotalTransactionCount = (int)3;
            ImportFileModel importFileModel = new()
            {
                TransactionImportTypeId = 1
            };
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);
            ProcessedTransactionModel processedTransactionModel = new()
            {
                Amount = (decimal)100.01,
            };
            transactionImportModel.Rows.Add(processedTransactionModel);

            // Act

            // Assert
            Exception ex = Assert.Throws<InvalidOperationException>(() => fileTotalAmount.CheckFileTotalsAreCorrect(fileTotalTransactionCount, transactionImportModel));
            ex.Message.Should().Contain("The files Total count of");
        }

        [Fact]
        public void Can_cope_with_no_records()
        {
            // Arrange
            decimal fileTotalAmount = (decimal)0;
            int fileTotalTransactionCount = (int)0;
            ImportFileModel importFileModel = new() 
            {
                TransactionImportTypeId = 1
            };
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);

            // Act
            var result = fileTotalAmount.CheckFileTotalsAreCorrect(fileTotalTransactionCount, transactionImportModel);

            // Assert
            result.Should().Be(true);
        }
    }
}
