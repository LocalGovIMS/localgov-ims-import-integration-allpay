using Microsoft.Extensions.Configuration;
using System.IO.Abstractions;
using Moq;
using Application.Commands;
using Command = Application.Commands.ImportFileCommand;
using Handler = Application.Commands.ImportFileCommandHandler;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using System.Threading;
using LocalGovImsApiClient.Model;

namespace Application.UnitTests.Commands
{
    public class HandleTests
    {
        private const string FileLocation = "c:\\temp\\allpayfile.txt";

        
        private readonly Handler _commandHandler;
        private Command _command;


        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();
        private readonly Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();
        private readonly Mock<LocalGovImsApiClient.Api.ITransactionImportApi> _mockTransactionImportApi = new Mock<LocalGovImsApiClient.Api.ITransactionImportApi>();


        public HandleTests()
        {
            _commandHandler = new Handler(
                _mockConfiguration.Object,
                _mockFileSystem.Object,
                _mockTransactionImportApi.Object);

            SetUpConfig();
            SetUpCommand();


        }

        private void SetUpConfig()
        {
            var pathConfigSection = new Mock<IConfigurationSection>();
            pathConfigSection.Setup(a => a.Value).Returns("c:\\");

            var searchPatternConfigSection = new Mock<IConfigurationSection>();
            searchPatternConfigSection.Setup(a => a.Value).Returns("be*");

            var maximumProcessableLineLengthConfigSection = new Mock<IConfigurationSection>();
            maximumProcessableLineLengthConfigSection.Setup(a => a.Value).Returns("43");

            var archivePathConfigSection = new Mock<IConfigurationSection>();
            archivePathConfigSection.Setup(a => a.Value).Returns("c:\\archive\\");

            var benefitOverpaymentFundCodeConfigSection = new Mock<IConfigurationSection>();
            benefitOverpaymentFundCodeConfigSection.Setup(a => a.Value).Returns("25");

            var businessRatesFundCodeConfigSection = new Mock<IConfigurationSection>();
            businessRatesFundCodeConfigSection.Setup(a => a.Value).Returns("24");

            var councilTaxFundCodeConfigSection = new Mock<IConfigurationSection>();
            councilTaxFundCodeConfigSection.Setup(a => a.Value).Returns("23");

            var housingRentsFundCodeConfigSection = new Mock<IConfigurationSection>();
            housingRentsFundCodeConfigSection.Setup(a => a.Value).Returns("05");

            var oldCouncilTaxFundCodeConfigSection = new Mock<IConfigurationSection>();
            oldCouncilTaxFundCodeConfigSection.Setup(a => a.Value).Returns("02");

            var oldNonDomesticRatesFundCodeConfigSection = new Mock<IConfigurationSection>();
            oldNonDomesticRatesFundCodeConfigSection.Setup(a => a.Value).Returns("03");

            var sapInvoicesFundCodeConfigSection = new Mock<IConfigurationSection>();
            sapInvoicesFundCodeConfigSection.Setup(a => a.Value).Returns("19");

            var suspenseFundCodeConfigSection = new Mock<IConfigurationSection>();
            suspenseFundCodeConfigSection.Setup(a => a.Value).Returns("SP");

            var mopCodePostOfficeConfigSection = new Mock<IConfigurationSection>();
            mopCodePostOfficeConfigSection.Setup(a => a.Value).Returns("15");

            var mopCodeCashAdjustmentConfigSection = new Mock<IConfigurationSection>();
            mopCodeCashAdjustmentConfigSection.Setup(a => a.Value).Returns("16");

            var mopCodeReturnedChequeConfigSection = new Mock<IConfigurationSection>();
            mopCodeReturnedChequeConfigSection.Setup(a => a.Value).Returns("17");

            var mopCodePayPointConfigSection = new Mock<IConfigurationSection>();
            mopCodePayPointConfigSection.Setup(a => a.Value).Returns("18");

            var mopCodePayzoneConfigSection = new Mock<IConfigurationSection>();
            mopCodePayzoneConfigSection.Setup(a => a.Value).Returns("19");

            var mopCodeIVRDesktopConfigSection = new Mock<IConfigurationSection>();
            mopCodeIVRDesktopConfigSection.Setup(a => a.Value).Returns("20");

            var mopCodeBillsOnlinerConfigSection = new Mock<IConfigurationSection>();
            mopCodeBillsOnlinerConfigSection.Setup(a => a.Value).Returns("21");

            var mopCodeOtherConfigSection = new Mock<IConfigurationSection>();
            mopCodeOtherConfigSection.Setup(a => a.Value).Returns("22");

            var paymentTypePostOfficeConfigSection = new Mock<IConfigurationSection>();
            paymentTypePostOfficeConfigSection.Setup(a => a.Value).Returns("P");

            var paymentTypeCashAdjustmentConfigSection = new Mock<IConfigurationSection>();
            paymentTypeCashAdjustmentConfigSection.Setup(a => a.Value).Returns("C");

            var paymentTypeReturnedChequeConfigSection = new Mock<IConfigurationSection>();
            paymentTypeReturnedChequeConfigSection.Setup(a => a.Value).Returns("Q");

            var paymentTypePayPointConfigSection = new Mock<IConfigurationSection>();
            paymentTypePayPointConfigSection.Setup(a => a.Value).Returns("T");

            var paymentTypePayzoneConfigSection = new Mock<IConfigurationSection>();
            paymentTypePayzoneConfigSection.Setup(a => a.Value).Returns("Z");

            var paymentTypeIVRDesktopConfigSection = new Mock<IConfigurationSection>();
            paymentTypeIVRDesktopConfigSection.Setup(a => a.Value).Returns("N");

            var paymentTypeBillsOnlineConfigSection = new Mock<IConfigurationSection>();
            paymentTypeBillsOnlineConfigSection.Setup(a => a.Value).Returns("B");

            var officeCodeConfigSection = new Mock<IConfigurationSection>();
            officeCodeConfigSection.Setup(a => a.Value).Returns("99");

            var suspenseVatCodeConfigSection = new Mock<IConfigurationSection>();
            suspenseVatCodeConfigSection.Setup(a => a.Value).Returns("M0");

            var sapDebtorVatCodeConfigSection = new Mock<IConfigurationSection>();
            sapDebtorVatCodeConfigSection.Setup(a => a.Value).Returns("11");

            var vatCodeConfigSection = new Mock<IConfigurationSection>();
            vatCodeConfigSection.Setup(a => a.Value).Returns("N0");

            var tansactionImportTypeIdConfigSection = new Mock<IConfigurationSection>();
            tansactionImportTypeIdConfigSection.Setup(a => a.Value).Returns("9");

            var transactionImportTypeDescriptionConfigSection = new Mock<IConfigurationSection>();
            transactionImportTypeDescriptionConfigSection.Setup(a => a.Value).Returns("AllPay Import");

            var pspReferencePrefixConfigSection = new Mock<IConfigurationSection>();
            pspReferencePrefixConfigSection.Setup(a => a.Value).Returns("AllPay");

            _mockConfiguration.Setup(x => x.GetSection("FileDetails:Path")).Returns(pathConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("FileDetails:SearchPattern")).Returns(searchPatternConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("FileDetails:MaximumProcessableLineLength")).Returns(maximumProcessableLineLengthConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("FileDetails:ArchivePath")).Returns(archivePathConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:BenefitOverpaymentFundCode")).Returns(benefitOverpaymentFundCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:BusinessRatesFundCode")).Returns(businessRatesFundCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:CouncilTaxFundCode")).Returns(councilTaxFundCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:HousingRentsFundCode")).Returns(housingRentsFundCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:OldCouncilTaxFundCode")).Returns(oldCouncilTaxFundCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:OldNonDomesticRatesFundCode")).Returns(oldNonDomesticRatesFundCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:SapInvoicesFundCode")).Returns(sapInvoicesFundCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:SuspenseFundCode")).Returns(suspenseFundCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:MopCodePostOffice")).Returns(mopCodePostOfficeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:MopCodeCashAdjustment")).Returns(mopCodeCashAdjustmentConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:MopCodeReturnedCheque")).Returns(mopCodeReturnedChequeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:MopCodePayPoint")).Returns(mopCodePayPointConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:MopCodePayzone")).Returns(mopCodePayzoneConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:MopCodeIVRDesktop")).Returns(mopCodeIVRDesktopConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:MopCodeBillsOnline")).Returns(mopCodeBillsOnlinerConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:MopCodeOther")).Returns(mopCodeOtherConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:PaymentTypePostOffice")).Returns(paymentTypePostOfficeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:PaymentTypeCashAdjustment")).Returns(paymentTypeCashAdjustmentConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:PaymentTypeReturnedCheque")).Returns(paymentTypeReturnedChequeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:PaymentTypePayPoint")).Returns(paymentTypePayPointConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:PaymentTypePayzone")).Returns(paymentTypePayzoneConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:PaymentTypeIVRDesktop")).Returns(paymentTypeIVRDesktopConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:PaymentTypeBillsOnline")).Returns(paymentTypeBillsOnlineConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:OfficeCode")).Returns(officeCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:SuspenseVatCode")).Returns(suspenseVatCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:SapDebtorVatCode")).Returns(sapDebtorVatCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:VatCode")).Returns(vatCodeConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:TransactionImportTypeId")).Returns(tansactionImportTypeIdConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:TransactionImportTypeDescription")).Returns(transactionImportTypeDescriptionConfigSection.Object);
            _mockConfiguration.Setup(x => x.GetSection("TransactionDefaultValues:PSPReferencePrefix")).Returns(pspReferencePrefixConfigSection.Object);


            _mockFileSystem.Setup(f => f.Directory.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(new[] { "c:\\Bert.pp" });
            //        _mockFileSystem.Setup(f => f.Directory.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(new[] { "c:\\Jacobs.txt", "c:\\Jacobs2.txt" });

            _mockFileSystem.Setup(x => x.Path.GetFileName(It.IsAny<string>()))
              .Returns("archiveFileName.csv");

            _mockFileSystem.Setup(x => x.File.Copy(It.IsAny<string>(), It.IsAny<string>(), true));

            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(new string[] { "A line of data" });

            _mockTransactionImportApi.Setup(x => x.TransactionImportPostAsync(It.IsAny<TransactionImportModel>(), 0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TransactionImportModel());
        }

        private void SetUpCommand()
        {
            _command = new Command() { };
        }

        [Fact]
        public async Task Handle_returns_a_ImportFileCommandResult()
        {
            // Arrange
            SetUpCommand();
            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());
            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
        }

        [Fact]
        public async Task Throws_error_when_line_length_not_long_enought()
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
            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(new string[] { " 3356107000669297      60.00T31/06/2022" });

            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.Errors[0].Should().Contain("File line item wrong length");
        }

        [Fact]
        public async Task Throws_error_when_date_on_record_is_invalid()
        {
            // Arrange
            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(new string[] { " 3356107000669297          60.00T31/06/2022" }); 

            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.Errors[0].Should().Contain("The DateTime represented by the string '31/06/2022' is not supported in calendar");
        }

        [Fact]
        public async Task Throws_error_when_amount_on_record_is_invalid()
        {
            // Arrange
            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(new string[] { " 3356107000669297         a60.00T30/06/2022" });
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.Errors[0].Should().Contain("Input string was not in a correct format");
        }

        [Fact]
        public async Task Throws_error_when_unable_to_read_file_total_amount()
        {
            // Arrange
            string[] lines = new string[4];
            lines[0] = " 3356107000669297          60.00T30/06/2022";
            lines[1] = "";
            lines[2] = "Total Amount Updated onto System:                                ";
            lines[3] = "Total Transactions updated onto System:                 4";


            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.Errors[0].Should().Contain("Input string was not in a correct format");
        }

        [Fact]
        public async Task Throws_error_when_total_line_too_short()
        {
            // Arrange
            string[] lines = new string[4];
            lines[0] = " 3356107000669297          60.00T30/06/2022";
            lines[1] = "";
            lines[2] = "Total Amount Updated onto System:                ";
            lines[3] = "Total Transactions updated onto System:                 4";


            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.Errors[0].Should().Contain("Index and length must refer to a location within the string");
        }

        [Fact]
        public async Task Throws_error_when_unable_to_read_file_total_count()
        {
            // Arrange
            string[] lines = new string[4];
            lines[0] = " 3356107000669297          60.00T30/06/2022";
            lines[1] = "";
            lines[2] = "Total Amount Updated onto System:             60.00";
            lines[3] = "Total Transactions updated onto System:";


            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.Errors[0].Should().Contain("Threw an error on line 4");
        }

        [Fact]
        public async Task Throws_error_when_file_totals_do_not_match()
        {
            // Arrange
            string[] lines = new string[3];
            lines[0] = " 3356107000669297          60.00T30/06/2022";
            lines[1] = "";
            lines[2] = "Total Amount Updated onto System:            419.00";


            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.Errors[0].Should().Contain("does not match the total calculated by the import");
        }

        [Fact]
        public async Task Throws_error_when_file_total_transaction_counts_do_not_match()
        {
            // Arrange
            string[] lines = new string[4];
            lines[0] = " 3356107000669297          60.00T30/06/2022";
            lines[1] = "";
            lines[2] = "Total Amount Updated onto System:             60.00";
            lines[3] = "Total Transactions updated onto System:                 4";


            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.Errors[0].Should().Contain("does not match the total calculated by the import of 1");
        }

        [Fact]
        public async Task Success_Is_Returned_with_multiple_records()
        {
            // Arrange
            string[] lines = new string[5];
            lines[0] = " 3356107000669297          60.00T30/06/2022";
            lines[1] = " 3356107000669297          60.00T30/06/2022";
            lines[2] = "";
            lines[3] = "Total Amount Updated onto System:            120.00";
            lines[4] = "Total Transactions updated onto System:                 2";


            _mockFileSystem.Setup(x => x.File.ReadAllLinesAsync(It.IsAny<string>(), new CancellationToken()))
                .ReturnsAsync(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(true);
            result.Errors.Count.Should().Be(0);
        }
    }
}
