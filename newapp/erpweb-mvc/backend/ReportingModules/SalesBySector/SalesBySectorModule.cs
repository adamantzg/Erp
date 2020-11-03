using company.Common;
using erp.Model;
using erp.DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model.DAL;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
//using Crosswater.Model;
using System.Threading.Tasks;
using System.Net.Http;

namespace backend.ReportingModules.SalesBySector
{
	public class SalesBySectorModule : ReportingModuleBase
	{
		private Parameters Parameters;
		
		

		public SalesBySectorModule()
		{
			Initialize();
		}

		public SalesBySectorModule(int? audience_id) : base(audience_id)
		{
			Initialize();
		}

		private void Initialize()
		{
			PartialViewName = "~/Views/ReportingModules/SalesBySector/_SalesBySector.cshtml";
			Parameters = new Parameters();
		}

		public override void LoadData()
		{
			var model = new ViewModel();
			ViewModel = model;

			List<BudgetActualData> budgetActualData;
						
			var fromMonth21 = Month21.FromDate(new DateTime(Parameters.From.Year, 1, 1));
			var toMonth21 = (fromMonth21+11);

			var ukCountries = new[] { "GB", "IE", "UK" };
			var asianCountries = CountriesDAL.GetAll().Where(c => c.continent_code == "AS").Select(c => c.ISO2).ToList();

			model.From = fromMonth21;
			model.To = toMonth21;
			var dataFrom = fromMonth21.Date;
			var dataTo = company.Common.Utilities.GetMonthEnd(toMonth21.Date);
			var excludedProductsList = new[] { "S1DISPLAY","S1TRAINING","S1MARKETING" };
			var excludedProductsNewList = new[] { "S1TRAINING", "S1MARKETING" };
			var newRulesDate = new DateTime(2018, 1, 1);
			var excludedFactoriesList = new[] { "99" };

			Title = $"Sales / open orders - GBP";

			budgetActualData =
				BudgetRepository.GetBudgetActualDataByCriteria(fromMonth21.Value, toMonth21.Value, CountryFilter.All, Parameters.excludedCustomers);

            //var testb = budgetActualData.Where(b => b.Distributor != null).ToList();

            //sector 0 - BBD UK
            model.Data[model.sectors[0]] = budgetActualData.Where(d => d.record_type == "A" && d.Distributor != null && ukCountries.Contains(d.Distributor.user_country))
				.GroupBy(d => d.month21).Select(g => new DataPoint { Month21 = new Month21(g.Key), Amount = g.Sum(d => d.value) }).ToList();

			var maxActualMonth = new Month21(model.Data[model.sectors[0]].Max(b => b.Month21.Value)) + 1;
			var intMaxActualMonth = maxActualMonth.Value;

			model.Data[model.sectors[0]].AddRange(AnalyticsDAL.GetSalesByMonth2(intMaxActualMonth, toMonth21.Value, CountryFilter.UKOnly, useETA: true,
				excludedCustomers: Parameters.excludedCustomers, brands: true).Select(d=> new DataPoint { Month21 = new Month21(d.Month21), Amount = d.Amount }));

			//sector 1 - BBD Non-UK
			model.Data[model.sectors[1]] = budgetActualData.Where(d => d.record_type == "A" && d.Distributor != null && !ukCountries.Contains(d.Distributor.user_country))
				.GroupBy(d => d.month21).Select(g => new DataPoint { Month21 = new Month21(g.Key), Amount = g.Sum(d => d.value) }).ToList();

			maxActualMonth = new Month21(model.Data[model.sectors[1]].Max(b => b.Month21.Value)) + 1;
			intMaxActualMonth = maxActualMonth.Value;

			model.Data[model.sectors[1]].AddRange(AnalyticsDAL.GetSalesByMonth2(intMaxActualMonth, toMonth21.Value, CountryFilter.NonUKExcludingAsia, useETA: true,
				excludedCustomers: Parameters.excludedCustomers, brands: true).Select(d => new DataPoint { Month21 = new Month21(d.Month21), Amount = d.Amount }));

			//sector 2 - BB Asia			
			model.Data[model.sectors[2]] = AnalyticsDAL.GetSalesByMonth2(fromMonth21.Value, toMonth21.Value, CountryFilter.NonUK, useETA: true, useCompanyPriceType: true,
				excludedCustomers: Parameters.excludedCustomers, brands: false).Select(d=> new DataPoint { Month21 = new Month21(d.Month21), Amount = d.Amount }).ToList();

			//sector 3 - private qc
			model.Data[model.sectors[3]] = AnalyticsDAL.GetSalesByMonth2(fromMonth21.Value, toMonth21.Value, CountryFilter.All, useETA: false, useCompanyPriceType: false,
				excludedCustomers: "", brands: false,includedCustomers: Parameters.PrivateQcClientCodes,excludeSpares: false)
				.Select(d => new DataPoint { Month21 = new Month21(d.Month21), Amount = d.Amount * 0.08 }).ToList();

			var sageSalesData = GetSageSalesData(fromMonth21, toMonth21);

			if(sageSalesData != null)
			{
				model.Data[model.sectors[4]] = sageSalesData.Where(d => d.sector.In("RETAIL"))
					.GroupBy(d=>new { d.year, d.month })
					.Select(g => new DataPoint { Month21 = new Month21(new DateTime(g.Key.year, g.Key.month, 1)), Amount = g.Sum(d=>d.value)}).ToList();
				model.Data[model.sectors[5]] = sageSalesData.Where(d => d.sector == "International")
					.GroupBy(d => new { d.year, d.month })
					.Select(g => new DataPoint { Month21 = new Month21(new DateTime(g.Key.year, g.Key.month, 1)), Amount = g.Sum(d => d.value) }).ToList();
				model.Data[model.sectors[6]] = sageSalesData.Where(d => d.sector.StartsWith("T6") || d.sector == "CONTRACT")
					.GroupBy(d => new { d.year, d.month })
					.Select(g => new DataPoint { Month21 = new Month21(new DateTime(g.Key.year, g.Key.month, 1)), Amount = g.Sum(d => d.value) }).ToList();
			}

			//USA
			var exchangeRates = unitOfWork.ExchangeRateRepository.Get(r => r.month21 >= fromMonth21.Value).ToList();
			model.Data[model.sectors[7]] = unitOfWork.SalesOrdersRepository.GetQuery(so => so.date_report != null && so.date_report >= dataFrom && so.date_report <= dataTo && so.value != 0
					&& !excludedFactoriesList.Contains(so.warehouse) && ((newRulesDate != null && so.date_report >= newRulesDate) || !so.delivery_reason.Contains("DISPLAY"))
					&& (((newRulesDate == null || so.date_report < newRulesDate) && !excludedProductsList.Contains(so.cprod_code1))
						|| ((newRulesDate != null && so.date_report >= newRulesDate) && !excludedProductsNewList.Contains(so.cprod_code1)))).
						GroupBy(so => new { so.date_report.Value.Year, so.date_report.Value.Month }).ToList().
						Select(g => new DataPoint { Month21 = Month21.FromYearMonth(g.Key.Year, g.Key.Month),
							Amount = g.Sum(so=>so.value) * UsdToGbp(exchangeRates, Month21.FromYearMonth(g.Key.Year, g.Key.Month))}).ToList();

			model.Data["budget"] = budgetActualData.Where(d => d.record_type == "B" && d.brand_id != null)
				.GroupBy(d => d.month21).Select(g => new DataPoint { Month21 = new Month21(g.Key), Amount = g.Sum(d => d.value) }).ToList();

			model.BudgetProRataCurrMonth = budgetActualData.Where(d => d.record_type == "B" && d.brand_id != null && d.month21 == Month21.Now.Value)
					.GroupBy(d => d.brand_id).Select(g => new { amount = GetProRataValue(g.Key, g.Sum(d => d.value)) }).Sum(d => d.amount);

			GenerateChart(model);
		}

