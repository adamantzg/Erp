using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace erp.Model
{
    public class ds_guarantee_registration
    {
        [Key]
        public int GRID { get; set; }
        public string GRCusDetailsName { get; set; }
        public string GRCusDetailsPostAdress { get; set; }
        public string GRCusDetailsTown { get; set; }
        public string GRCusDetailsCountry { get; set; }
        public string GRCusDetailsPostalCode { get; set; }
        public string GRCusDetailsEmail { get; set; }
        public string GRCusDetailTelephone { get; set; }
        public bool? GRCusDetailReceivePromoEmail { get; set; }
        public bool? GRCusDetailReceivePromoDirect { get; set; }
        public DateTime? GRSupDetailsPurchaseDate { get; set; }
        public string GRSupDetailsPurchasedFrom { get; set; }
        public string GRSupDetailsTown { get; set; }
        public string GRSupDetailsCountry { get; set; }
        public string GRSupDetailsPostCode { get; set; }
        public string GRInstDetailsInstalledByName { get; set; }
        public string GRInstDetailsInstalledByCompany { get; set; }
        public string GRInstDetailsInstallerEmail { get; set; }
        public DateTime? GRInstDetailsInstallationDate { get; set; }
        public int? GRInstDetailsProcessorType { get; set; }
        public int? GRInstDetailsProcessorVariant { get; set; }
        public bool? GRInstDetailsRemoteButton { get; set; }
        public string GrInstDetailsShowerStyle { get; set; }
        public DateTime? GRDateCreated { get; set; }
    }
}
