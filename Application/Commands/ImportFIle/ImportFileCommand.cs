using Microsoft.Extensions.Configuration;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using LocalGovImsApiClient.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace Application.Commands
{
    public class ImportFileCommand : IRequest<ImportFileCommandResult>
    {

    }

    public class ImportFileCommandHandler : IRequestHandler<ImportFileCommand, ImportFileCommandResult>
    {
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;

        private ImportFileCommandResult _result = new ImportFileCommandResult();
        private string _fileLocation;
        private string[] _lines;
        private List<ProcessedTransactionModel> _transactions = new List<ProcessedTransactionModel>();
        private Random _randomGenerator = new Random();
        private decimal _fileTotalAmount;
        private int _fileTotalTransactionCount;

        public ImportFileCommandHandler(
            IConfiguration configuration,
            IFileSystem file)
        {
            _configuration = configuration;
            _fileSystem = file;
        }

        public async Task<ImportFileCommandResult> Handle(ImportFileCommand request, CancellationToken cancellationToken)
        {
            GetFileLocation();

            ReadFile();

            if (_result.ErrorsDetected == 0)
            {
                PostFile();
            }

            //TODO: how is file moved so it isnt processed again

            return _result;
        }

        private void GetFileLocation()
        {
            //TODO: is there any validation needed on checking have the right file eg checking name? extension?
            _fileLocation = _configuration.GetValue<string>("FileDetails:Location");

        }
        private void ReadFile()
        {
            //TODO: are we loading multiple files or just one at a time?
            int lineCount = 0;
            decimal fileTotal = 0;
            bool endOfFile = false;
            _result.FilesProcessed++;
            _lines = _fileSystem.File.ReadAllLines(_fileLocation);
            foreach (var line in _lines)
            {
                var transaction = new ProcessedTransactionModel() { 
                    OfficeCode = "99",
                    EntryDate = DateTime.Now,
                    VatAmount = 0,
                    VatRate = 0,
                    UserCode = 0
                };
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
                    if (line.Length < 43) { throw new Exception("File line item wrong length"); }
            //        transaction.Reference = lineCount + GetNextReferenceId(_randomGenerator);
                    transaction.InternalReference = transaction.Reference;
                    transaction.PspReference = transaction.Reference;
                    transaction.TransactionDate = DateTime.ParseExact(line.Substring(33, 10).ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    transaction.AccountReference = SetAccountReference(line);

                    transaction.FundCode = SetFundCode(transaction.AccountReference);

                    transaction.VatCode = SetVatCode(transaction.FundCode);

                    transaction.MopCode = SetMopCode(line.Substring(32, 1));


                    var amount = line.Substring(23, 9).Trim();
                    transaction.Amount = decimal.Parse(amount);
                    transaction.Narrative = line.Substring(0, 7).Trim();
                    //       transaction.BatchReference = batchref;  //TODO: is batch reference going

                    _transactions.Add(transaction);
                    fileTotal += transaction.Amount.Value;
                }
                catch (Exception exception)
                {
                    _result.ErrorString.Add("Threw an error on line " + lineCount + " - " + exception.Message);
                    _result.ErrorsDetected++;
                }
            }

            CheckFileTotalsAreCorrect(fileTotal);
        }

        private void PostFile()
        {
            _result.Success = true;
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
            if (accountReference.StartsWith("6"))
            {
                return "05";
            }
            else if (accountReference.StartsWith("55"))
            {
                return "03";
            }
            else if (accountReference.StartsWith("95"))
            {
                return "19";
            }
            else if (accountReference.StartsWith("77") && accountReference.All(char.IsDigit))
            {
                return "23";
            }
            else if (accountReference.StartsWith("56") && accountReference.All(char.IsDigit))
            {
                return "24";
            }
            else if (accountReference.StartsWith("70") && accountReference.Length > 8 && accountReference.All(char.IsDigit))
            {
                return "02";
            }
            else if (accountReference.StartsWith("70") && accountReference.All(char.IsDigit))
            {
                return "25";
            }
            else
            {
                return "SP";
            }
        }

        private string SetVatCode(string fundcode)
        {
            if (fundcode == "19")
            {
                return "11";
            }
            else if (fundcode == "SP")
            {
                return "M0";
            }
            else { return "N0"; } 
        }

        private string SetMopCode(string mopType)
        {
            if (mopType == "P")
            {
                return "15";
            }
            else if (mopType == "C")
            {
                return "16";
            }
            else if (mopType == "Q")
            {
                return "17";
            }
            else if (mopType == "T")
            {
                return "18";
            }
            else if (mopType == "Z")
            {
                return "19";
            }
            else if (mopType == "N")
            {
                return "20";
            }
            else if (mopType == "B")
            {
                return "21";
            }
            else
            {
                return "22";
            }
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
                    _result.ErrorString.Add("Unable to read/convert Total Amount value on the file - " + e.Message);
                    _result.ErrorsDetected++;
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
                    _result.ErrorString.Add("Unable to read/convert Total Transactions count on the file - " + e.Message);
                    _result.ErrorsDetected++;
                }
            }
        }

        private  void CheckFileTotalsAreCorrect(decimal fileTotal)
        {
            if (_fileTotalAmount != fileTotal ) 
            {
                _result .ErrorString.Add("The files Total Amount of " + _fileTotalAmount + " does not match the total calculated by the import of " + fileTotal);
                _result.ErrorsDetected++;
            }
            if (_fileTotalTransactionCount != _transactions.Count)
            {
                _result.ErrorString.Add("The files Total Transaction count of " + _fileTotalTransactionCount + " does not match the total calculated by the import of " + _transactions.Count);
                _result.ErrorsDetected++;
            }
        }

        //internal static string GetNextReferenceId(Random randomGenerator)
        //{
        //   var hash = new Hashids("BMBC", 9);
        //    return hash.Encode(DateTime.Now.Millisecond, randomGenerator.Next(999999));
        //}

    }
}
