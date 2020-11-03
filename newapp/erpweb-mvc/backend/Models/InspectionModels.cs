using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using erp.Model;

namespace backend.Models
{
    public class InspectionsPlanListModel
    {
        public List<Location> Locations { get; set; }
        public int location_id { get; set; }
        
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<Inspections> Inspections { get; set; }
        public List<User> Inspectors { get; set; }
        public bool ForPdf { get; set; }
        public bool ForExport { get; set; }
        public List<int> Inspectors_Seeunallocated { get; set; }    //list of inspectors who can see unallocated inspections
    }

    public class InspectionPlanEditModel
    {
        public Inspections Inspection { get; set; }
        public string returnTo { get; set; }
        public List<User> Inspectors { get; set; }
        public int location_id { get; set; }
        public bool IsV2 { get; set; }
    }

    public class InspectionExportModel
    {
        public List<Company> Factories { get; set; }
        public List<Inspections> Inspections { get; set; }
        public List<Inspections> LoadingInspections { get; set; }
        public List<Order_lines> Lines { get; set; }
        public Dictionary<int, int> ProductPreviousShipments { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Dictionary<string, int> InspectionChangedProducts { get; set; }
        //public string SelectedFactories { get; set; }
    }

    public class InspectionReportModel
    {
        public Inspections Inspection { get; set; }
        public List<User> Inspectors { get;set; } 
        public List<Inspection_lines_accepted> InspectionLinesAccepted { get; set; }
        public List<Inspection_lines_rejected> InspectionLinesRejected { get; set; }
        public List<Inspection_lines_tested> InspectionLinesTested { get; set; }
        public List<Inspection_images> InspectionImages { get; set; }
        public List<Mast_products> Products { get; set; }
        public List<Cust_products> CustProducts { get; set; } 
        public List<Inspection_criteria> InspectionCriteria { get; set; }
        public List<Containers> Containers { get; set; }
        public List<Inspections_loading> InspectionLoadings { get; set; }
        public List<Container_images> ContainerImages { get; set; }
        public Company Client { get; set; }
        public Company Factory { get; set; }
        public string InspectorName { get; set; }
        public List<products_track_number_fc> ProductTrackNumbers { get; set; }
        public List<Cust_products> ProductsForTracking { get; set; }
        public string ImagesFolder { get; set; }
        public List<Factory_client_settings> FactoryClientSettings { get; set; }
		public List<Return_category> ReturnCategories { get; set; }
		public List<aql_new_range> Ranges { get; set; }
		public List<aql_new_range_level_sample> RangeLevelSamples { get; set; }
		public List<aql_new_category1_returncategory_level> Category1ReturncategoryLevels { get; set; }
		
    }

    public class InspectionBulkModel
    {
        public List<Inspection_lines_tested> InspectionLines { get; set; }
        public List<Inspections_documents> Insepctions { get; set; }   

        public List<Cust_product_doctype> DocTypes { get; set; }
    }
    public class FilteredBulkDownload
    {
        public List<Company> Factories { get; set; }
        public List<Cust_product_doctype> DocTypes{ get; set; }
        
        /* Privremeno */
        public List<Inspections_documents> Insepctions { get; set; }
    }
    public class ImprovedChangedLoadingReportModel
    {
        public Inspections Inspection { get; set; }
        public string insp_document { get; set; }
        public inspection_notified_summary_table InspectionNotifiedSummary { get; set; }
        public List<inspection_lines_notified_v2_table> InspectionLinesNotified { get; set; }
        public List<inspection_notified_summary_images_table> InspectionNotifiedSummaryImages { get; set; }
        public Dictionary<int,List<change_notice_table>> ChangeNotices { get; set; }
        public List<inspection_images_v2_table> InspectionImages { get; set; }
        public bool ForPdf { get; set; }
    }

    public class ImprovedChangedImagesPartialModel
    {
        public string Caption { get; set; }
        public string SubTitle { get; set; }
        public bool ShowCaptions { get; set; }
        public List<string> Images { get; set; }
        
    }

    public class LoadingInspectionV2ReportModel
    {
        public const int LoadingTableRowsPerFirstPage = 28;
        public const int LoadingTableRowsPerPage = 53;

        public Inspection_v2 Inspection { get; set; }
        public List<Inspection_v2> CombinedInspections { get; set; }
        public List<Order_header> CombinedOrders { get; set; }
        public List<User> Inspectors { get; set; }
        
        public bool ForPdf { get; set; }
        public bool IsEdit { get; set; }
        public string ImagesFolder { get; set; }
        public List<Inspection_v2_area> Areas { get; set; }
        public List<Inspection_v2_loading> AllLoadings { get; set; }

        public LoadingInspection2EditModel EditModel { get; set; }

        
        public List<Inspection_v2_line> AutoAddedLines { get; set; }
    }

    public class LoadingInspection2EditModel
    {
        public Inspection_v2 Inspection { get; set; }
        public List<Inspection_v2_container> Containers { get; set; }
        public List<InspectionV2Loading> AllLoadings { get; set; }
        public List<Inspection_v2_image> AllImages { get; set; }
        public List<Inspection_v2_area> Areas { get; set; }
        public List<Inspection_v2_line> AutoAddedLines { get; set; }
    }

    public class InspectionV2Loading
    {        
        public int id { get; set; }
        public int? insp_line { get; set; }
        public double? qty { get; set; }
        public int? container_id { get; set; }
        public string cprod_code { get; set; }
        public string factory_ref { get; set; }
        public string custpo { get; set; } 
        public int? full_pallets { get; set; }
        public int? qty_per_pallet { get; set; }
        public int? loose_load_qty { get; set; }
        public int? mixed_pallet_qty { get; set; }
        public int? mixed_pallet_qty2 { get; set; }
        public int? mixed_pallet_qty3 { get; set; }
        public int? area_id { get; set; }
        public List<Inspection_v2_loading_mixedpallet> QtyMixedPallets { get; set; }
        public string AreasText { get; set; }
        public string Description { get; set; }
    }
}