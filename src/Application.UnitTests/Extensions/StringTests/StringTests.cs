using Application.Commands;
using Application.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace Application.UnitTests.Extensions.String
{
    public class StringTests
    {
        [Fact]
        public void Check_for_a_council_tax_account()
        {
            // Arrange
            string accountReference = "771234567";

            // Act
            var result = accountReference.IsCouncilTax();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Should_fail_check_for_a_council_tax_account()
        {
            // Arrange
            string accountReference = "711234567";

            // Act
            var result = accountReference.IsCouncilTax();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Check_for_a_business_rates()
        {
            // Arrange
            string accountReference = "560000003";

            // Act
            var result = accountReference.IsBusinessRates();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Should_fail_check_for_a_business_rates()
        {
            // Arrange
            string accountReference = "5600000031";

            // Act
            var result = accountReference.IsBusinessRates();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Check_for_a_benefit_overpayment()
        {
            // Arrange
            string accountReference = "71234567";

            // Act
            var result = accountReference.IsBenefitOverpayment();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Check_not_a_benefit_overpayment()
        {
            // Arrange
            string accountReference = "710453824";

            // Act
            var result = accountReference.IsBenefitOverpayment();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Check_for_a_sap_invoice()
        {
            // Arrange
            string accountReference = "9123456789";

            // Act
            var result = accountReference.IsSapInvoice();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Check_not_a_sap_invoice()
        {
            // Arrange
            string accountReference = "91123456789";

            // Act
            var result = accountReference.IsSapInvoice();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Check_for_a_old_non_domestic_rate()
        {
            // Arrange
            string accountReference = "5510002361";

            // Act
            var result = accountReference.IsOldNonDomesticRates();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Check_not_a_old_non_domestic_rate()
        {
            // Arrange
            string accountReference = "5000236195";

            // Act
            var result = accountReference.IsOldNonDomesticRates();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Check_for_a_old_council_tax()
        {
            // Arrange
            string accountReference = "7012345678";

            // Act
            var result = accountReference.IsOldCouncilTax();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Check_not_a_old_council_tax()
        {
            // Arrange
            string accountReference = "7712345678";

            // Act
            var result = accountReference.IsOldCouncilTax();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Check_for_a_housing_rent()
        {
            // Arrange
            string accountReference = "61111111111";

            // Act
            var result = accountReference.IsHousingRents();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Check_not_a_housing_rent()
        {
            // Arrange
            string accountReference = "51111111111";

            // Act
            var result = accountReference.IsHousingRents();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Set_account_reference_for_bern()
        {
            // Arrange
            string line = " 261398BERN61234567890     73.63T07/06/2022";

            // Act
            var accountReference = line.SetAccountReference();

            // Assert
            accountReference.Should().Be("61234567890");
        }

        [Fact]
        public void Set_account_reference_for_not_bern_ref()
        {
            // Arrange
            string line = " 26139861234567890         73.63T07/06/2022";

            // Act
            var accountReference = line.SetAccountReference();

            // Assert
            accountReference.Should().Be("61234567890");
        }

        [Fact]
        public void Set_council_tax_fund_code()
        {
            // Arrange
            string field = "770778675";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("23");
        }

        [Fact]
        public void Set_business_rates_fund_code()
        {
            // Arrange
            string field = "560000003";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("24");
        }

        [Fact]
        public void Set_sap_invoice_fund_code()
        {
            // Arrange
            string field = "9000236195";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("19");
        }

        [Fact]
        public void Set_benefit_overpayment_fund_code()
        {
            // Arrange
            string field = "71045382";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("25");
        }

        [Fact]
        public void Set_housing_rent_fund_code()
        {
            // Arrange
            string field = "61111111111";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("05");
        }

        [Fact]
        public void Set_old_council_tax_fund_code()
        {
            // Arrange
            string field = "7011111111";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("02");
        }


        [Fact]
        public void Set_old_non_domestic_rate_fund_code()
        {
            // Arrange
            string field = "5511111111";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("03");
        }

        [Fact]
        public void return_suspense_if_fund_not_known()
        {
            // Arrange
            string field = "11045382";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";

            // Act
            var fundCode = field.SetFundCode(importFileModel);

            // Assert
            fundCode.Should().Be("SP");
        }

        [Fact]
        public void Set_vat_code_to_default_for_council_tax()
        {
            // Arrange
            string field = "23";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";
            importFileModel.VatCode = "N0";
            importFileModel.SapDebtorVatCode = "11";
            importFileModel.SuspenseVatCode = "M0";

            // Act
            var vatCode = field.SetVatCode(importFileModel);

            // Assert
            vatCode.Should().Be("N0");
        }

        [Fact]
        public void Set_vat_code_to_default_for_busiess_rate_fund()
        {
            // Arrange
            string field = "24";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";
            importFileModel.VatCode = "N0";
            importFileModel.SapDebtorVatCode = "11";
            importFileModel.SuspenseVatCode = "M0";

            // Act
            var vatCode = field.SetVatCode(importFileModel);

            // Assert
            vatCode.Should().Be("N0");
        }

        [Fact]
        public void Set_vat_code_to_default_for_benefits_fund_code()
        {
            // Arrange
            string field = "25";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";
            importFileModel.VatCode = "N0";
            importFileModel.SapDebtorVatCode = "11";
            importFileModel.SuspenseVatCode = "M0";

            // Act
            var vatCode = field.SetVatCode(importFileModel);

            // Assert
            vatCode.Should().Be("N0");
        }

        [Fact]
        public void Set_vat_code_to_sap_vat_code_for_sap_invoice()
        {
            // Arrange
            string field = "19";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";
            importFileModel.VatCode = "N0";
            importFileModel.SapDebtorVatCode = "11";
            importFileModel.SuspenseVatCode = "M0";

            // Act
            var vatCode = field.SetVatCode(importFileModel);

            // Assert
            vatCode.Should().Be("11");
        }


        [Fact]
        public void Set_suspense_vat_code()
        {
            // Arrange
            string field = "SP";
            var importFileModel = new ImportFileModel();
            importFileModel.CouncilTaxFundCode = "23";
            importFileModel.BusinessRatesFundCode = "24";
            importFileModel.SapInvoicesFundCode = "19";
            importFileModel.BenefitOverpaymentFundCode = "25";
            importFileModel.HousingRentsFundCode = "05";
            importFileModel.OldCouncilTaxFundCode = "02";
            importFileModel.OldNonDomesticRatesFundCode = "03";
            importFileModel.SuspenseFundCode = "SP";
            importFileModel.VatCode = "N0";
            importFileModel.SapDebtorVatCode = "11";
            importFileModel.SuspenseVatCode = "M0";

            // Act
            var vatCode = field.SetVatCode(importFileModel);

            // Assert
            vatCode.Should().Be("M0");
        }

        [Fact]
        public void Check_mop_code_for_post_office()
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
            var mopCode = "P";

            // Act
            var result = mopCode.SetMopCode(importFileModel);

            // Assert
            result.Should().Be("15");
        }

        [Fact]
        public void Check_mop_code_for_cash_adjustment()
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
            var mopCode = "C";

            // Act
            var result = mopCode.SetMopCode(importFileModel);

            // Assert
            result.Should().Be("16");
        }
        [Fact]
        public void Check_mop_code_for_returned_cheque()
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
            var mopCode = "Q";

            // Act
            var result = mopCode.SetMopCode(importFileModel);

            // Assert
            result.Should().Be("17");
        }
        [Fact]
        public void Check_mop_code_for_paypoint()
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
            var mopCode = "T";

            // Act
            var result = mopCode.SetMopCode(importFileModel);

            // Assert
            result.Should().Be("18");
        }
        [Fact]
        public void Check_mop_code_for_payzone()
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
            var mopCode = "Z";

            // Act
            var result = mopCode.SetMopCode(importFileModel);

            // Assert
            result.Should().Be("19");
        }
        [Fact]
        public void Check_mop_code_for_ivr_desktop()
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
            var mopCode = "N";

            // Act
            var result = mopCode.SetMopCode(importFileModel);

            // Assert
            result.Should().Be("20");
        }
        [Fact]
        public void Check_mop_code_for_billsonline()
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
            var mopCode = "B";

            // Act
            var result = mopCode.SetMopCode(importFileModel);

            // Assert
            result.Should().Be("21");
        }
        [Fact]
        public void Check_mop_code_for_default_other()
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
            var mopCode = "A";

            // Act
            var result = mopCode.SetMopCode(importFileModel);

            // Assert
            result.Should().Be("22");
        }
        [Fact]
        public void Check_its_a_detail_line()
        {
            // Arrange
            var line = " 3333333333";

            // Act
            var result = line.PotentialDetailLine();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Check_its_not_a_detail_line()
        {
            // Arrange
            var line = "3333333333";

            // Act
            var result = line.PotentialDetailLine();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Is_a_total_amount_line()
        {
            // Arrange
            var line = "Total Amount Updated";

            // Act
            var result = line.TotalAmount();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Is_not_a_total_amount_line()
        {
            // Arrange
            var line = " Total Transactions for this line";

            // Act
            var result = line.TotalAmount();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void Is_a_total_transaction_count_line()
        {
            // Arrange
            var line = "Total Transactions updated";

            // Act
            var result = line.TotalTransactionCount();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void Is_not_a_total_transaction_count_line()
        {
            // Arrange
            var line = "Total Amount for this line";

            // Act
            var result = line.TotalTransactionCount();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void return_valid_file_total_amount()
        {
            // Arrange
            var line = "Total Amount Updated onto System:            419.00";

            // Act
            var result = line.FileTotalAmount();

            // Assert
            result.Should().Be((decimal)419.00);
        }

        [Fact]
        public void returns_error_for_bad_file_total_amount()
        {
            // Arrange
            var line = "Total Amount Updated onto System:           a419.00";

            // Act

            // Assert
            Exception ex = Assert.Throws<FormatException>(() => line.FileTotalAmount());
        }

        [Fact]
        public void return_valid_file_total_count()
        {
            // Arrange
            var line = "Total Transactions updated onto System:                 4";

            // Act
            var result = line.FileTotalTransactionCount();

            // Assert
            result.Should().Be(4);
        }

        [Fact]
        public void returns_error_for_bad_file_total_count()
        {
            // Arrange
            var line = "Total Transactions updated onto System:                a4";

            // Act

            // Assert
            Exception ex = Assert.Throws<FormatException>(() => line.FileTotalTransactionCount());
        }
    }
}
