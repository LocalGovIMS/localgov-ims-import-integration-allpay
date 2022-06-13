using Microsoft.Extensions.Configuration;
using System.IO.Abstractions;
using Moq;
using Application.Commands;
using Command = Application.Commands.ImportFileCommand;
using Handler = Application.Commands.ImportFileCommandHandler;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;

namespace Application.UnitTests.Commands
{
    public class HandleTests
    {
        private const string FileLocation = "c:\\temp\\allpayfile.txt";

        
        private readonly Handler _commandHandler;
        private Command _command;


        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();
        private readonly Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();


        public HandleTests()
        {
            _commandHandler = new Handler(
                _mockConfiguration.Object,
                _mockFileSystem.Object);

            SetUpConfig();
            SetUpCommand();


        }

        private void SetUpConfig()
        {
            var fileLoationConfigSection = new Mock<IConfigurationSection>();
            fileLoationConfigSection.Setup(a => a.Value).Returns(FileLocation);

            _mockConfiguration.Setup(x => x.GetSection("FileDetails:Location")).Returns(fileLoationConfigSection.Object);
            _mockFileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>()))
                .Returns(new string[] { "A line of data" });
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
            var result = await _commandHandler.Handle(_command, new System.Threading.CancellationToken());
            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
        }

        [Fact]
        public async Task Throws_error_when_line_length_not_long_enought()
        {
            // Arrange
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.ErrorString[0].Should().Contain("File line item wrong length");
        }

        [Fact]
        public async Task Throws_error_when_date_on_record_is_invalid()
        {
            // Arrange
            _mockFileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>()))
                .Returns(new string[] { " 3356107000669297          60.00T31/06/2022" }); 
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.ErrorString[0].Should().Contain("The DateTime represented by the string '31/06/2022' is not supported in calendar");
        }

        [Fact]
        public async Task Throws_error_when_amount_on_record_is_invalid()
        {
            // Arrange
            _mockFileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>()))
                .Returns(new string[] { " 3356107000669297         a60.00T30/06/2022" });
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.ErrorString[0].Should().Contain("Input string was not in a correct format");
        }

        [Fact]
        public async Task Throws_error_when_unable_to_read_file_total_amount()
        {
            // Arrange
            string[] lines = new string[4];
            lines[0] = " 3356107000669297          60.00T30/06/2022";
            lines[1] = "";
            lines[2] = "Total Amount Updated onto System:            ";
            lines[3] = "Total Transactions updated onto System:                 4";


            _mockFileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>()))
                .Returns(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.ErrorString[0].Should().Contain("Unable to read/convert Total Amount value on the file");
        }

        [Fact]
        public async Task Throws_error_when_unable_to_read_file_total_count()
        {
            // Arrange
            string[] lines = new string[4];
            lines[0] = " 3356107000669297          60.00T30/06/2022";
            lines[1] = "";
            lines[2] = "Total Amount Updated onto System:            419.00";
            lines[3] = "Total Transactions updated onto System:";


            _mockFileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>()))
                .Returns(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.ErrorString[0].Should().Contain("Unable to read/convert Total Transactions count on the file");
        }

        [Fact]
        public async Task Throws_error_when_file_totals_do_not_match()
        {
            // Arrange
            string[] lines = new string[3];
            lines[0] = " 3356107000669297          60.00T30/06/2022";
            lines[1] = "";
            lines[2] = "Total Amount Updated onto System:            419.00";


            _mockFileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>()))
                .Returns(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.ErrorString[0].Should().Contain("does not match the total calculated by the import");
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


            _mockFileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>()))
                .Returns(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(false);
            result.ErrorString[0].Should().Contain("does not match the total calculated by the import of 1");
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


            _mockFileSystem.Setup(x => x.File.ReadAllLines(It.IsAny<string>()))
                .Returns(lines);
            SetUpCommand();

            //Act
            var result = await _commandHandler.Handle(_command, new System.Threading.CancellationToken());

            //Assert
            result.Should().BeOfType<ImportFileCommandResult>();
            result.Success.Should().Be(true);
            result.ErrorString.Count.Should().Be(0);
            result.FilesProcessed.Should().Be(1);
        }
    }
}
