using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using company.Common;
using erp.DAL.EF.New;

namespace backend.Properties
{
    public partial class Settings : ISettings
    {
        public int CreditNote_Create_InvoiceFromId_NonUK
        {
            get { return Configuration.GetIntValue("CreditNote_Create_InvoiceFromId_NonUK"); }
        }

        public int CreditNote_Create_InvoiceFromId
        {
            get { return Configuration.GetIntValue("CreditNote_Create_InvoiceFromId"); }
        }

        public string Analytics_IncludedNonDistributors
        {
            get { return Configuration.GetValue("Analytics_IncludedNonDistributors"); }
        }

        public string CreditNote_Create_UKDistributors_Exceptions
        {
            get { return Configuration.GetValue("CreditNote_Create_UKDistributors_Exceptions"); }
        }

        public string Analytics_ForBrands_ExcludedCustomers
        {
            get { return Configuration.GetValue("Analytics_ForBrands_ExcludedCustomers"); }
        }


        public string BBSInvoices_ExcludedNonUKCustomers
        {
            get { return Configuration.GetValue("BBSInvoices_ExcludedNonUKCustomers"); }
        }

        public List<int> PendingInvoicesException_Companies
        {
            get
            {
                var idstring = Configuration.GetValue("PendingInvoicesException_Companies");
                return Utilities.GetIdsFromString(idstring);
            }
        }

        public List<int> OMExport_UseridsForExtraValueFields
        {
            get
            {
                var idstring = Configuration.GetValue("OMExport_UseridsForExtraValueFields");
                return Utilities.GetIdsFromString(idstring);
            }
        }

        public string TrackingNumbersMail_Subject
        {
            get { return Configuration.GetValue("TrackingNumbersMail_Subject"); }
        }

        public string TrackingNumbersMail__To
        {
            get { return Configuration.GetValue("TrackingNumbersMail__To"); }
        }

        public string TrackingNumbersMail__Cc
        {
            get { return Configuration.GetValue("TrackingNumbersMail__Cc"); }
        }

        public string TrackingNumbersMail__Bcc
        {
            get { return Configuration.GetValue("TrackingNumbersMail__Bcc"); }
        }

        public string OMExport_CompaniesUsingBookedInDate
        {
            get
            {
                return Configuration.GetValue("OMExport_CompaniesUsingBookedInDate");
            }
        }

        public DateTime? Invoice_ComponentCalculation_Start
        {
            get
            {
                var sDate = Configuration.GetValue("Invoice_ComponentCalculation_Start");
                DateTime result;
                if (DateTime.TryParse(sDate, out result))
                    return result;
                return null;
            }
        }

        public string OMExport_ClientsUsingSalesOrders
        {
            get
            {
                return Configuration.GetValue("OMExport_ClientsUsingSalesOrders");
            }
        }

        public List<int> UsSales_CompanyIds
        {
            get
            {
                return Utilities.GetIdsFromString(Configuration.GetValue("UsSales_CompanyIds"));
            }
            
        }

        public List<string> UsSales_Warehouses
        {
            get
            {
                return Configuration.GetValue("UsSales_Warehouses").Split(',').ToList();
            }

        }

        public List<int> UsSales_FactoryIds
        {
            get
            {
                return Utilities.GetIdsFromString(Configuration.GetValue("UsSales_FactoryIds"));
            }

        }

        public string FeedbackCorrectiveActionsAdditionalEMails
        {   
            get { return Configuration.GetValue("FeedbackCorrectiveActionsAdditionalEMails"); }
        }

        public string FeedbackCorrectiveActionsAdditionalEMailsCC
        {
            get { return Configuration.GetValue("FeedbackCorrectiveActionsAdditionalEMailsCC"); }
        }

        public string FeedbackCorrectiveActionsAdditionalEMailsBCC
        {
            get { return Configuration.GetValue("FeedbackCorrectiveActionsAdditionalEMailsBCC"); }
        }


        public DateTime? Invoices_NewCalculationStartDate
        {
            get { return Configuration.GetDateValue("Invoices_NewCalculationStartDate"); }
        }

        public List<int> Invoices_NewCalculationExceptions_OrderIds
        {
            get
            {
                var sValue = Configuration.GetValue("Invoices_NewCalculationExceptions_OrderIds");
                if (!string.IsNullOrEmpty(sValue))
                    return Utilities.GetIdsFromString(sValue);
                return new List<int>();
            }
        }

        public string ClientDocumentsRootFolder
        {
            get { return Configuration.GetValue("ClientDocumentsRootFolder"); }
        }

        public int QCSupervisorLocation1
        {
            get { return Configuration.GetIntValue("QCSupervisorLocation1"); }
        }

