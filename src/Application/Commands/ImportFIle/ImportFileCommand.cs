using Microsoft.Extensions.Configuration;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using LocalGovImsApiClient.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
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

        private ImportFileCommandResult _result = new();
        private ImportFileModel _importFileModel = new();
        private TransactionImportModel _transactionImportModel = new();
        private IEnumerable<string> _files;
        private decimal _fileTotalAmount;
        private int _fileTotalTransactionCount;

        public ImportFileCommandHandler(
            IConfiguration configuration,
            IFileSystem file,
            LocalGovImsApiClient.Api.ITransactionImportApi transactionImportApi)
        {
            _configuration = configuration;
            _fileSystem = file;
            _transactionImportApi = transactionImportApi;
        }

        public async Task<ImportFileCommandResult> Handle(ImportFileCommand request, CancellationToken cancellationToken)
        {
            GetTransactionDefaultValues();

            GetFileConfiDetails();

            GetFiles();

            await ProcessFiles(cancellationToken);

            //TODO: how is file moved so it isnt processed again

            SetUpResult();

            return _result;
        }

        private void GetTransactionDefaultValues()
        {
            _importFileModel.BenefitOverpaymentFundCode = _configuration.GetValue<string>("TransactionDefaultValues:BenefitOverpaymentFundCode");
            _importFileModel.BusinessRatesFundCode = _configuration.GetValue<string>("TransactionDefaultValues:BusinessRatesFundCode");
            _importFileModel.CouncilTaxFundCode = _configuration.GetValue<string>("TransactionDefaultValues:CouncilTaxFundCode");
            _importFileModel.HousingRentsFundCode = _configuration.GetValue<string>("TransactionDefaultValues:HousingRentsFundCode");
            _importFileModel.OldCouncilTaxFundCode = _configuration.GetValue<string>("TransactionDefaultValues:OldCouncilTaxFundCode");
            _importFileModel.OldNonDomesticRatesFundCode = _configuration.GetValue<string>("TransactionDefaultValues:OldNonDomesticRatesFundCode");
            _importFileModel.SapInvoicesFundCode = _configuration.GetValue<string>("TransactionDefaultValues:SapInvoicesFundCode");
            _importFileModel.SuspenseFundCode = _configuration.GetValue<string>("TransactionDefaultValues:SuspenseFundCode");
            _importFileModel.MopCodePostOffice = _configuration.GetValue<string>("TransactionDefaultValues:MopCodePostOffice");
            _importFileModel.MopCodeCashAdjustment = _configuration.GetValue<string>("TransactionDefaultValues:MopCodeCashAdjustment");
            _importFileModel.MopCodeReturnedCheque = _configuration.GetValue<string>("TransactionDefaultValues:MopCodeReturnedCheque");
            _importFileModel.MopCodePayPoint = _configuration.GetValue<string>("TransactionDefaultValues:MopCodePayPoint");
            _importFileModel.MopCodePayzone = _configuration.GetValue<string>("TransactionDefaultValues:MopCodePayzone");
            _importFileModel.MopCodeIVRDesktop = _configuration.GetValue<string>("TransactionDefaultValues:MopCodeIVRDesktop");
            _importFileModel.MopCodeBillsOnline = _configuration.GetValue<string>("TransactionDefaultValues:MopCodeBillsOnline");
            _importFileModel.MopCodeOther = _configuration.GetValue<string>("TransactionDefaultValues:MopCodeOther");
            _importFileModel.PaymentTypePostOffice = _configuration.GetValue<string>("TransactionDefaultValues:PaymentTypePostOffice");
            _importFileModel.PaymentTypeCashAdjustment = _configuration.GetValue<string>("TransactionDefaultValues:PaymentTypeCashAdjustment");
            _importFileModel.PaymentTypeReturnedCheque = _configuration.GetValue<string>("TransactionDefaultValues:PaymentTypeReturnedCheque");
            _importFileModel.PaymentTypePayPoint = _configuration.GetValue<string>("TransactionDefaultValues:PaymentTypePayPoint");
            _importFileModel.PaymentTypePayzone = _configuration.GetValue<string>("TransactionDefaultValues:PaymentTypePayzone");
            _importFileModel.PaymentTypeIVRDesktop = _configuration.GetValue<string>("TransactionDefaultValues:PaymentTypeIVRDesktop");
            _importFileModel.PaymentTypeBillsOnline = _configuration.GetValue<string>("TransactionDefaultValues:PaymentTypeBillsOnline");
            _importFileModel.OfficeCode = _configuration.GetValue<string>("TransactionDefaultValues:OfficeCode");
            _importFileModel.SuspenseVatCode = _configuration.GetValue<string>("TransactionDefaultValues:SuspenseVatCode");
            _importFileModel.SapDebtorVatCode = _configuration.GetValue<string>("TransactionDefaultValues:SapDebtorVatCode");
            _importFileModel.VatCode = _configuration.GetValue<string>("TransactionDefaultValues:VatCode");
            _importFileModel.TransactionImportTypeId = _configuration.GetValue<int>("TransactionDefaultValues:TransactionImportTypeId");
            _importFileModel.TransactionImportTypeDescription = _configuration.GetValue<string>("TransactionDefaultValues:TransactionImportTypeDescription");
            _importFileModel.PSPReferencePrefix = _configuration.GetValue<string>("TransactionDefaultValues:PSPReferencePrefix");
        }

        private void GetFileConfiDetails()
        {
            _importFileModel.Path = _configuration.GetValue<string>("FileDetails:Path");
            _importFileModel.SearchPattern = _configuration.GetValue<string>("FileDetails:SearchPattern");
            _importFileModel.MaximumProcessableLineLength = _configuration.GetValue<int>("FileDetails:MaximumProcessableLineLength");
        }

        private void GetFiles()
        {
            _files = _fileSystem.Directory.GetFiles(_importFileModel.Path, _importFileModel.SearchPattern);
        }

        private async Task ProcessFiles(CancellationToken cancellationToken)
        {
            Prepare();

            // TODO: need to decide if processing more than one file at a time
            foreach (var file in _files)
            {
                await ReadFile(file, cancellationToken);

                await PostFileAsync();
            }
            //TODO: how is file moved so it isnt processed again
        }

        private void Prepare()
        {
            _transactionImportModel.Initialise();
        }

        private async Task ReadFile(string file, CancellationToken cancellationToken)
        {
            //TODO: are we loading multiple files or just one at a time?
            int lineCount = 0;
            bool endOfFile = false;
            var _lines = await _fileSystem.File.ReadAllLinesAsync(file, cancellationToken);
            foreach (var line in _lines)
            {
                var transaction = new ProcessedTransactionModel();
                transaction.SetStaticValues(_importFileModel);
                try
                {
                    lineCount++;
                    if (endOfFile)
                    {
                        ProcessFileTotalRecords(line);
                        continue;
                    }
                    if (line.Length < 2) {
                        endOfFile = true;
                        continue;
                    }
                    if (line.Length < _importFileModel.MaximumProcessableLineLength) { throw new Exception("File line item wrong length"); }
                    transaction.PspReference = _importFileModel.PSPReferencePrefix + DateTime.Now.ToString("yyMMddhhmm") + lineCount;
                    transaction.TransactionDate = DateTime.ParseExact(line.Substring(33, 10).ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    transaction.AccountReference = SetAccountReference(line);
                    transaction.FundCode = SetFundCode(transaction.AccountReference);
                    transaction.VatCode = SetVatCode(transaction.FundCode);
                    transaction.MopCode = SetMopCode(line.Substring(32, 1));
                    var amount = line.Substring(23, 9).Trim();
                    transaction.Amount = decimal.Parse(amount);
                    transaction.Narrative = line.Substring(0, 7).Trim();
                    _transactionImportModel.Rows.Add(transaction);
                }
                catch (Exception exception)
                {
                    _transactionImportModel.Errors.Add("Threw an error on line " + lineCount + " - " + exception.Message);
                }
            }
            CheckFileTotalsAreCorrect();
        }

        private async Task PostFileAsync()
        {
            _transactionImportModel.ImportTypeId = _importFileModel.TransactionImportTypeId;
            _transactionImportModel.NumberOfRows = _transactionImportModel.Rows?.Count() ?? 0;
            try
            {
                var result = await _transactionImportApi.TransactionImportPostAsync(_transactionImportModel);
                if (result == null)
                {
                    throw new Exception("IMSApi not found to post the transactions");
                }
            }
            catch (Exception exception)
            {
                _transactionImportModel.Errors.Add(exception.Message);
            }
        }

        private string SetAccountReference(string line)
        {
            if (line.Substring(7, 4) == "BERN")
            {
                return line.Substring(11, 12).Trim();
            }
            else
            {
                return line.Substring(7, 16).Trim();
            }
        }

        private string SetFundCode(string accountReference)
        {
            if (accountReference.IsHousingRents())
            {
                return _importFileModel.HousingRentsFundCode;
            }
            else if (accountReference.IsOldNonDomesticRates())
            {
                return _importFileModel.OldNonDomesticRatesFundCode;
            }
            else if (accountReference.IsSapInvoice())
            {
                return _importFileModel.SapInvoicesFundCode;
            }
            else if (accountReference.IsCouncilTax())
            {
                return _importFileModel.CouncilTaxFundCode;
            }
            else if (accountReference.IsBusinessRates())
            {
                return  _importFileModel.BusinessRatesFundCode;
            }
            else if (accountReference.IsOldCouncilTax())
            {
                return _importFileModel.OldCouncilTaxFundCode;
            }
            else if (accountReference.IsBenefitOverpayment())
            {
                return _importFileModel.BenefitOverpaymentFundCode;
            }
            return _importFileModel.SuspenseFundCode;
        }

        private string SetVatCode(string fundcode)
        {
            if (fundcode == _importFileModel.SapInvoicesFundCode)
            {
                return _importFileModel.SapDebtorVatCode;
            }
            else if (fundcode == _importFileModel.SuspenseFundCode)
            {
                return _importFileModel.SuspenseVatCode;
            }
            return _importFileModel.VatCode;
        }

        private string SetMopCode(string mopType)
        {
            if (mopType == _importFileModel.PaymentTypePostOffice)
            {
                return _importFileModel.MopCodePostOffice;
            }
            else if (mopType == _importFileModel.PaymentTypeCashAdjustment)
            {
                return  _importFileModel.MopCodeCashAdjustment;
            }
            else if (mopType == _importFileModel.PaymentTypeReturnedCheque)
            {
                return _importFileModel.MopCodeReturnedCheque;
            }
            else if (mopType == _importFileModel.PaymentTypePayPoint)
            {
                return _importFileModel.MopCodePayPoint;
            }
            else if (mopType == _importFileModel.PaymentTypePayzone)
            {
                return _importFileModel.MopCodePayzone;
            }
            else if (mopType == _importFileModel.PaymentTypeIVRDesktop)
            {
                return _importFileModel.MopCodeIVRDesktop;
            }
            else if (mopType == _importFileModel.PaymentTypeBillsOnline)
            {
                return _importFileModel.MopCodeBillsOnline;
            }
            return _importFileModel.MopCodeOther;
        }
        
        private  void ProcessFileTotalRecords(string line)
        {
            if (line.StartsWith("Total Amount"))
            {
                try 
                {
                    var fileTotalAmount = line.Substring(34, 17).Trim();
                    _fileTotalAmount = Convert.ToDecimal(fileTotalAmount);
                }
                catch(Exception e)
                {
                    _transactionImportModel.Errors.Add("Unable to read/convert Total Amount value on this file - " + e.Message);
                }
            }
            if (line.StartsWith("Total Transactions"))
            {
                try
                {
                    var fileTotalTransactions = line.Substring(40, 17).Trim();
                    _fileTotalTransactionCount = Convert.ToInt16(fileTotalTransactions);
                }
                catch (Exception e)
                {
                    _transactionImportModel.Errors.Add("Unable to read/convert Total Transactions count on this file - " + e.Message);
                }
            }
        }

        private  void CheckFileTotalsAreCorrect()
        {
            if (_fileTotalAmount != _transactionImportModel.Rows.Sum(x => x.Amount))
            {
                _transactionImportModel.Errors.Add("The files Total Amount of " + _fileTotalAmount + " does not match the total calculated by the import of " + _transactionImportModel.Rows.Sum(x => x.Amount));
            }
            int numberOfRows = _transactionImportModel.Rows?.Count() ?? 0;
            if (_fileTotalTransactionCount != numberOfRows)
            {
                _transactionImportModel.Errors.Add("The files Total Transaction count of " + _fileTotalTransactionCount + " does not match the total calculated by the import of " + numberOfRows);
            }
        }

        private void SetUpResult()
        {
            _result.Errors = _transactionImportModel.Errors;
        }
    }
}
