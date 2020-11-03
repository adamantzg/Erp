using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.ReportingModules.SalesBySector
{
	public class Parameters
	{
		public CountryFilter CountryFilter { get; set; }
		public DateTime From { get; set; }
		public string excludedCustomers { get; set; }
		public string PrivateQcClientCodes { get; set; }
		public List<int?> ProRataBrandIds { get; set; }

		public Parameters()
		{
			From = DateTime.Now;
			excludedCustomers = "184,321";
			PrivateQcClientCodes = "BS,W1";
			ProRataBrandIds = new List<int?> { 7,202,21};
		}
	}
}