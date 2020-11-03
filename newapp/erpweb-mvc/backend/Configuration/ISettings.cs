using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;

namespace backend.Properties
{
	public interface ISettings
	{
		int CreditNote_Create_InvoiceFromId_NonUK { get; }
		int CreditNote_Create_InvoiceFromId { get; }
		string Analytics_IncludedNonDistributors { get; }
		string CreditNote_Create_UKDistributors_Exceptions { get; }
		string Analytics_ForBrands_ExcludedCustomers { get; }
		string BBSInvoices_ExcludedNonUKCustomers { get; }
		List<int> PendingInvoicesException_Companies { get; }
		List<int> OMExport_UseridsForExtraValueFields { get; }
		string TrackingNumbersMail_Subject { get; }
		string TrackingNumbersMail__To { get; }
		string TrackingNumbersMail__Cc { get; }
		string TrackingNumbersMail__Bcc { get; }
		string OMExport_CompaniesUsingBookedInDate { get; }
		DateTime? Invoice_ComponentCalculation_Start { get; }
		string OMExport_ClientsUsingSalesOrders { get; }
		List<int> UsSales_CompanyIds { get; }
		List<string> UsSales_Warehouses { get; }
		List<int> UsSales_FactoryIds { get; }
		string FeedbackCorrectiveActionsAdditionalEMails { get; }
		string FeedbackCorrectiveActionsAdditionalEMailsCC { get; }
		string FeedbackCorrectiveActionsAdditionalEMailsBCC { get; }
		DateTime? Invoices_NewCalculationStartDate { get; }
		List<int> Invoices_NewCalculationExceptions_OrderIds { get; }
		string ClientDocumentsRootFolder { get; }
		int QCSupervisorLocation1 { get; }
		int QCSupervisorLocation2 { get; }
		int QCSupervisorLocation3 { get; }
		string ApiKey { get; }
		DateTime? Invoices_AllocationBasedValueCalculationStartDate { get; }
		List<int?> Invoices_AllocationBasedValueCalculationClients { get; }
		List<int?> Invoices_AllocationBasedValueCalculationFactories { get; }
		List<int?> OMExport_UsersForFinancialInfo { get; }
		List<int?> BudgetActual_DistributorsIds { get; }
		List<int?> OMExport_SaleOrdersCustomerIds { get; }
		DateTime? OMExport_SaleOrdersDateFrom { get; }
		DateTime? OMExport_SaleOrdersDateTo { get; }
		List<int?> InspectionReportClientsForAql { get; }
		int InspectionReportQuantityPercentage { get; }
		DateTime? InspectionReportAqlStartDate { get; }
		List<int?> OMExport_SaleOrderExceptions { get; }
		int Invoice_DefaultClient { get; }
		DateTime? InspectionV2_AddAutoAddedProducts_DateFrom { get; }
		List<int?> ExpectedActualChartClientIds { get; }
        List<int?> UsersExcludedFromVAT { get; }
		string ClaimsStatisticsClients { get;}
        List<int?> Cprod_UserIds_ProductSpares { get; }
        string OMExport_ExcludedCustProductsUsers { get; }
		List<int?> OMExport_ForwardLinesClients { get;}
		DateTime? Invoices_NoCommision_EtdFrom { get;  }
		List<int?> Invoices_NoCommision_Clients { get;  }
		List<int?> Invoices_NoCommision_CompaniesFrom { get;  }
	}
}