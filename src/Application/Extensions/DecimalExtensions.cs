using LocalGovImsApiClient.Model;
using System;
using System.Linq;

namespace Application.Extensions
{
    public static class DecimalExtensions
    {
        public static bool CheckFileTotalsAreCorrect(this decimal fileTotalAmount, int fileTotalTransactionCount, TransactionImportModel transactionImportModel)
        {
            if (fileTotalAmount != transactionImportModel.Rows.Sum(x => x.Amount))
            {
                throw new InvalidOperationException("The files Total Amount of " + fileTotalAmount + " does not match the total calculated by the import of " + transactionImportModel.Rows.Sum(x => x.Amount));
            }
            var numberOfRows = transactionImportModel.Rows?.Count ?? 0;
            if (fileTotalTransactionCount != numberOfRows)
            {
                throw new InvalidOperationException("The files Total count of " + fileTotalTransactionCount + " does not match the total calculated by the import of " + numberOfRows);
            }
            return true;
        }
    }
}
