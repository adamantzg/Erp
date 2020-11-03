using erp.DAL.EF;
using erp.DAL.EF.New;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI.DataVisualization.Charting;

namespace backend.ReportingModules
{
	public abstract class ReportingModuleBase
	{
		public string SectionName { get; set; }

		public string PartialViewName { get; set; }

		public string Title { get; set; }

		public object ViewModel { get; set; }

		public int? AudienceId { get; set; }

		protected HttpClient apiClient = new HttpClient();

		public virtual void ParseParameters(Dictionary<string, object> parameters) {
		}

		protected IUnitOfWork unitOfWork;

		public ReportingModuleBase(IUnitOfWork unitOfWork)
		{
			Initialize();
		}

		public ReportingModuleBase(int? audience_id)
		{
			AudienceId = audience_id;
			Initialize();
		}

		private void Initialize()
		{
			apiClient.DefaultRequestHeaders.Add("apiKey", Properties.Settings.Default.sageApiKey);
		}

		protected virtual void SaveChartImage(string name, Chart chart)
		{
			
			var	folder = Properties.Settings.Default.Analytics_ImagesFolder;
			chart.SaveImage(
				Path.Combine(HttpContext.Current.Server.MapPath(folder),$"{name}.jpg"),
				ChartImageFormat.Jpeg);
		}

		protected string GetImageUrl(string chartname)
		{
			return $"{WebUtilities.GetSiteUrl()}/analytics/image?param={chartname}%23{Properties.Settings.Default.StatsKey}";
		}

		public abstract void LoadData();
	}
}