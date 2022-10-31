using Application.Commands;
using System;
using System.Text.RegularExpressions;

namespace Application.Extensions
{
    public static class StringExtension
    {
        private const string CouncilTaxPattern = "^7{2}[0-9]{7}$";
        private const string BenefitOverpaymentPattern = "^7{1}[0-9]{7}$";
        private const string BusinessRatesPattern = "^56[0-9]{7}$";
        private const string OldNonDomesticRatePattern = "^5{2}[0-9]{8}$";
        private const string OldCouncilTaxPattern = "^70[0-9]{8}$";
        private const string SapInvoicePattern = "^[93]{1}[0-9]{9}$";
        private const string HousingRentsPattern = "^6{1}[0-9]{10}$";

        public static bool IsBenefitOverpayment(this string value)
        {
            return Regex.IsMatch(value, BenefitOverpaymentPattern);
        }

        public static bool IsCouncilTax(this string value)
        {
            return Regex.IsMatch(value, CouncilTaxPattern);
        }

        public static bool IsBusinessRates(this string value)
        {
            return Regex.IsMatch(value, BusinessRatesPattern);
        }

        public static bool IsOldNonDomesticRates(this string value)
        {
            return Regex.IsMatch(value, OldNonDomesticRatePattern);
        }

        public static bool IsOldCouncilTax(this string value)
        {
            return Regex.IsMatch(value, OldCouncilTaxPattern);
        }

        public static bool IsSapInvoice(this string value)
        {
            return Regex.IsMatch(value, SapInvoicePattern);
        }

        public static bool IsHousingRents(this string value)
        {
            return Regex.IsMatch(value, HousingRentsPattern);
        }

        public static string SetAccountReference(this string line)
        {
            if (line.Substring(7, 4) == "BERN")
            {
                return line.Substring(11, 12).Trim();
            }
            return line.Substring(7, 16).Trim();
        }

        public static string SetFundCode(this string accountReference, ImportFileModel importFileModel)
        {
            if (accountReference.IsHousingRents())
            {
                return importFileModel.HousingRentsFundCode;
            }
            if (accountReference.IsOldNonDomesticRates())
            {
                return importFileModel.OldNonDomesticRatesFundCode;
            }
            if (accountReference.IsSapInvoice())
            {
                return importFileModel.SapInvoicesFundCode;
            }
            if (accountReference.IsCouncilTax())
            {
                return importFileModel.CouncilTaxFundCode;
            }
            if (accountReference.IsBusinessRates())
            {
                return importFileModel.BusinessRatesFundCode;
            }
            if (accountReference.IsOldCouncilTax())
            {
                return importFileModel.OldCouncilTaxFundCode;
            }
            if (accountReference.IsBenefitOverpayment())
            {
                return importFileModel.BenefitOverpaymentFundCode;
            }
            return importFileModel.SuspenseFundCode;
        }

        public static string SetVatCode(this string fundcode, ImportFileModel importFileModel)
        {
            if (fundcode == importFileModel.SapInvoicesFundCode)
            {
                return importFileModel.SapDebtorVatCode;
            }
            if (fundcode == importFileModel.SuspenseFundCode)
            {
                return importFileModel.SuspenseVatCode;
            }
            return importFileModel.VatCode;
        }

        public static string SetMopCode(this string mopType, ImportFileModel importFileModel)
        {
            if (mopType == importFileModel.PaymentTypePostOffice)
            {
                return importFileModel.MopCodePostOffice;
            }
            if (mopType == importFileModel.PaymentTypeCashAdjustment)
            {
                return importFileModel.MopCodeCashAdjustment;
            }
            if (mopType == importFileModel.PaymentTypeReturnedCheque)
            {
                return importFileModel.MopCodeReturnedCheque;
            }
            if (mopType == importFileModel.PaymentTypePayPoint)
            {
                return importFileModel.MopCodePayPoint;
            }
            if (mopType == importFileModel.PaymentTypePayzone)
            {
                return importFileModel.MopCodePayzone;
            }
            if (mopType == importFileModel.PaymentTypeIVRDesktop)
            {
                return importFileModel.MopCodeIVRDesktop;
            }
            if (mopType == importFileModel.PaymentTypeBillsOnline)
            {
                return importFileModel.MopCodeBillsOnline;
            }
            return importFileModel.MopCodeOther;
        }

        public static bool PotentialDetailLine(this string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                return line.Substring(0, 1) == " ";
            }
            return false;
        }

         public static bool TotalAmount(this string line)
        {
            return line.StartsWith("Total Amount");
        }

        public static bool TotalTransactionCount(this string line)
        {
            return line.StartsWith("Total Transactions");
        }

        public static decimal FileTotalAmount(this string line)
        {
            var fileTotalAmount = Convert.ToDecimal(line.Substring(34, 17).Trim());
            return fileTotalAmount ;
        }

        public static int FileTotalTransactionCount(this string line)
        {
            var fileTotalTransactionCount = Convert.ToInt16(line.Substring(40, 17).Trim());
            return fileTotalTransactionCount;
        }
    }
}
