using company.Common;
using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.ReportingModules.SalesBySector
{
	public class ViewModel
	{
		public Dictionary<string, List<DataPoint>> Data { get; set; }
		public List<string> sectors = new List<string> { "BBD UK", "BBD non-UK / Europe", "BB Asia", "Private label QC", "Crosswater retail",
				"Crosswater Intl","Projects inc 36", "USA" };
		public Month21 From { get; set; }
		public Month21 To { get; set; }
		public string Title { get; set; }
		public string ChartUrl { get; set; }
		public double? BudgetProRataCurrMonth { get; set; }

		public ViewModel()
		{
			Data = new Dictionary<string, List<DataPoint>>();			
		}
	}

	public class DataPoint
	{
		public Month21 Month21 { get; set; }
		public double? Amount { get; set; }
	}
}