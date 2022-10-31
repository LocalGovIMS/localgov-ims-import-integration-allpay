using Application.Commands;
using LocalGovImsApiClient.Model;
using System;
using System.Globalization;

namespace Application.Extensions
{
    public static class ProcessedTransactionExtensions
    {
        public static bool PopulateFields(this ProcessedTransactionModel item, ImportFileModel importFileModel, string line, int lineCount, TransactionImportModel transactionImportModel)
        {
            if (line.Length < 2)
            { 
                return true;
            }
            if (line.Length < importFileModel.MaximumProcessableLineLength)
            {
                throw new InvalidOperationException("File line item wrong length");
            }
 
            item.OfficeCode = importFileModel.OfficeCode;
            item.EntryDate = DateTime.Now;
            item.VatAmount = 0;
            item.VatRate = 0;
            item.UserCode = 0;
            item.PspReference = importFileModel.PSPReferencePrefix + DateTime.Now.ToString("yyMMddhhmm") + lineCount;
            item.TransactionDate = DateTime.ParseExact(line.Substring(33, 10).ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            item.AccountReference = line.SetAccountReference();
            item.FundCode = item.AccountReference.SetFundCode(importFileModel);
            item.VatCode = item.FundCode.SetVatCode(importFileModel);
            item.MopCode = line.Substring(32, 1).SetMopCode(importFileModel);
            var amount = line.Substring(23, 9).Trim();
            item.Amount = decimal.Parse(amount);
            item.Narrative = line.Substring(0, 7).Trim();
            transactionImportModel.Rows.Add(item);
            return false;
        }
    }
}