		private double? GetProRataValue(int? brand_id, double? value)
		{
			var currMonth21 = Month21.Now;
			if (Parameters.ProRataBrandIds.Contains(brand_id))
			{
				return value;
			}
			else
			{
				return value * (DateTime.Now.Day * 1.0 / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
			}			
		}

		private double? UsdToGbp(List<Exchange_rates> rates, Month21 month21)
		{
			var exchRate = rates.FirstOrDefault(r => r.month21 == month21);
			if (exchRate == null)
				exchRate = rates.OrderBy(r => r.month21).Last();
			return 1.0 / exchRate?.usd_gbp;
		}

		private void GenerateChart(ViewModel model)
		{
			var chart = new Chart { Width = 900, Height = 290 };
			
			FormatChart(chart);
						
			var currMonth = company.Common.Utilities.GetMonthFromNow(0);
			DataPoint anchorPoint = null;

			foreach(var s in model.sectors)
			{
				if(model.Data.ContainsKey(s))
				{
					var series = new Series
					{
						MarkerStyle = MarkerStyle.None,
						ChartType = SeriesChartType.Line,
						BorderWidth = 3,
						LegendText = s,
						//string.Format("{0}/{1}", DateTime.Today.Year - 2, DateTime.Today.Year - 1),
						XValueType = ChartValueType.Date
					};
					for (var i = model.From; i <= model.To; i += 1)
					{
						var value = model.Data[s].FirstOrDefault(d => d.Month21 == i)?.Amount;
						series.Points.AddXY(i.Date, value);						
					}
					chart.Series.Add(series);
				}
				
			}
			var chartKey = Guid.NewGuid();
			var chartName = $"chart_{chartKey}";
			//return File(StreamChart(chart), "image/jpg");
			model.ChartUrl = GetImageUrl(chartName);
			SaveChartImage(chartName, chart);
		}

		private void FormatChart(Chart chart)
		{
			var area = new ChartArea();
			area.AxisY.IsInterlaced = true;
			area.AxisY.InterlacedColor = Color.FromArgb(0xEE, 0xEE, 0xEE);

			area.AxisX.Title = "";
			area.AxisX.IntervalType = DateTimeIntervalType.Months;
			area.AxisX.Interval = 1;
			area.AxisX.MajorGrid.Enabled = false;
			area.AxisX.MinorGrid.Enabled = false;

			area.AxisY.MajorGrid.LineColor = Color.LightGray;
			area.AxisY.Title = "GBP";
			area.AxisY.LabelStyle.Format = "N0";


			area.AxisX.LabelStyle.Format = "MMM-yy";

			var legend = new Legend();
			legend.Docking = Docking.Bottom;
			legend.Alignment = StringAlignment.Center;
			chart.Legends.Add(legend);

			chart.ChartAreas.Add(area);
		}

		public override void ParseParameters(Dictionary<string, object> parameters)
		{
			base.ParseParameters(parameters);
			if(parameters != null)
			{
				if (parameters.ContainsKey("from"))
				{
					DateTime date;
					if (DateTime.TryParse(parameters["from"].ToString(), out date))
					{
						Parameters.From = date;
					}
				}
				if (parameters.ContainsKey("excludedCustomers"))
				{
					Parameters.excludedCustomers = parameters["excludedCustomers"].ToString();
				}
				if (parameters.ContainsKey("privateQcClientCodes"))
				{
					Parameters.PrivateQcClientCodes = parameters["privateQcClientCodes"].ToString();
				}
				if(parameters.ContainsKey("prorata_brands"))
				{
					Parameters.ProRataBrandIds = company.Common.Utilities.GetNullableIntsFromString(parameters["prorata_brands"].ToString());
				}
			}
		}

		private List<SalesData> GetSageSalesData(Month21 from, Month21 to)
		{
			List<SalesData> result = null;
			try
			{
				var response = apiClient.GetAsync($"{Properties.Settings.Default.sageApiUrl}/api/getSalesData?monthFrom={from.Date.Month}&yearFrom={from.Date.Year}&monthTo={to.Date.Month}&yearTo={to.Date.Year}").Result;
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					result = response.Content.ReadAsAsync<List<SalesData>>().Result;
				}
			}
			catch (Exception)
			{
				//Until CW opens port for hk server
			}			
			return result;
		}
	}
}