        public int QCSupervisorLocation2
        {
            get { return Configuration.GetIntValue("QCSupervisorLocation2"); }
        }

        public int QCSupervisorLocation3
        {
            get { return Configuration.GetIntValue("QCSupervisorLocation3"); }
        }

        public string ApiKey
        {
            get
            {
                return Configuration.GetValue("apiKey");
            }
        }

        public DateTime? Invoices_AllocationBasedValueCalculationStartDate
        {
            get
            {
                return Configuration.GetDateValue("Invoices_AllocationBasedValueCalculationStartDate");
            }
        }

        public List<int?> Invoices_AllocationBasedValueCalculationClients
        {
            get
            {
                return Utilities.GetNullableIntsFromString(Configuration.GetValue("Invoices_AllocationBasedValueCalculationClients"));
            }
        }

        public List<int?> Invoices_AllocationBasedValueCalculationFactories
        {
            get
            {
                return Utilities.GetNullableIntsFromString(Configuration.GetValue("Invoices_AllocationBasedValueCalculationFactories"));
            }
        }

        public List<int?> OMExport_UsersForFinancialInfo
        {
            get
            {
                return Utilities.GetNullableIntsFromString(Configuration.GetValue("OMExport_UsersForFinancialInfo"));
            }
        }

        public List<int?> BudgetActual_DistributorsIds
        {
            get
            {
                return Utilities.GetNullableIntsFromString(Configuration.GetValue("BudgetActual_Distributors"));
            }
        }

	    public List<int?> OMExport_SaleOrdersCustomerIds => Utilities.GetNullableIntsFromString(Configuration.GetValue("OMExport_SaleOrdersCustomerIds"));

	    public DateTime? OMExport_SaleOrdersDateFrom => Configuration.GetDateValue("OMExport_SaleOrdersDateFrom");

		public DateTime? OMExport_SaleOrdersDateTo => Configuration.GetDateValue("OMExport_SaleOrdersDateTo");

	    public List<int?> InspectionReportClientsForAql =>
		    Utilities.GetNullableIntsFromString(Configuration.GetValue("InspectionReport_ClientsForAql"));

	    public int InspectionReportQuantityPercentage =>
		    Configuration.GetIntValue("InspectionReportQuantityPercentage");

	    public DateTime? InspectionReportAqlStartDate => Configuration.GetDateValue("InspectionReportAqlStartDate");

	    public List<int?> OMExport_SaleOrderExceptions =>
		    Utilities.GetNullableIntsFromString(Configuration.GetValue("OMExport_SaleOrderExceptions"));

	    public int Invoice_DefaultClient => Configuration.GetIntValue("Invoice_DefaultClient");

        public DateTime? InspectionV2_AddAutoAddedProducts_DateFrom => Configuration.GetDateValue("InspectionV2_AddAutoAddedProducts_DateFrom");
        public List<int?> ExpectedActualChartClientIds => Utilities.GetNullableIntsFromString(Configuration.GetValue("ExpectedActualChartClientIds"));

        public List<int?> UsersExcludedFromVAT => Utilities.GetNullableIntsFromString(Configuration.GetValue("UsersExcludedFromVAT"));

        public string ClaimsStatisticsClients => Configuration.GetValue("ClaimsStatisticsClients");

        public List<int?> Cprod_UserIds_ProductSpares => Utilities.GetNullableIntsFromString(Configuration.GetValue("Cprod_UserIds_ProductSpares"));

        public string OMExport_ExcludedCustProductsUsers => Configuration.GetValue("OMExport_ExcludedCustProductsUsers");

        public List<int?> OMExport_ForwardLinesClients => Utilities.GetNullableIntsFromString(Configuration.GetValue("OMExport_forwardlinesclients"));

        public MailConfigSettings LoadingInspectionReportSubmitted_MailSettings
        {
            get
            {
                MailConfigSettings settings;
                var ok = MailConfigSettings.TryParse(Configuration.GetValue("LoadingInspectionReportSubmitted_MailSettings"),
                    out settings);
                return ok ? settings : null;
            }

        }

        public DateTime? Invoices_NoCommision_EtdFrom => Configuration.GetDateValue("Invoices_NoCommision_EtdFrom");

        public List<int?> Invoices_NoCommision_Clients => Utilities.GetNullableIntsFromString(Configuration.GetValue("Invoices_NoCommision_Clients"));

        public List<int?> Invoices_NoCommision_CompaniesFrom => Utilities.GetNullableIntsFromString(Configuration.GetValue("Invoices_NoCommision_CompanyFrom"));
    }
}