using Xunit;
using FluentAssertions;
using System;
using LocalGovImsApiClient.Model;
using Application.Extensions;
using Application.Commands;

namespace Application.UnitTests.Extensions.ProcessedTransaction
{
    public class ProcessedTransactionTests
    {
        [Fact]
        public void Populate_for_good_fields()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                CouncilTaxFundCode = "23",
                BusinessRatesFundCode = "24",
                SapInvoicesFundCode = "19",
                BenefitOverpaymentFundCode = "25",
                HousingRentsFundCode = "05",
                OldCouncilTaxFundCode = "02",
                OldNonDomesticRatesFundCode = "03",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SapDebtorVatCode = "11",
                SuspenseVatCode = "M0",
                PSPReferencePrefix = "AllPay",
                MopCodePostOffice = "15",
                MopCodeCashAdjustment = "16",
                MopCodeReturnedCheque = "17",
                MopCodePayPoint = "18",
                MopCodePayzone = "19",
                MopCodeIVRDesktop = "20",
                MopCodeBillsOnline = "21",
                MopCodeOther = "22",
                PaymentTypePostOffice = "P",
                PaymentTypeCashAdjustment = "C",
                PaymentTypeReturnedCheque = "Q",
                PaymentTypePayPoint = "T",
                PaymentTypePayzone = "Z",
                PaymentTypeIVRDesktop = "N",
                PaymentTypeBillsOnline = "B",
                OfficeCode = "99",
                MaximumProcessableLineLength = 43
            };
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);
            int lineCount = 5;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            var line = " 3356107000123456          60.00T06/06/2022";

            // Act
            var result =  processedTransaction.PopulateFields(importFileModel, line, lineCount, transactionImportModel );

            // Assert
            processedTransaction.OfficeCode.Should().Be("99");
            processedTransaction.VatAmount.Should().Be(0);
            processedTransaction.VatRate.Should().Be(0);
            processedTransaction.UserCode.Should().Be(0);
            processedTransaction.MopCode.Should().Be("18");
            processedTransaction.Narrative.Should().Be("335610");
            processedTransaction.PspReference.Should().Be("AllPay" + dateTime);
            processedTransaction.TransactionDate.ToString().Should().Be("06/06/2022 00:00:00");
            processedTransaction.AccountReference.Should().Be("7000123456");
            processedTransaction.FundCode.Should().Be("02");
            processedTransaction.Amount.Should().Be((decimal)60.00);
            processedTransaction.VatCode.Should().Be("N0");
            transactionImportModel.Rows.Count.Should().Be(1);
            result.Should().Be(false);
        }

        [Fact]
        public void catch_error_for_amount_being_bad_data()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                CouncilTaxFundCode = "23",
                BusinessRatesFundCode = "24",
                SapInvoicesFundCode = "19",
                BenefitOverpaymentFundCode = "25",
                HousingRentsFundCode = "05",
                OldCouncilTaxFundCode = "02",
                OldNonDomesticRatesFundCode = "03",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SapDebtorVatCode = "11",
                SuspenseVatCode = "M0",
                PSPReferencePrefix = "AllPay",
                MopCodePostOffice = "15",
                MopCodeCashAdjustment = "16",
                MopCodeReturnedCheque = "17",
                MopCodePayPoint = "18",
                MopCodePayzone = "19",
                MopCodeIVRDesktop = "20",
                MopCodeBillsOnline = "21",
                MopCodeOther = "22",
                PaymentTypePostOffice = "P",
                PaymentTypeCashAdjustment = "C",
                PaymentTypeReturnedCheque = "Q",
                PaymentTypePayPoint = "T",
                PaymentTypePayzone = "Z",
                PaymentTypeIVRDesktop = "N",
                PaymentTypeBillsOnline = "B",
                OfficeCode = "99",
                MaximumProcessableLineLength = 43
            };

            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);
            int lineCount = 5;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            var line = " 3356107000123456         a60.00T06/06/2022";

            // Act

            // Assert
            Exception ex = Assert.Throws<FormatException>(() => processedTransaction.PopulateFields(importFileModel, line, lineCount, transactionImportModel));
            ex.Message.Should().Contain("Input string was not in a correct format");
        }

        [Fact]
        public void throws_an_error_when_line_length_wrong()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                CouncilTaxFundCode = "23",
                BusinessRatesFundCode = "24",
                SapInvoicesFundCode = "19",
                BenefitOverpaymentFundCode = "25",
                HousingRentsFundCode = "05",
                OldCouncilTaxFundCode = "02",
                OldNonDomesticRatesFundCode = "03",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SapDebtorVatCode = "11",
                SuspenseVatCode = "M0",
                PSPReferencePrefix = "AllPay",
                MopCodePostOffice = "15",
                MopCodeCashAdjustment = "16",
                MopCodeReturnedCheque = "17",
                MopCodePayPoint = "18",
                MopCodePayzone = "19",
                MopCodeIVRDesktop = "20",
                MopCodeBillsOnline = "21",
                MopCodeOther = "22",
                PaymentTypePostOffice = "P",
                PaymentTypeCashAdjustment = "C",
                PaymentTypeReturnedCheque = "Q",
                PaymentTypePayPoint = "T",
                PaymentTypePayzone = "Z",
                PaymentTypeIVRDesktop = "N",
                PaymentTypeBillsOnline = "B",
                OfficeCode = "99",
                MaximumProcessableLineLength = 43
            };

            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);
            int lineCount = 5;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            var line = " 3356107000123456        60.00T06/06/2022";

            // Act

            // Assert
            Exception ex = Assert.Throws<InvalidOperationException>(() => processedTransaction.PopulateFields(importFileModel, line, lineCount, transactionImportModel));
            ex.Message.Should().Contain("File line item wrong length");
        }

        [Fact]
        public void catch_error_for_transaction_date_being_bad_date()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                CouncilTaxFundCode = "23",
                BusinessRatesFundCode = "24",
                SapInvoicesFundCode = "19",
                BenefitOverpaymentFundCode = "25",
                HousingRentsFundCode = "05",
                OldCouncilTaxFundCode = "02",
                OldNonDomesticRatesFundCode = "03",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SapDebtorVatCode = "11",
                SuspenseVatCode = "M0",
                PSPReferencePrefix = "AllPay",
                MopCodePostOffice = "15",
                MopCodeCashAdjustment = "16",
                MopCodeReturnedCheque = "17",
                MopCodePayPoint = "18",
                MopCodePayzone = "19",
                MopCodeIVRDesktop = "20",
                MopCodeBillsOnline = "21",
                MopCodeOther = "22",
                PaymentTypePostOffice = "P",
                PaymentTypeCashAdjustment = "C",
                PaymentTypeReturnedCheque = "Q",
                PaymentTypePayPoint = "T",
                PaymentTypePayzone = "Z",
                PaymentTypeIVRDesktop = "N",
                PaymentTypeBillsOnline = "B",
                OfficeCode = "99",
                MaximumProcessableLineLength = 43
            };

            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);
            int lineCount = 5;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            var line = " 3356107000123456          60.00T06/16/2022";


            // Act

            // Assert
            Exception ex = Assert.Throws<FormatException>(() => processedTransaction.PopulateFields(importFileModel, line, lineCount, transactionImportModel));
            ex.Message.Should().Contain("The DateTime represented by the string '06/16/2022' is not supported");
        }

        [Fact]
        public void ignore_record_if_line_is_blank()
        {
            // Arrange
            ImportFileModel importFileModel = new()
            {
                CouncilTaxFundCode = "23",
                BusinessRatesFundCode = "24",
                SapInvoicesFundCode = "19",
                BenefitOverpaymentFundCode = "25",
                HousingRentsFundCode = "05",
                OldCouncilTaxFundCode = "02",
                OldNonDomesticRatesFundCode = "03",
                SuspenseFundCode = "SP",
                VatCode = "N0",
                SapDebtorVatCode = "11",
                SuspenseVatCode = "M0",
                PSPReferencePrefix = "AllPay",
                MopCodePostOffice = "15",
                MopCodeCashAdjustment = "16",
                MopCodeReturnedCheque = "17",
                MopCodePayPoint = "18",
                MopCodePayzone = "19",
                MopCodeIVRDesktop = "20",
                MopCodeBillsOnline = "21",
                MopCodeOther = "22",
                PaymentTypePostOffice = "P",
                PaymentTypeCashAdjustment = "C",
                PaymentTypeReturnedCheque = "Q",
                PaymentTypePayPoint = "T",
                PaymentTypePayzone = "Z",
                PaymentTypeIVRDesktop = "N",
                PaymentTypeBillsOnline = "B",
                OfficeCode = "99",
                MaximumProcessableLineLength = 43
            };
            TransactionImportModel transactionImportModel = new();
            transactionImportModel.Initialise(importFileModel);
            int lineCount = 8;
            ProcessedTransactionModel processedTransaction = new();
            string dateTime = DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            var line = "";


            // Act
            var result = processedTransaction.PopulateFields(importFileModel, line, lineCount, transactionImportModel);

            // Assert
            transactionImportModel.Rows.Count.Should().Be(0);
            result.Should().Be(true);
        }
    }
}
