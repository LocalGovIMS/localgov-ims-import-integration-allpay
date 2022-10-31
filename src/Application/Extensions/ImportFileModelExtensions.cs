using Application.Commands;
using Microsoft.Extensions.Configuration;

namespace Application.Extensions
{
    public static class ImportFileModelExtensions
    {
        public static void GetConfigValues(this ImportFileModel model, IConfiguration configuration)
        {
            model.BenefitOverpaymentFundCode = configuration.GetValue<string>("TransactionDefaultValues:BenefitOverpaymentFundCode");
            model.BusinessRatesFundCode = configuration.GetValue<string>("TransactionDefaultValues:BusinessRatesFundCode");
            model.CouncilTaxFundCode = configuration.GetValue<string>("TransactionDefaultValues:CouncilTaxFundCode");
            model.HousingRentsFundCode = configuration.GetValue<string>("TransactionDefaultValues:HousingRentsFundCode");
            model.OldCouncilTaxFundCode = configuration.GetValue<string>("TransactionDefaultValues:OldCouncilTaxFundCode");
            model.OldNonDomesticRatesFundCode = configuration.GetValue<string>("TransactionDefaultValues:OldNonDomesticRatesFundCode");
            model.SapInvoicesFundCode = configuration.GetValue<string>("TransactionDefaultValues:SapInvoicesFundCode");
            model.SuspenseFundCode = configuration.GetValue<string>("TransactionDefaultValues:SuspenseFundCode");
            model.MopCodePostOffice = configuration.GetValue<string>("TransactionDefaultValues:MopCodePostOffice");
            model.MopCodeCashAdjustment = configuration.GetValue<string>("TransactionDefaultValues:MopCodeCashAdjustment");
            model.MopCodeReturnedCheque = configuration.GetValue<string>("TransactionDefaultValues:MopCodeReturnedCheque");
            model.MopCodePayPoint = configuration.GetValue<string>("TransactionDefaultValues:MopCodePayPoint");
            model.MopCodePayzone = configuration.GetValue<string>("TransactionDefaultValues:MopCodePayzone");
            model.MopCodeIVRDesktop = configuration.GetValue<string>("TransactionDefaultValues:MopCodeIVRDesktop");
            model.MopCodeBillsOnline = configuration.GetValue<string>("TransactionDefaultValues:MopCodeBillsOnline");
            model.MopCodeOther = configuration.GetValue<string>("TransactionDefaultValues:MopCodeOther");
            model.PaymentTypePostOffice = configuration.GetValue<string>("TransactionDefaultValues:PaymentTypePostOffice");
            model.PaymentTypeCashAdjustment = configuration.GetValue<string>("TransactionDefaultValues:PaymentTypeCashAdjustment");
            model.PaymentTypeReturnedCheque = configuration.GetValue<string>("TransactionDefaultValues:PaymentTypeReturnedCheque");
            model.PaymentTypePayPoint = configuration.GetValue<string>("TransactionDefaultValues:PaymentTypePayPoint");
            model.PaymentTypePayzone = configuration.GetValue<string>("TransactionDefaultValues:PaymentTypePayzone");
            model.PaymentTypeIVRDesktop = configuration.GetValue<string>("TransactionDefaultValues:PaymentTypeIVRDesktop");
            model.PaymentTypeBillsOnline = configuration.GetValue<string>("TransactionDefaultValues:PaymentTypeBillsOnline");
            model.OfficeCode = configuration.GetValue<string>("TransactionDefaultValues:OfficeCode");
            model.SuspenseVatCode = configuration.GetValue<string>("TransactionDefaultValues:SuspenseVatCode");
            model.SapDebtorVatCode = configuration.GetValue<string>("TransactionDefaultValues:SapDebtorVatCode");
            model.VatCode = configuration.GetValue<string>("TransactionDefaultValues:VatCode");
            model.TransactionImportTypeId = configuration.GetValue<int>("TransactionDefaultValues:TransactionImportTypeId");
            model.TransactionImportTypeDescription = configuration.GetValue<string>("TransactionDefaultValues:TransactionImportTypeDescription");
            model.PSPReferencePrefix = configuration.GetValue<string>("TransactionDefaultValues:PSPReferencePrefix");
            model.Path = configuration.GetValue<string>("FileDetails:Path");
            model.SearchPattern = configuration.GetValue<string>("FileDetails:SearchPattern");
            model.MaximumProcessableLineLength = configuration.GetValue<int>("FileDetails:MaximumProcessableLineLength");
            model.ArchivePath = configuration.GetValue<string>("FileDetails:ArchivePath");
        }
    }
}
