using erp.Model;
using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class StatisticsModel
    {
        public List<Web_site_visits> Visits { get; set; }
        public List<Web_site> Websites { get; set; }
        public DateTime? datefrom { get; set; }
        public DateTime? dateto { get; set; }
        public int? site_id { get; set; }
        public int? limit { get; set; }
    }

    public class StatisticsBrandsModel
    {
        public List<Web_site> Websites { get; set; }
        public Dictionary<int, List<Web_site_visits>> TableHeader { get; set; }
        public List<Countries> Countries { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? DateFromCompare { get; set; }
        public DateTime? DateToCompare { get; set; }
        public Dictionary<int, List<Web_site_visits>> SitesVisitsByBrand { get; set; }
        public Dictionary<int, List<Web_site_visits>> SitesVisitsByBrandPrevMonth { get; set; }
        public bool UniqueVisits { get; set; }
    }
   
}