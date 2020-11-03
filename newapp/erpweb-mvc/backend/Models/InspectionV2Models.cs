using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class InspectionTemplateEditModel
    {
        public List<LookupItem> Factories { get; set; }
        public List<LookupItem> Clients { get; set; }
        public Inspv2_template Template { get; set; }
        public List<Inspv2point> Points { get; set; }
        public List<Inspv2_criteriacategory> Categories { get; set; }
    }

    public class InspectionProductCriteriaModel
    {
        public List<LookupItem> Factories { get; set; }
        public List<LookupItem> Clients { get; set; }
        public List<Inspv2_template> Templates { get; set; }
        public List<Inspv2point> Points { get; set; }
        public List<Inspv2_criteriacategory> Categories { get; set; }
    }

    public class InspectionLoadingTableModel
    {
        public List<Inspection_v2_loading> Loadings { get; set; }
        public int StartIndex { get; set; }
        public int Rows { get; set; }
        public int Column { get; set; }
        public bool ForPdf { get; set; }
        public bool IsEdit { get; set; }
    }

    public class OrderLoadingModel
    {
        public Order_header OrderHeader { get; set; }
        public int PorderId { get; internal set; }
    }
    public class OrderLoadingModelFinal
    {
        public Order_header OrderHeader { get; set; }
        public List<Inspection_v2_type> InspectionTypes { get; set; }

        public List<Order_lines> OrderLines { get; set; }

        public List<Inspection_v2_line> InspectionLinesTable { get; set; }
    }
    public class OrderLineTable
    {
        public string cprod_code1 { get; set; }
        public string factory_ref{ get; set; }
        public string cprod_name { get; set; }
        public double? insp_qty { get; set; }

        public double? order_qty { get; set; }
    }
    public class InspectionObjForUpdate
    {
        public int id { get; set; }
        public DateTime? startdate { get; set; }
        public int? type { get; set; }
        public string custpo { get; set; }
        public int? factory_id { get; set; }
        public string code { get; set; }
        public int? client_id { get; set; }
        public int? duration { get; set; }
        public string comments { get; set; }
        public int? qc_required { get; set; }
        public int orderid { get; set; }
        public string drawingFile { get; set; }
        public List<int> orderids { get; set; }
        public List<ContainersNumber> Containers { get; set; }
        public List<Inspection_v2_line> Lines { get; set; }
        public string file_id { get; set; }
        public bool? factory_loading { get; set; }
    }

    public class OrdersCombinedLoadingList
    {
        public List<Company> Clients { get; set; }

        public List<Company> Factories { get; set; }

        public List<Container_types> Containers { get; set; }
    }
    public class ContainersNumber
    {
        public int id { get; set; }
        public int num { get; set; }
    }

    public class InspectionProductListModel
    {
        public Inspection_v2 Inspection { get; set; }
        public List<Inspection_v2> CombinedInspections { get; set; }
        public List<Inspection_v2_type> InspectionV2Types { get; set; }
        public List<Returns> Returns { get; set; }
        public List<Company> Factories { get; set; }
        public bool ShowEditLink { get; set; }
        public bool EditMode { get; set; }
        public List<change_notice_product_table> ChangeNoticesProducts { get; set; }
        public List<Inspection_v2_line> AutoAddedLines { get; set; }
    }

    public class InspectionListModel
    {
        //public List<Inspection_v2> Inspections { get; set; }
        public List<Company> Factories { get; set; }
        public List<Company> Clients { get; set; }
        public List<LookupItem> StatusFilters { get; set; }
    }

    public class InspectionListItem
    {
        public InspectionStatus status { get; set; }
        public InspectionForDisplay FI { get; set; }
        public InspectionForDisplay LI { get; set; }
    }

    public class InspectionListResult
    {
        public List<InspectionListItem> FiLiInspections { get; set; }
        public List<InspectionForDisplay> OtherInspections { get; set; }
    }

    public class InspectionForDisplay
    {
        public int id { get; set; }
        public string factory_code { get; set; }
        public string customer_code { get; set; }
        public string type { get; set; }
        public string[] custpos { get; set; }
        public InspectionV2Status? insp_status { get; set; }
        public DateTime? startdate { get; set; }
    }

    public class FinalInspectionV2ReportModel
    {
        
        public Inspection_v2 Inspection { get; set; }
        public List<User> Inspectors { get; set; }

        //public List<Mast_products> Products { get; set; }
        //public List<Cust_products> CustProducts { get; set; }
        //public List<Inspection_criteria> InspectionCriteria { get; set; }
        //public List<Inspection_v2_container> Containers { get; set; }
        public bool ForPdf { get; set; }
        public bool IsEdit { get; set; }
        public string ImagesFolder { get; set; }
        public FinalInspection2EditModel EditModel { get; set; }
    }

    public class FinalInspection2EditModel
    {
        public Inspection_v2 Inspection { get; set; }
        public List<Inspection_v2_Image_ex> AllImages { get; set; }
        public List<Inspection_v2_image_type> ImageTypes { get; set; }
    }

    public class TrackingNumbersModel
    {
        public List<products_track_number_qc> ProductTrackNumbers { get; set; }
        public List<InspectionListFlatLine> Lines { get; set; }
    }


    public class TrackingNumbersModelMail
    {
        public int id { get; set; }
        public string factory_code { get; set; }
        public string customer_code { get; set; }
        public string custpo { get; set; }
        public string qc { get; set; }
        public string report_number { get; set; }
        public List<products_track_number_qc> product_track_number_gc_list { get; set; }
    }

    public class Inspection_v2_Image_ex : Inspection_v2_image
    {
        public string fileId { get; set; }
        public byte[] Data { get; set; }
    }

    public class InspectionListFlatLine
    {
        public int id { get; set; }
        public string custpo { get; set; }
        public int? factory_id { get; set; }
        public string factory_code { get; set; }
        public string factory_ref { get; set; }
        public string cprod_code { get; set; }
        public string cprod_name { get; set; }
        public double? qty { get; set; }
        public int? mast_id { get; set; }
        public int? insp_id { get; set; }
        public Mast_products MastProduct { get; set; }
        public Cust_products CustProduct { get; set; }
    }

    
    public class NRReportInspectionLine
    {
        public int insp_line_id { get; set; }
        public int? insp_line_id_v2 { get; set; }

    }

    public class NREditModel
    {
        public bool? IsFromOldRecord { get; set; }
        public object Inspection { get; set; }
        public object NrHeader { get; set; }
    }

    public class NRReportEditModel : NREditModel
    {
        public List<Order_header> Orders { get; set; }
        public List<Nr_image_type> ImageTypes { get; set; }
        public string ImagesRootUrl { get; set; }
    }

    public class NRReportRenderModel
    {
        public List<Order_header> Orders { get; set; }
        public List<Nr_image_type> ImageTypes { get; set; }
        public string ImagesRootUrl { get; set; }
        public bool? IsFromOldRecord { get; set; }
        public Inspection_v2 Inspection { get; set; }
        public Nr_header NrHeader { get; set; }
        public string Title { get; set; }
    }


}