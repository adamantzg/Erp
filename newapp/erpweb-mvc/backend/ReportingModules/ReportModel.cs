using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.ReportingModules
{
	public class ReportModel
	{
		public Ar_report Report { get; set; }
		public DateTime ReportDate { get; set; }
		public List<UISection> Sections { get; set; }
	}
}