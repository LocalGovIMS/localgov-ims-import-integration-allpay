using Microsoft.Extensions.Configuration;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using LocalGovImsApiClient.Model;
using System;
using System.Linq;
using System.IO.Abstractions;
using Application.Extensions;

namespace Application.Commands
{
    public class ImportFileCommand : IRequest<ImportFileCommandResult>
    {

    }

    public class ImportFileCommandHandler : IRequestHandler<ImportFileCommand, ImportFileCommandResult>
    {
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;
        private readonly LocalGovImsApiClient.Api.ITransactionImportApi _transactionImportApi;

        private readonly ImportFileCommandResult _result = new();
        private readonly ImportFileModel _importFileModel = new();
        private readonly TransactionImportModel _transactionImportModel = new();
        private string file;
        private decimal _fileTotalAmount;
        private int _fileTotalTransactionCount;

        public ImportFileCommandHandler(
            IConfiguration configuration,
            IFileSystem fileSystem,
            LocalGovImsApiClient.Api.ITransactionImportApi transactionImportApi)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
            _transactionImportApi = transactionImportApi;
        }

        public async Task<ImportFileCommandResult> Handle(ImportFileCommand request, CancellationToken cancellationToken)
        {
            GetConfigurationDetails();

            GetFile();

            try
            {
                await ProcessFile(cancellationToken);
            }
            catch (Exception exception)
            {
                _result.Errors.Add("Error processing file, " + exception.Message);
            }

            return _result;
        }
        private void GetConfigurationDetails()
        {
            _importFileModel.GetConfigValues(_configuration);
        }

        private void GetFile()
        {
            var _files = _fileSystem.Directory.GetFiles(_importFileModel.Path, _importFileModel.SearchPattern);
            file = _files.FirstOrDefault();
        }

        private async Task ProcessFile(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(file))
            {
                Prepare();

                await ReadFile(file, cancellationToken);

                DeleteFile();

                if (_result.Success)
                {
                    await PostFileAsync();
                }
            }
        }

        private void Prepare()
        {
            _transactionImportModel.Initialise(_importFileModel);
            var archiveFileName = _importFileModel.ArchivePath + DateTime.Now.ToString("yyyyMMddhhmm") + _fileSystem.Path.GetFileName(file);
            _fileSystem.File.Copy(file, archiveFileName, true);
        }

        private async Task ReadFile(string file, CancellationToken cancellationToken)
        {
            int lineCount = 0;
            try
            {
                var _lines = await _fileSystem.File.ReadAllLinesAsync(file, cancellationToken);
                foreach (var line in _lines)
                {
                    lineCount++;
                    if (line.PotentialDetailLine())
                    {
                        var transaction = new ProcessedTransactionModel();
                        transaction.PopulateFields(_importFileModel, line, lineCount, _transactionImportModel);
                    }
                    else if(line.TotalAmount())
                    {
                        _fileTotalAmount = line.FileTotalAmount();
                    }
                    else if(line.TotalTransactionCount())
                    {
                        _fileTotalTransactionCount = line.FileTotalTransactionCount();
                    }
                }
                var result = _fileTotalAmount.CheckFileTotalsAreCorrect(_fileTotalTransactionCount, _transactionImportModel);
            }
            catch (Exception exception)
            {
                _result.Errors.Add("Threw an error on line " + lineCount + " - " + exception.Message);
            }
        }

        private void DeleteFile()
        {
            try
            {
                _fileSystem.File.Delete(file);
            }
            catch (Exception exception)
            {
                _result.Errors.Add("Error deleting the file " + exception.Message);
            }
        }

        private async Task PostFileAsync()
        {
            _transactionImportModel.NumberOfRows = _transactionImportModel.Rows?.Count() ?? 0;
            try
            {
                var result = await _transactionImportApi.TransactionImportPostAsync(_transactionImportModel);
                if (result == null)
                {
                    throw new InvalidOperationException("IMSApi not found to post the transactions");
                }
            }
            catch (Exception exception)
            {
                _result.Errors = _transactionImportModel.Errors;
                _result.Errors.Add(exception.Message);
            }
        }
    }
}
