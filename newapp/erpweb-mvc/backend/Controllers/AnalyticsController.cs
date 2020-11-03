using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

using System.Web.UI.DataVisualization.Charting;
using ASPPDFLib;
using company.Common;
using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using backend.Models;
using backend.Properties;
//using MVCControlsToolkit.Controls;

using Utilities = company.Common.Utilities;
using System.Diagnostics;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using backend.ApiServices;
using System.Web.Security;

namespace backend.Controllers
{

    public enum BrandSalesChartType
    {
        Lines,
        Bars
    }

    public enum DateMode
    {
        YearFromNow,
        FromYearStart,
        All
    }

	

    public class AnalyticsController : BaseController
    {
        
        
        private readonly IAnalyticsDAL analyticsDAL;
        private readonly ICompanyDAL companyDAL;
        private readonly IBrandsDAL brandsDAL;
        private readonly ISabcSortDal sabcSortDal;
        private readonly IReturnsDAL returnsDAL;
        private readonly IOrderLineExportDal orderLineExportDal;
        private readonly ICountriesDAL countriesDAL;
        private readonly IStockCodeDal stockCodeDal;
        private readonly ICustproductsDAL custproductsDAL;
        private readonly ISalesForecastDal salesForecastDal;
        private readonly IOrderLinesDAL orderLinesDAL;
        private readonly IStockCodeFactoryDal stockCodeFactoryDal;
        private readonly IAnalyticsSubcategoryDal analyticsSubcategoryDal;
        private readonly IAnalyticsOptionDal analyticsOptionDal;
        private readonly IAnalyticsCategoryDal analyticsCategoryDal;
        private readonly IReturnCategoryDAL returnCategoryDAL;
        private readonly IBrandSalesAnalysis2DAL brandSalesAnalysis2DAL;
        private readonly ILoginHistoryDetailDAL loginHistoryDetailDAL;
        private readonly IAdminPagesDAL adminPagesDAL;
        private readonly IAdminPagesNewDAL adminPagesNewDAL;
        private readonly IClientPagesAllocatedDAL clientPagesAllocatedDAL;
        private readonly IAccountService accountService;
        public const int AgeForExclusion = 6;
        public const string DistCountries = "GB,IE";
        
        // GET: /Analytics/

        public AnalyticsController(IUnitOfWork unitOfWork, IAnalyticsDAL analyticsDAL, ICompanyDAL companyDAL, IBrandsDAL brandsDAL, ISabcSortDal sabcSortDal,
            IReturnsDAL returnsDAL, IOrderLineExportDal orderLineExportDal, ICountriesDAL countriesDAL, IStockCodeDal stockCodeDal, ICustproductsDAL custproductsDAL,
            ISalesForecastDal salesForecastDal, IOrderLinesDAL orderLinesDAL, IStockCodeFactoryDal stockCodeFactoryDal, IAnalyticsSubcategoryDal analyticsSubcategoryDal,
            IAnalyticsOptionDal analyticsOptionDal, IAnalyticsCategoryDal analyticsCategoryDal, IReturnCategoryDAL returnCategoryDAL,
            IBrandSalesAnalysis2DAL brandSalesAnalysis2DAL, ILoginHistoryDetailDAL loginHistoryDetailDAL, IAdminPagesDAL adminPagesDAL, IAdminPagesNewDAL adminPagesNewDAL,
            IClientPagesAllocatedDAL clientPagesAllocatedDAL, IAccountService accountService
            ) : base(unitOfWork, loginHistoryDetailDAL, companyDAL, adminPagesDAL, adminPagesNewDAL, clientPagesAllocatedDAL, accountService )
        {            
            
            this.analyticsDAL = analyticsDAL;
            this.companyDAL = companyDAL;
            this.brandsDAL = brandsDAL;
            this.sabcSortDal = sabcSortDal;
            this.returnsDAL = returnsDAL;
            this.orderLineExportDal = orderLineExportDal;
            this.countriesDAL = countriesDAL;
            this.stockCodeDal = stockCodeDal;
            this.custproductsDAL = custproductsDAL;
            this.salesForecastDal = salesForecastDal;
            this.orderLinesDAL = orderLinesDAL;
            this.stockCodeFactoryDal = stockCodeFactoryDal;
            this.analyticsSubcategoryDal = analyticsSubcategoryDal;
            this.analyticsOptionDal = analyticsOptionDal;
            this.analyticsCategoryDal = analyticsCategoryDal;
            this.returnCategoryDAL = returnCategoryDAL;
            this.brandSalesAnalysis2DAL = brandSalesAnalysis2DAL;
            this.loginHistoryDetailDAL = loginHistoryDetailDAL;
            this.adminPagesDAL = adminPagesDAL;
            this.adminPagesNewDAL = adminPagesNewDAL;
            this.clientPagesAllocatedDAL = clientPagesAllocatedDAL;
            this.accountService = accountService;
        }

        

		private void GetSalesByMonthData(DateTime from, CountryFilter countryFilter, int? brand_id, DateMode brandShipmentsDateMode, 
			int monthsInFuture, bool useBrands, int[] incClients, int[] exClients, bool useCompanyPriceType, out Month21 month21YearBefore, out Month21 month21TwoYearsBefore, 
			out Month21 month21ThisYearStart, out Month21 month21ThisYearEnd, out Month21 currSalesDataMonth21To, out Month21 prevSalesDataMonth21From,
			out List<int> yearsForGraph, out List<List<SalesByMonth>> salesDataPeriods, bool? extendPreviousYearOverride = null)
		{
			month21YearBefore = Month21.FromDate(from) - 12;
			month21TwoYearsBefore = Month21.FromDate(from) - 24;
			month21ThisYearStart = Month21.FromDate(Utilities.GetYearStart(from));
			Month21 month21PreviousYearStart = month21ThisYearStart - 12;
			//Month21 month21TwoYearsBeforeStart = month21ThisYearStart - 24;
			month21ThisYearEnd = month21ThisYearStart + 11;
			Month21 month21PreviousYearEnd = month21PreviousYearStart + 11;
			//Month21 month21NextYearLimit = Month21.Now + 2;

			var salesDataTo = brandShipmentsDateMode == DateMode.YearFromNow ? Month21.Now : month21ThisYearEnd;
			if (from.Month >= Settings.Default.Analytics_NextYearThreshold)
				salesDataTo += monthsInFuture;

			var currSalesDataMonth21From = brandShipmentsDateMode == DateMode.YearFromNow ? month21YearBefore : month21ThisYearStart;
			currSalesDataMonth21To = salesDataTo;
			var extendPreviousYear = extendPreviousYearOverride ?? brandShipmentsDateMode != DateMode.YearFromNow && from.AddMonths(monthsInFuture).Year == from.Year;  //if future months in same year, show two years before
			prevSalesDataMonth21From = brandShipmentsDateMode != DateMode.YearFromNow ? month21PreviousYearStart - (extendPreviousYear ? 12 : 0) : month21TwoYearsBefore;
			yearsForGraph = new List<int>();
			var startYear = prevSalesDataMonth21From.Date.Year;
			yearsForGraph.Add(startYear);
			yearsForGraph.Add(startYear + 1);
			if(extendPreviousYear)
				yearsForGraph.Add(startYear + 2);

			var salesData = analyticsDAL.GetSalesByMonth(prevSalesDataMonth21From.Value, currSalesDataMonth21To.Value,
				countryFilter, brands: useBrands, includedClients: incClients, excludedClients: exClients,
				brand_id: brand_id, useCompanyPriceType: useCompanyPriceType, useETA: useCompanyPriceType);

			salesDataPeriods = new List<List<SalesByMonth>>();
			if (brandShipmentsDateMode == DateMode.YearFromNow)
			{
				salesDataPeriods.Add(salesData.Where(d => d.Month21 < currSalesDataMonth21From.Value).ToList());
				salesDataPeriods.Add(salesData.Where(d => d.Month21 >= currSalesDataMonth21From.Value).ToList());

			}
			else
			{
				foreach (var y in yearsForGraph)
				{
					var firstInYear = Month21.FromDate(new DateTime(y, 1, 1));
					salesDataPeriods.Add(salesData.Where(d => d.Month21.Between(firstInYear.Value, (firstInYear + 11).Value)).ToList());
				}
			}
		}

		private List<ReportSection> GetDefaultSections(List<string> excludedSections, List<string> includedSections )
        {
            var sections = new List<ReportSection>
			{
				new ReportSection
					{
						Name = "sales",
						Subsections = new List<ReportSection>(new[]
							{
								new ReportSection {Name = "brand shipments"},
								new ReportSection {Name = "brand container orders"},
								new ReportSection {Name = "container orders LCL"},
								new ReportSection {Name = "shipments by brand graph"},
                                new ReportSection {Name = "shipments by brand graph 2"},
								new ReportSection {Name = "brand shipments eta"},
								new ReportSection {Name = "shipments by customer"},
								new ReportSection {Name = "shipments by brand"},
								new ReportSection {Name = "order summary analysis"}
							})
					},
				new ReportSection
					{
						Name = "products",
						Subsections = new List<ReportSection>(new[]
							{
                                new ReportSection {Name="stock summary"},
                                new ReportSection {Name="top10byfactory"},
								new ReportSection {Name = "group c"},
                                new ReportSection {Name = "brochurerequests"}
							})
					},
				new ReportSection
					{
						Name = "feedbacks",
						Subsections = new List<ReportSection>(new[]
							{
								new ReportSection {Name = "uk claims analysis 12"},
								new ReportSection {Name = "uk claims analysis 6"},
                                new ReportSection {Name = "uk claims analysis 12 brands"},
                                new ReportSection {Name = "uk claims analysis 6 brands"},
								new ReportSection {Name = "uk claims decision"},
								new ReportSection {Name = "top 10 products"}
							})
					}
			};
			return FilterSections(sections, excludedSections, includedSections);
		}


        private List<ReportSection> GetDefaultSectionsV2(List<string> excludedSections, List<string> includedSections)
        {
            var sections = new List<ReportSection>
            {
                 new ReportSection
                    {
                        Name = "sales",
                        Subsections = new List<ReportSection>(new[]
                        {
							new ReportSection {Name = "brand shipments", Visible = false},
							new ReportSection {Name = "expected actual invoicing"},
                            new ReportSection {Name = "brand container orders"},
                            new ReportSection {Name = "invoicing by customer"},
							new ReportSection {Name = "shipments by customer", Visible = false},
							new ReportSection {Name = "invoicing by brand"},
                            new ReportSection {Name = "order summary analysis"},
                            new ReportSection {Name = "brand distributor sales", Visible = false}
                        })
                    },
                    new ReportSection
                    {
                        Name = "products",
                        Subsections = new List<ReportSection>(new []
                        {
                            new ReportSection{Name="live orders analysis"}
                        })
                    },
                    new ReportSection
                    {
                        Name = "feedbacks",
                        Subsections = new List<ReportSection>(new[]
                            {
                                new ReportSection {Name = "uk claims analysis 12"},
                                new ReportSection {Name = "uk claims analysis 12 brands"},
                                new ReportSection {Name = "uk claims decision"}
                            })
                    }
            };
            return FilterSections(sections, excludedSections, includedSections);
            
        }

        private static List<ReportSection> FilterSections(List<ReportSection> sections,  List<string> excludedSections, List<string> includedSections)
        {
            if (includedSections != null && includedSections.Count > 0) {
                foreach (var s in sections) {
                    if (includedSections.Contains(s.Name)) {
                        s.Visible = true;
                    }
                    else {
                        //var found = false;
                        foreach (var subsec in s.Subsections) {
                            if (includedSections.Contains(subsec.Name)) {
                                //found = true;
                                subsec.Visible = true;
                                s.Visible = true;
                            }
                        }
                       
                    }
                }
            }

            //remove excluded
            if (excludedSections.Count > 0) {
                var excluded = new List<ReportSection>();

                foreach (var s in sections) {
                    if (excludedSections.Contains(s.Name)) {
                        //excluded.Add(s);
                        s.Visible = false;
						foreach(var subsec in s.Subsections)
						{
							subsec.Visible = false;
						}
                    }
					else
					{
						foreach (var subsec in s.VisibleSections)
						{
							if (!excludedSections.Contains(subsec.Name))
							{
								continue;
							}
							//subsec.Parent = s;
							//excluded.Add(subsec);
							subsec.Visible = false;
						}
					}

				}

            }
            return sections;
        }

        private List<ReportSection> GetDefaultSectionsForBrandReport(List<string> excludedSections, List<string> includedSections, bool useOnlyIncluded = false)
        {
            var sections = new List<ReportSection>
            {
                new ReportSection
                    {
                        Name = "sales",
                        Subsections = new List<ReportSection>(new[]
                            {
                                new ReportSection {Name = "brand shipments"},
                                new ReportSection {Name = "sales by category"},
                                new ReportSection {Name = "shipments by customer"},
                                new ReportSection {Name = "claims analysis"},
                                new ReportSection {Name = "top10 selling"},
                                new ReportSection {Name = "top10 selling units"},
                                new ReportSection {Name = "sales orders", Visible = false },
                                new ReportSection {Name = "sales orders monthly", Visible = false }

								/*new ReportSection {Name = "sales by displays"} ,
								new ReportSection {Name = "non selling last 12m"}*/
							})
                    },
                new ReportSection
                    {
                        Name = "products",
                        Subsections = new List<ReportSection>(new []
                            {
                                new ReportSection {Name="option graph", Visible = false},
                                new ReportSection {Name = "sales by distributor"},
                                new ReportSection {Name = "sales by range"}                                
                            })
                    }
            };
            if(useOnlyIncluded) {
                foreach(var s in sections) {
                    s.Visible = false;
                    foreach(var sub in s.Subsections) {
                        sub.Visible = false;
                    }
                }
            }
            return FilterSections(sections, excludedSections, includedSections);

        }


        public ActionResult IndexV2(DateTime from, Guid statsKey, CountryFilter countryFilter = CountryFilter.UKOnly, ReportType type = ReportType.Brands, 
            string includedClients = "", string excludedClients = "", int? brand_id = null,DateMode brandContainersDateMode = DateMode.FromYearStart,
            bool separateUKForClaimsDecision = true,int? topNMinUnitsDelivered = 200, string excludedCustomers = "NK2,CWB,LK", int monthsHistoryForNewProducts = 6, 
            DateMode brandShipmentsDateMode = DateMode.FromYearStart, bool showNewProducts = true, bool increaseDecreaseSplit = true, string summaryDataPoints = "100,50",
            string excludedSections = "", string includedSections = "",string privateLabelDistributorsIds = "208,275,43", bool useCompanyPriceTypeForSales = false,
                string budgetBrands = "1,2,3,4,5,6,11", string startLateGroup = "C")
        {
            var new_from = from.AddDays(-6).Date;
            var new_to = from;
            ViewBag.from = new_from;
            ViewBag.to = new_to;
            var useBrands = type == ReportType.Brands;
            var incClients = !string.IsNullOrEmpty(includedClients)
                ? includedClients.Split(',').Select(int.Parse).ToArray()
                : null;
            var exClients = !string.IsNullOrEmpty(excludedClients)
                ? excludedClients.Split(',').Select(int.Parse).ToArray()
                : null;

            var plDistribs = !string.IsNullOrEmpty(privateLabelDistributorsIds)
                ? privateLabelDistributorsIds.Split(',').Select(int.Parse).ToArray()
                : null;

            var distributors = useBrands ? companyDAL.GetDistributors() : companyDAL.GetClients();
            
            List<Company> includedNonDistributors = new List<Company>();
            List<Company> privateLabelDistributors = new List<Company>();
            
            string included_distributors = Settings.Default.Analytics_IncludedNonDistributors;

            if (!string.IsNullOrEmpty(included_distributors))
            {
                includedNonDistributors =
                   companyDAL.GetByIds(Utilities.GetIdsFromString(included_distributors));
            }
            
            if(!string.IsNullOrEmpty(privateLabelDistributorsIds))
                privateLabelDistributors = companyDAL.GetByIds(Utilities.GetIdsFromString(privateLabelDistributorsIds));

            if (useBrands)
                distributors.AddRange(includedNonDistributors);

            var respondedClaims = analyticsDAL.GetRespondedClaims(Utilities.GetMonthFromDate(from, -24),
                    Utilities.GetMonthFromDate(from, -1), countryFilter, useBrands, incClients, exClients);

            var currentRespondedClaims =
                respondedClaims.Where(
                    r => r.cc_response_date >= Utilities.GetDateFromMonth21(Utilities.GetMonthFromDate(from, -12)))
                    .ToList();

            var previousRespondedClaims = respondedClaims.Where(
                r => r.cc_response_date < Utilities.GetDateFromMonth21(Utilities.GetMonthFromDate(from, -12)))
                .ToList();
            var chartKey = Guid.NewGuid();

            GenerateClaimsDecisionChart(currentRespondedClaims, 445, 360, chartKey, separatebbUK: separateUKForClaimsDecision);
            GenerateClaimsDecisionChart(previousRespondedClaims, 445, 360, chartKey, false, separatebbUK: separateUKForClaimsDecision);

            var topReturnedTo = from;
            var topReturnedFrom1 = from.AddMonths(-6);
            var topReturnedFrom2 = from.AddMonths(-12);

            var includedNonDistributorsIds = includedNonDistributors.Select(d => d.user_id).ToList();

            var exSections = excludedSections.Split(',').ToList();
            var incSections = includedSections.Split(',').ToList();

            var budgetBrandsIds = !string.IsNullOrEmpty(budgetBrands) ? budgetBrands.Split(',').Select(int.Parse).ToArray() : null;

            var model = new AnalyticsModel
            {
                ChartKey = chartKey,
                CountryFilter = countryFilter,
                ReportType = type,
                From = from,
                OrderStatsNew =
                       analyticsDAL.GetOrderProductGroupStats_New(new_from, new_to, countryFilter, useBrands,
                           incClients, exClients, includedNonDistributorsIds),
                OrderBrandStatsNew =
                        analyticsDAL.GetOrderBrandStats_New(new_from, new_to, countryFilter, useBrands, incClients,
                            exClients,includedNonDistributorsIds),
                OrderFactoryStatsNew =
                        analyticsDAL.GetOrderFactoryStats_New(new_from, new_to, countryFilter, useBrands, incClients,
                            exClients, includedNonDistributorsIds),
                OrderLocationStatsNew =
                        analyticsDAL.GetOrderLocationStats_New(new_from, new_to, countryFilter, useBrands,
                            incClients, exClients, includedNonDistributorsIds),
                OrderStatsETA = 
                       analyticsDAL.GetOrderProductGroupStats_ETA(new_from, new_to, countryFilter, useBrands,
                           incClients, exClients, includedNonDistributorsIds),
                OrderBrandStatsETA = 
                        analyticsDAL.GetOrderBrandStats_ETA(new_from, new_to, countryFilter, useBrands, incClients,
                            exClients, includedNonDistributorsIds),
                OrderFactoryStatsETA = 
                        analyticsDAL.GetOrderFactoryStats_ETA(new_from, new_to, countryFilter, useBrands, incClients,
                            exClients, includedNonDistributorsIds),
                OrderLocationStatsETA = 
                        analyticsDAL.GetOrderLocationStats_ETA(new_from, new_to, countryFilter, useBrands,
                            incClients, exClients, includedNonDistributorsIds),
                Sabc = sabcSortDal.GetAll(),
                Brands =  brandsDAL.GetAll(),
                SalesByCustomer = orderLineExportDal.GetCustomerSummaryForPeriod(from.AddYears(-1), from,countryFilter: countryFilter,brands: useBrands),
                Distributors = distributors,
                ReturnsSummaryByCustomer =
                        returnsDAL.GetTotalsPerClient(from.AddYears(-1), from, incClients: incClients,
                            exClients: exClients,countryFilter:countryFilter)
                            .Where(r => distributors.Count(d => d.customer_code == r.code) != 0)
                            .ToList(),
                PYSales =
                        orderLineExportDal.GetCustomerSummaryForPeriod(from.AddYears(-2),
                            from.AddYears(-1).AddDays(-1),countryFilter: countryFilter, brands: useBrands),
                PYSalesByBrand = GetBrandSummaryForPeriod(from.AddYears(-2), from.AddYears(-1), brands_only: useBrands),
                PYReturnsSummaryByCustomer =
                        returnsDAL.GetTotalsPerClient(from.AddYears(-2), from.AddYears(-1).AddDays(-1),
                            incClients: incClients, exClients: exClients,countryFilter:countryFilter)
                            .Where(r => distributors.Count(d => d.customer_code == r.code) != 0)
                            .ToList(),
                SalesByBrand =
                        GetBrandSummaryForPeriod(from.AddYears(-1), from, brands_only: useBrands),
                ReturnsSummaryByBrand =
                        GetTotalsPerBrand(from.AddYears(-1), from, incClients, exClients,countryFilter),
                PYReturnsSummaryByBrand =
                        GetTotalsPerBrand(from.AddYears(-2), from.AddYears(-1).AddDays(-1), incClients, exClients,countryFilter),
                

                Sections = GetDefaultSectionsV2(exSections, incSections),
                StatsKey =  statsKey,
                TopNMinUnitsDelivered = topNMinUnitsDelivered,
                ExpandPreviousForClaimsAnalysis =  true,
                IncludedNonDistributors =  includedNonDistributors,
                PrivateLabelDistributors = privateLabelDistributors,
				StartLateGroup = startLateGroup
            };

            model.OrdersInHand =
                analyticsDAL.GetOrderProductGroupStats_Out(DateTime.Today, new_from, new_to,
                    OutstandingOrdersMode.Production, countryFilter,
                    useBrands, incClients, exClients);


            model.OrdersInHand.AddRange(analyticsDAL.GetOrderProductGroupStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.ShippingInNextNDays, countryFilter,
                            useBrands, incClients, exClients));

            //need this grouped by customer only
            model.OrderCountClientProduction = model.OrdersInHand.GroupBy(o => new { o.client_code }).
                                                        Select(g =>
                                                                new OrderClientStat
                                                                {
                                                                    client_code = g.Key.client_code,
                                                                    ordersCount = g.Sum(c => c.orders_count),
                                                                    totalGPB = g.Sum(c => c.totalGPB)
                                                                }).OrderBy(f => f.client_code).ToList();
            
            //only transit
            List<OrderProductGroupStats> tmpList = analyticsDAL.GetOrderProductGroupStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.Transit, countryFilter,
                            useBrands, incClients, exClients, etaCriteriaFrom: WebUtilities.GetFirstDayOfWeek(from),
                            etaCriteriaTo: WebUtilities.GetFirstDayOfWeek(from).AddDays(6));

            model.OrderCountClientTransit = tmpList.GroupBy(o => new { o.client_code }).
                                                Select(g =>
                                                        new OrderClientStat
                                                        {
                                                            client_code = g.Key.client_code,
                                                            ordersCount = g.Sum(c => c.orders_count),
                                                            totalGPB = g.Sum(c => c.totalGPB)
                                                        })
                                                        .OrderBy(f => f.client_code).ToList();

            model.OrdersInHand.AddRange(tmpList);

            /*
            model.OrdersInHand.AddRange(AnalyticsDAL.GetOrderProductGroupStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.Transit, countryFilter,
                            useBrands, incClients, exClients, etaCriteriaFrom: WebUtilities.GetFirstDayOfWeek(from),
                            etaCriteriaTo: WebUtilities.GetFirstDayOfWeek(from).AddDays(6)));
            */
            
            model.OrderBrandStatsInHand = analyticsDAL.GetOrderBrandStats_Out(DateTime.Today, new_from, new_to,
                OutstandingOrdersMode.Production, countryFilter,
                useBrands, incClients, exClients);
            model.OrderBrandStatsInHand.AddRange(analyticsDAL.GetOrderBrandStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.ShippingInNextNDays, countryFilter,
                            useBrands, incClients, exClients));
            model.OrderBrandStatsInHand.AddRange(analyticsDAL.GetOrderBrandStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.Transit, countryFilter, useBrands,
                            incClients, exClients, etaCriteriaFrom: WebUtilities.GetFirstDayOfWeek(from),
                            etaCriteriaTo: WebUtilities.GetFirstDayOfWeek(from).AddDays(6)));

            model.OrderFactoryStatsInHand = analyticsDAL.GetOrderFactoryStats_Out(DateTime.Today, new_from, new_to,
                OutstandingOrdersMode.Production, countryFilter,
                useBrands, incClients, exClients);
            model.OrderFactoryStatsInHand.AddRange(analyticsDAL.GetOrderFactoryStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.ShippingInNextNDays, countryFilter,
                            useBrands, incClients, exClients));
            model.OrderFactoryStatsInHand.AddRange(analyticsDAL.GetOrderFactoryStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.Transit, countryFilter,
                            useBrands, incClients, exClients, etaCriteriaFrom: WebUtilities.GetFirstDayOfWeek(from),
                            etaCriteriaTo: WebUtilities.GetFirstDayOfWeek(from).AddDays(6)));

            model.OrderLocationStatsInHand = analyticsDAL.GetOrderLocationStats_Out(DateTime.Today, new_from, new_to,
                OutstandingOrdersMode.Production, countryFilter,
                useBrands, incClients, exClients);
            model.OrderLocationStatsInHand.AddRange(analyticsDAL.GetOrderLocationStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.ShippingInNextNDays, countryFilter,
                            useBrands, incClients, exClients));
            model.OrderLocationStatsInHand.AddRange(analyticsDAL.GetOrderLocationStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.Transit, countryFilter,
                            useBrands, incClients, exClients, etaCriteriaFrom: WebUtilities.GetFirstDayOfWeek(from),
                            etaCriteriaTo: WebUtilities.GetFirstDayOfWeek(from).AddDays(6)));

            model.dictProductChangeNotices = new Dictionary<int, List<Change_notice>>();
            

            var fromMonth21 = Month21.FromDate(new DateTime(from.Year - 1, 1, 1));
            var toMonth21 = Month21.FromDate(new DateTime(from.Year, 12, 1)).Value;

            model.BudgetActualData =
                GetBudgetActualDataByCriteria(fromMonth21.Value,toMonth21, countryFilter, excludedCustomers, budgetBrands: budgetBrands);

            if (privateLabelDistributors.Count > 0)
            {
                var privateLabelDistributorsCusCode = String.Join(",",
                    privateLabelDistributors.Select(m => m.customer_code));

                model.PrivateLabelDistributorsBudgetActualData =
                    GetBudgetActualDataByCriteria(fromMonth21.Value, toMonth21, countryFilter, "",
                        privateLabelDistributorsCusCode, budgetBrands);

                //orders in hand for private label distributors

                var pldOrders = 
                    analyticsDAL.GetOrderProductGroupStats_Out(DateTime.Today, new_from, new_to,
                        OutstandingOrdersMode.Production, countryFilter,
                        useBrands, plDistribs, null);


                pldOrders.AddRange(analyticsDAL.GetOrderProductGroupStats_Out(DateTime.Today, new_from, new_to,
                                OutstandingOrdersMode.ShippingInNextNDays, countryFilter,
                                useBrands, plDistribs, null));

                pldOrders.AddRange(analyticsDAL.GetOrderProductGroupStats_Out(DateTime.Today, new_from, new_to,
                            OutstandingOrdersMode.Transit, countryFilter,
                            useBrands, plDistribs, null, etaCriteriaFrom: WebUtilities.GetFirstDayOfWeek(from),
                            etaCriteriaTo: WebUtilities.GetFirstDayOfWeek(from).AddDays(6)));
                
                
                var grupedPldOrders = pldOrders.GroupBy(o => new { o.client_code }).
                                                            Select(g =>
                                                                    new OrderClientStat
                                                                    {
                                                                         client_code = g.Key.client_code,
                                                                        ordersCount = g.Sum(c => c.orders_count),
                                                                        totalGPB = g.Sum(c => c.totalGPB)
                                                                    }).OrderBy(f => f.client_code).ToList();

                model.PrivateLabelDistributorsOrderCount = grupedPldOrders;
            }

            var pldIds = model.PrivateLabelDistributors.Select(m => m.user_id).ToList();

            var privateLabelDistributorsData = model.BudgetActualData.Where(m => pldIds.Contains(Convert.ToInt32(m.distributor_id)) && m.distributor_id != null).ToList();

            var maxActualMonth = new Month21( model.BudgetActualData.Where(b => b.record_type == "A").Max(b => b.month21))+1;
            var intMaxActualMonth = maxActualMonth.Value;
            
            //grab estimated data from last actual data
            

            model.CurrentCustomerSalesData = analyticsDAL.GetCustomerSalesByMonth(fromDate:Month21.GetDate(intMaxActualMonth) ,toDate: from, countryFilter: countryFilter,
                brands: useBrands, includedClients: incClients, excludedClients: exClients, brand_id: brand_id,useETA:true,excludedCustomers:excludedCustomers,groupByMonth: false,
                includedNonDistributors:includedNonDistributorsIds,useCompanyPriceType: useCompanyPriceTypeForSales);

			if(model.Sections.SelectMany(s=>s.Subsections).Count(s=>s.Name == "shipments by customer" && s.Visible ) > 0)
			{
				model.PreviousCustomerSalesData = analyticsDAL.GetCustomerSalesByMonth(Utilities.GetMonthFromDate(from, -24),
						Utilities.GetMonthFromDate(from, -13), countryFilter: countryFilter, brands: useBrands,
						includedClients: incClients, excludedClients: exClients, brand_id: brand_id, groupByMonth: false);
			}

            model.PrivateLabelDistributorsCustomerSalesData = analyticsDAL.GetCustomerSalesByMonth(fromDate: Month21.GetDate(intMaxActualMonth), toDate: from, countryFilter: countryFilter,
                brands: useBrands,
                includedClients: plDistribs, excludedClients: null, brand_id: brand_id, useETA: true, excludedCustomers: string.Empty, groupByMonth: true, includedNonDistributors: null);

            model.CurrentBrandSalesData = analyticsDAL.GetBrandSalesByMonth(fromDate: Month21.GetDate(intMaxActualMonth), toDate: from,
                countryFilter:countryFilter,brands: useBrands,includedClients: incClients,excludedClients: exClients,useETA:true,excludedCustomers:excludedCustomers,groupByPeriod: true,
                useCompanyPriceType: useCompanyPriceTypeForSales);


            GenerateOrdersByMonthChart(900, 290, model.ChartKey, countryFilter: countryFilter, brands: useBrands,
                    includedClients: incClients, excludedClients: exClients, brand_id: brand_id, dateMode: brandContainersDateMode,useOrderDate:true,
                    excludedCustomersString:excludedCustomers,from: from, showPreviousYear: true,extendPreviousYearOverride:false);

			if (FindSection(model.Sections, "brand shipments"))
			{
				Month21 month21YearBefore, month21TwoYearsBefore, month21ThisYearStart, month21ThisYearEnd, currSalesDataMonth21To, prevSalesDataMonth21From;
				List<int> yearsForGraph;
				List<List<SalesByMonth>> salesDataPeriods;
				var monthsInFuture = 3;
				GetSalesByMonthData(from, countryFilter, brand_id, brandShipmentsDateMode, monthsInFuture,
					useBrands, incClients, exClients, useCompanyPriceTypeForSales, out month21YearBefore, out month21TwoYearsBefore, out month21ThisYearStart,
					out month21ThisYearEnd, out currSalesDataMonth21To, out prevSalesDataMonth21From, out yearsForGraph, out salesDataPeriods);
				var period3Data = salesDataPeriods.Count > 2 ? salesDataPeriods[2] : null;
				GenerateSalesByMonthChart(900, 290, salesDataPeriods[1], salesDataPeriods[0], chartKey, from, period3Data: period3Data, yearsForGraph: yearsForGraph);
			}
				
			if (FindSection(model.Sections, "expected actual invoicing"))
			{
				model.SalesByMonth = analyticsDAL.GetSalesByMonthNoView(intMaxActualMonth, null, countryFilter, useETA: true,
				excludedCustomers: excludedCustomers, includedNonDistributors: includedNonDistributorsIds, useCompanyPriceType: useCompanyPriceTypeForSales, brands: useBrands);
				GenerateExpectedActualInvoicingChart(model.ChartKey, fromMonth21, 900, 290, model.BudgetActualData, model.SalesByMonth, countryFilter, from);
			}
				

            model.OrdersAnalysis = GetLiveOrdersAnalysis(statsKey,model.ChartKey, from, countryFilter,excludedCustomers,
				monthsHistoryForNewProducts,includedNonDistributorsIds,showNewProducts,increaseDecreaseSplit,summaryDataPoints, useBrands, startLateGroup );

            model.DistributorSalesModel = FillDistributorSalesModel(from,countryFilter);
            
            return View("Index",model);
        }

        private DistributorBrandSalesModel FillDistributorSalesModel(DateTime from, CountryFilter countryFilter)
        {
            var month21From = Month21.FromDate(Utilities.GetYearStart(from.AddYears(-1)));
            var month21To = Month21.Now;
            var data = unitOfWork.DistSalesRepository.Get(ds => ds.month21 >= month21From.Value && ds.month21 <= month21To.Value).ToList();
            var brands = brandsDAL.GetAll();
            var distIds = data.Select(d => d.distributor_id).Distinct().ToList();
            return new DistributorBrandSalesModel
            {
                ForMonth = Month21.FromDate(from),
                Brands = data.Select(d => d.brand_id).Distinct().Select(d => brands.FirstOrDefault(b => b.brand_id == d)).ToList(),
                Distributors = unitOfWork.CompanyRepository.Get(c=>distIds.Contains(c.user_id)).ToList(),
                Sales = data,
                SalesInThousands = true
            };
        }

        [AllowAnonymous]
        public ActionResult DistributorSalesReport(Guid statsKey, DateTime from)
        {
            if(statsKey == new Guid(Settings.Default.StatsKey)) {
                return View("DistributorBrandSales", FillDistributorSalesModel(from, CountryFilter.UKOnly));
            }
            ViewBag.message = "No key";
            return View("Message");
        }

        public ActionResult DistributorSalesReportPdf(Guid statsKey, DateTime from, string options= "scale=0.78, leftmargin=22,rightmargin=22,media=1,Timeout=300,Landscape=True")
        {
            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("DistributorSalesReport", new { statsKey, from }), options,
                "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var fileName = "DistBrandSales.pdf";
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf", fileName);
        }

        public ActionResult ClaimsAnalysisPDF(Guid statsKey, DateTime from, DateTime? to = null,
            ReportType type = ReportType.Brands, string includedClients = "", string excludedClients = "")
        {
            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("ClaimsAnalysis", 
                new { statsKey, from, to, type, includedClients, excludedClients }), 
                "scale=0.70, leftmargin=22,rightmargin=22,media=1,Timeout=300",
                "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var fileName = string.Format(string.Format("ClaimsAnalysis_{0}.pdf",from.Year));
            var s = doc.SaveToMemory();
            return File((byte[])s, "application/pdf", fileName);
        }

        [AllowAnonymous]
        public ActionResult ClaimsAnalysis(Guid statsKey,DateTime from, DateTime? to = null, ReportType type = ReportType.Brands, string includedClients = "", string excludedClients = "")
        {
            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                if (to == null)
                {
                    to = @from.Year == DateTime.Today.Year ? DateTime.Today : @from.AddYears(1).AddSeconds(-1);
                }
                var distributors = type == ReportType.Brands ? companyDAL.GetDistributors() : companyDAL.GetClients();
                var useBrands = type == ReportType.Brands;
                var incClients = !string.IsNullOrEmpty(includedClients)
                    ? includedClients.Split(',').Select(int.Parse).ToArray()
                    : null;
                var exClients = !string.IsNullOrEmpty(excludedClients)
                    ? excludedClients.Split(',').Select(int.Parse).ToArray()
                    : null;
                var prevFrom = from.AddYears(-1);
                var prevTo = from.AddSeconds(-1);
                var model = new List<AnalyticsClaimsModel>();
                model.Add(new AnalyticsClaimsModel
                {
                    Distributors = distributors,
                    Brands = brandsDAL.GetAll(),
                    Sales = orderLineExportDal.GetCustomerSummaryForPeriod(from, to),
                    ReturnsSummary =  returnsDAL.GetTotalsPerClient(from, to, incClients: incClients,
                                exClients: exClients)
                                .Where(r => distributors.Count(d => d.customer_code == r.code) != 0)
                                .ToList() ,
                    PYSales = orderLineExportDal.GetCustomerSummaryForPeriod(prevFrom, prevTo) ,
                    PYReturnsSummary = returnsDAL.GetTotalsPerClient(prevFrom, prevTo, incClients: incClients, exClients: exClients)
                                .Where(r => distributors.Count(d => d.customer_code == r.code) != 0)
                                .ToList() ,
                    ExpandPreviousForClaimsAnalysis = true,
                    From = from,
                    To = to
                });
                model.Add(new AnalyticsClaimsModel
                {
                    Brands = brandsDAL.GetAll(),
                    Sales = orderLineExportDal.GetCustomerSummaryForPeriod(from, to),
                    ReturnsSummary = GetTotalsPerBrand(from, to, incClients: incClients,
                                exClients: exClients),
                    PYSales = GetBrandSummaryForPeriod(from, to, brands_only: useBrands),
                    PYReturnsSummary = GetTotalsPerBrand(prevFrom, prevTo, incClients: incClients, exClients: exClients),
                    ExpandPreviousForClaimsAnalysis = true,
                    From = from,
                    To = to
                });
                return View(model);    
            }
            ViewBag.message = "Wrong key";
            return View("Message");

        }

        [AllowAnonymous]
        public ActionResult AnalysisProductInfo(Guid statsKey, DateTime from, CountryFilter countryFilter, int? client_id = null, string excludedCustomersString = "NK2,CWB",
            bool excel = false, bool showFactory = false, bool separateNewProducts = false, int monthsHistoryForNewProducts = 6, bool showNewProducts = true, 
            bool increaseDecreaseSplit = true,string summaryDataPoints = "100,50", bool brands = true, string startLateGroup = "C" )
        {
            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                if (excel) {
                    Response.AddHeader("Content-Disposition", "attachment;filename=ProductAnalysis.xls");
                    Response.ContentType = "application/vnd.ms-excel";
                }
                return View(new AnalysisProductInfoModel
                {
                    OrdersAnalysisProductReport = GetLiveOrdersAnalysisProductInfo(from, countryFilter, client_id, excludedCustomersString, monthsHistoryForNewProducts,brands, startLateGroup ),
                    Brands = brandsDAL.GetAll(),
                    Client = client_id != null ? companyDAL.GetById(client_id.Value) : null,
                    From = from,
                    Factories = showFactory ? companyDAL.GetFactories() : null,
                    SeparateNewProducts = separateNewProducts,
                    ShowNewProducts = showNewProducts,
                    IncreaseDecreaseSplit = increaseDecreaseSplit,
                    SummaryDataPoints = Utilities.GetDoublesFromString(summaryDataPoints),
					DaysLeadTime = GetDaysLeadTime(sabcSortDal.GetAll(), startLateGroup)
                });    
            }
            ViewBag.message = "Invalid key";
            return View("Message");

        }

        

        private Dictionary<string,StockCodeProductCount> GenerateStockCodeProductCounts(DateTime dateFrom, List<Stock_code> stockCodes,List<Cust_products> products, ProductForecastingData forecastData, int stockWeekFrom, int stockWeekTo)
        {
            var result = new Dictionary<string, StockCodeProductCount>();
            var stockCodeIdDict = stockCodes.ToDictionary(sc => sc.stock_code_id);
            
            foreach (var p in products.Where(p=>p.cprod_stock_code > 0 && p.cprod_stock_date >= dateFrom.AddMonths(-1)))
            {
                if (p.cprod_stock_code != null)
                {
                    var key = stockCodeIdDict[p.cprod_stock_code.Value].stock_code_name;
                    if (!result.ContainsKey(key))
                        result[key] = new StockCodeProductCount();
                    result[key].ProductCount++;

                    var date = dateFrom;
                    //Compute stock on current date
                    var stockOnDate = ComputeStock(p.cprod_id, p.cprod_stock ?? 0, p.cprod_stock_date ?? DateTime.Today, date,forecastData,false);

                    for (int i = stockWeekFrom; i <= stockWeekTo; i++)
                    {
                        //Compute stock in period
                        var dateTo = dateFrom.AddDays(7*i);
                        
                        stockOnDate = ComputeStock(p.cprod_id, stockOnDate, date, dateTo,forecastData,i != 0);
                        if (stockOnDate < 0)
                        {
                            result[key].NegativeStockProductCount++;
                            Debug.Write(string.Format("{0}, ", p.cprod_code1));
                            break;
                        }
                        date = dateTo;
                    }    
                }
            }
            return result;
        }

        public static int ComputeStock(int cprod_id, int stock, DateTime dateFrom, DateTime dateTo,ProductForecastingData forecastData, bool breakOnNegative = true)
        {
            var result = stock;
            var arrived = Convert.ToInt32(
                forecastData.ArrivingLines.Where(l => l.cprod_id == cprod_id && l.Header.req_eta >= dateFrom && l.Header.req_eta <= dateTo)
                    .Sum(l => l.orderqty));
            result += arrived;

            var date = dateFrom;
            while (date < dateTo)
            {
                var month21 = Utilities.GetMonth21FromDate(date);
                var end = Utilities.Min(Utilities.GetMonthEnd(date), dateTo);
                var forecast = Convert.ToInt32((company.Common.Extensions.IfNotNull(forecastData.SalesForecasts.FirstOrDefault(sf => sf.cprod_id == cprod_id && sf.month21 == month21), sf=>sf.sales_qty) ?? 0) 
                        * ((end - date).TotalDays * 1.0 / (DateTime.DaysInMonth(date.Year, date.Month))));
                result -= forecast;
                if (result < 0 && breakOnNegative)
                    break;
                date = end.AddDays(1).Date;
            }
            return result;
        }


        private void GenerateExpectedActualInvoicingChart(Guid chartKey,Month21 from, int width, int height, List<BudgetActualData> budgetActualData, 
            List<SalesByMonth> estimatedData, CountryFilter countryFilter = CountryFilter.UKOnly, DateTime? reportDate = null)
        {
            var chart = new Chart { Width = width, Height = height };

            if (reportDate == null)
                reportDate = DateTime.Today;
            
            FormatChart(chart, "ETA", "GBP", showBudget: true,reportDate: reportDate );

            //estimation series
            chart.Series.Insert(2,new Series
            {
                MarkerStyle = MarkerStyle.Circle,
                Color = Settings.Default.Analytics_NextYearSeriesColor,
                MarkerColor = Color.White,
                MarkerBorderColor = Settings.Default.Analytics_NextYearSeriesColor,
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                LegendText = string.Format("{0} estimated",reportDate.Value.Year),
                //string.Format("{0}/{1}", DateTime.Today.Year - 2, DateTime.Today.Year - 1),
                XValueType = ChartValueType.Date
            });

            var month21NextYear = Month21.FromDate(Utilities.GetYearStart(reportDate.Value.AddYears(1)));
            var dataNextYear =
                estimatedData.Where(d => d.Month21 >= month21NextYear.Value)
                .GroupBy(d => d.Month21)
                    .Select(d => new { month21 = d.Key, total = d.Sum(da => da.Amount) })
                    .ToList();
            if (dataNextYear.Count > 0)
            {
                //if there is data for next year, add series
                chart.Series.Add(new Series
                {
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerColor = Color.White,
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 3,
                    LegendText = string.Format("{0} estimated", reportDate.Value.Year + 1),
                    //string.Format("{0}/{1}", DateTime.Today.Year - 2, DateTime.Today.Year - 1),
                    XValueType = ChartValueType.Date
                });
            }

            var data =
                budgetActualData.Where(b => b.record_type == "A" && b.distributor_id == null &&  b.month21 >= from.Value && b.month21 < (from + 12).Value 
                    && (b.ukflag == (countryFilter == CountryFilter.UKOnly ? (int?) 1 : null)) )
                    .GroupBy(b => b.month21)
                    .Select(g => new {month21=g.Key,total= g.Sum(b => b.value)})
                    .ToList();

            for (int i = 0; i < 12; i++)
            {
                double? total = 0.0;
                var value = data.FirstOrDefault(d => d.month21 == (from + i).Value);
                if (value != null)
                    total = value?.total;
                else {
                    total = estimatedData.FirstOrDefault(d => d.Month21 == (from + i).Value)?.Amount;                    
                }
                chart.Series[0].Points.AddXY((from + i).Date, total ?? 0);
            }

            data =
                budgetActualData.Where(b => b.record_type == "A" && b.distributor_id == null && b.month21 >= (from+12).Value && b.month21 < (from + 24).Value
                    && (b.ukflag == (countryFilter == CountryFilter.UKOnly ? (int?)1 : null)))
                    .GroupBy(b => b.month21)
                    .Select(g => new { month21 = g.Key, total = g.Sum(b => b.value) })
                    .ToList();
            var budgetData = budgetActualData.Where(b => b.record_type == "B" && b.distributor_id == null && b.month21 >= (from + 12).Value && b.month21 < (from + 24).Value
                    && (b.ukflag == (countryFilter == CountryFilter.UKOnly ? (int?)1 : null)))
                    .GroupBy(b => b.month21)
                    .Select(g => new { month21 = g.Key, total = g.Sum(b => b.value) })
                    .ToList();
            bool firstTime = true;
            double? lastActual = null;
            Month21 lastActualMonth21 = from;
            for (int i = 12; i < 24; i++)
            {
                var m21 = (from + i);
                var actual = data.FirstOrDefault(d => d.month21 == m21.Value);
                double? total = company.Common.Extensions.IfNotNull(actual, a=>a.total);
                var dateForChart = m21.Date.AddYears(-1);
                if (total == null)
                {
                    var estimation = estimatedData.FirstOrDefault(d => d.Month21 == m21.Value);
                    if (estimation != null)
                    {
                        if (firstTime)
                        {
                            //When first actual amount is null , this is used to connect line to estimation series
                            //chart.Series[1].Points.AddXY(dateForChart, estimation.Amount);
                            if (m21.Value - lastActualMonth21.Value == 1)
                                chart.Series[2].Points.AddXY(lastActualMonth21.Date.AddYears(-1), lastActual);
                            firstTime = false;
                        }
                        chart.Series[2].Points.AddXY(dateForChart, estimation.Amount);
                    }
                }
                else
                {
                    chart.Series[1].Points.AddXY(dateForChart, total ?? 0);
                    lastActual = total;
                    lastActualMonth21 = m21;
                }

                var budget = budgetData.FirstOrDefault(d => d.month21 == m21.Value);
                if(budget != null)
                    chart.Series[3].Points.AddXY(dateForChart, budget.total);

            }

            var j = 0;
            double? val;
            var seriesIndex = 4;
            while ((val = company.Common.Extensions.IfNotNull(dataNextYear.FirstOrDefault(d=>d.month21 == month21NextYear.Value + j), d=>d.total)) != null)
            {
                chart.Series[seriesIndex].Points.AddXY((month21NextYear + j).Date.AddYears(-2), val);
                j++;
            }

            SaveChartImage("Chart_ExpectedActual", chart, chartKey);
            
        }


        //private List<BudgetSummaryData> GetBudgetData(DateTime @from, DateTime to, int type)
        //{
        //    //get budget data for current year
        //    var mFrom = Utilities.GetMonth21FromDate(Utilities.GetYearStart(from));
        //    var mTo = Utilities.GetMonth21FromDate(to), type);
        //    var budgetData =
        //        unitOfWork.BudgetActualDataRepository.Get(b => (b.month21 >= mFrom || from == null) && (b.month21 <= mTo || to == null) && (b.data_type == type || type == null)).ToList();
        //    return
        //        budgetData.GroupBy(g => g.month21)
        //            .Select(g => new BudgetSummaryData {Month21 = g.Key, Amount = g.Sum(b => b.value)})
        //            .ToList();
        //}

        private string GetLogoFromClients(int[] incClients)
        {
            var result = string.Empty;
            foreach (var incClient in incClients)
            {
                var client = companyDAL.GetById(incClient);
                if (!string.IsNullOrEmpty(client.clientlogo))
                {
                    return client.clientlogo;
                }
            }
            return result;
        }

        [AllowAnonymous]
        public ActionResult Index2(DateTime from, Guid statsKey, CountryFilter countryFilter = CountryFilter.UKOnly)
        {

            Guid chartKey = Guid.NewGuid();

            if (statsKey == new Guid(Settings.Default.StatsKey2))
            {

                var brands = brandsDAL.GetAll();
                var top10from = Utilities.GetMonthFromNow(-6);
                var top10to = Utilities.GetMonthFromNow(0);

                return View(new AnalyticsModel
                {
                    Top10ByBrand = analyticsDAL.GetTopNByBrands(top10from, top10to, countryFilter),
                    Top10Universal =
                        analyticsDAL.GetTopNUniversal(Settings.Default.Analytics_BrandSales_NumOfProductsPerBrand,
                            top10from, top10to, countryFilter),
                    Top10By907 =
                        analyticsDAL.GetTopForBrandCat(Settings.Default.Analytics_BrandSales_NumOfProductsPerBrand,
                            top10from, top10to, Settings.Default.Analyitics_BrandSales_CleargreenTrays, countryFilter),
                    Top10By201 =
                        analyticsDAL.GetTopForBrandCat(Settings.Default.Analytics_BrandSales_NumOfProductsPerBrand,
                            top10from, top10to, Settings.Default.Analyitics_BrandSales_ClearwaterBaths, countryFilter),
                    Brands = brands,
                    CountryFilter = countryFilter,
                    ChartKey = chartKey
                });

            }
            else
            {
                ViewBag.message = "Invalid stats key";
                return View("Message");
            }
        }

        

        [AllowAnonymous]
        public ActionResult ForBrand(int brand_id, DateTime from, Guid statsKey,
            CountryFilter countryFilter = CountryFilter.UKOnly, string excludedSections = "",string includedSections = "",
            string includedClients = "", string excludedClients = "",
            string shippedFrom = null, bool showOptions = false, string brand_cat = "", int monthSpan = 6, bool includedSectionsOnly = false, 
            string excludedFactoriesSalesOrders="99", bool useBrandId = false, bool useSalesOrders = false, string salesOrdersWarehouse = "", int chartWidth = 900, 
            int chartHeight = 400, int historicalYears = 3, string brands = "CW", ProductAnalysisDateMode unitsSoldDateMode = ProductAnalysisDateMode.ETA )
        {
          Brand b = brandsDAL.GetById(brand_id);
            var chartKey = Guid.NewGuid();

            if (b != null)
            {
                
                if (statsKey == new Guid(Settings.Default.StatsKey))
                {
                    var exCustomers = Properties.Settings.Default.Analytics_ForBrands_ExcludedCustomers;

                    var incClients = !string.IsNullOrEmpty(includedClients)
                        ? includedClients.Split(',').Select(int.Parse).ToArray()
                        : null;
                    var exClients = !string.IsNullOrEmpty(excludedClients)
                        ? excludedClients.Split(',').Select(int.Parse).ToArray()
                        : null;


                    var currentSalesData = analyticsDAL.GetSalesByMonth(Utilities.GetMonthFromDate(from, -12),
                        Utilities.GetMonthFromDate(from, 2), countryFilter,
                        b.user_id, excludedCustomers:exCustomers);
                    var previousYearSalesData = analyticsDAL.GetSalesByMonth(Utilities.GetMonthFromDate(from, -24),
                        Utilities.GetMonthFromDate(from, -13),
                        countryFilter, b.user_id, excludedCustomers:exCustomers);

                    var excludedDistributors =
                        Settings.Default.Analytics_ExcludedDistributors.Split(',').Select(int.Parse);
                    var chartPrefix = b.code + "_";

                    var exSections = excludedSections.Split(',').ToList();
                    var inSections = includedSections.Split(',').ToList();
                    
                    
                    var distributors = companyDAL.GetDistributors();
                    var new_from = from.AddDays(-6).Date;
                    var new_to = from;
                    ViewBag.from = new_from;
                    ViewBag.to = new_to;

                    var PAModel = BuildProductAnalysisModel(shippedFrom, b, brand_cat, chartKey, showOptions,!useSalesOrders,countryFilter, excludedCustomers: exCustomers, 
                                useBrandId: useBrandId, useSalesOrders: useSalesOrders, dateMode: unitsSoldDateMode);
                    GenerateProductAnalysisTotalChart(
                        PAModel.SalesData.Where(s => useBrandId ? s.brand_id == b.brand_id : s.brand_user_id == b.user_id).ToList(), PAModel.Distributors.Count > 0 ? 450: 900 , 340, chartKey);
                    GenerateProductAnalysisDistributorsCharts(PAModel.Distributors,
                        PAModel.SalesData.Where(s => useBrandId ? s.brand_id == b.brand_id : s.brand_user_id == b.user_id).ToList(), 150, 170, chartKey);

                    //if (b.brand_id != Brand.AquaCabinets)
                    //{
                    //    exSections.Add("option graph");
                    //}

                    var model = new AnalyticsModel
                    {
                        CurrentSalesData = currentSalesData,
                        PreviousYearSalesData = previousYearSalesData,
                        CurrentCustomerSalesData =
                            !excludedSections.Contains("shipments by customer")
                                ? analyticsDAL.GetCustomerSalesByMonth(Utilities.GetMonthFromDate(from, -1*monthSpan),
                                    Utilities.GetMonthFromDate(from, -1), true, countryFilter, b.user_id, excludedCustomers:exCustomers)
                                : null,
                        PreviousCustomerSalesData =
                            !excludedSections.Contains("shipments by customer")
                                ? analyticsDAL.GetCustomerSalesByMonth(Utilities.GetMonthFromDate(from, -2*monthSpan),
                                    Utilities.GetMonthFromDate(from, -1*monthSpan - 1), true, countryFilter,
                                    b.user_id, excludedCustomers: exCustomers)
                                : null,
                        Brand = b,
                        Brands = brandsDAL.GetAll(),
                        Distributors = distributors,
                        SalesByCustomer =
                            orderLineExportDal.GetCustomerSummaryForPeriod(from.AddYears(-1), from, b.user_id, countryFilter: countryFilter,excludedCustomers:exCustomers),
                        ReturnsSummaryByCustomer =
                            returnsDAL.GetTotalsPerClient(from.AddYears(-1), from, b.user_id, countryFilter: countryFilter, excludedCustomers : exCustomers)
                                .Where(r => distributors.Count(d => d.customer_code == r.code) != 0)
                                .ToList(),
                        PYSales =
                            orderLineExportDal.GetCustomerSummaryForPeriod(from.AddYears(-2),
                                from.AddYears(-1).AddDays(-1), b.user_id, countryFilter: countryFilter, excludedCustomers: exCustomers),
                        PYReturnsSummaryByCustomer =
                            returnsDAL.GetTotalsPerClient(from.AddYears(-2), from.AddYears(-1).AddDays(-1), b.user_id,countryFilter:countryFilter)
                                .Where(r => distributors.Count(d => d.customer_code == r.code) != 0)
                                .ToList(),
                        CurrentProductSalesData =
                            analyticsDAL.GetProductSales(Utilities.GetMonthFromDate(from, -1*monthSpan),
                                Utilities.GetMonthFromDate(from, -1), countryFilter, b.user_id,
                                ignoreFactoryCode: true, excludedCustomers: exCustomers.Split(',').ToList()),
                        PreviousProductSalesData =
                            analyticsDAL.GetProductSales(Utilities.GetMonthFromDate(from, -2*monthSpan),
                                Utilities.GetMonthFromDate(from, -1*monthSpan - 1), countryFilter, b.user_id,
                                ignoreFactoryCode: true, excludedCustomers: exCustomers.Split(',').ToList()),
                        NonSelling = analyticsDAL.GetNonSelling(Utilities.GetMonthFromDate(from, -12),
                            Utilities.GetMonthFromDate(from, -1), b.user_id),
                        ChartKey = chartKey,
                        ExcludedSections = exSections,
                        AnalyticsSubCategories = analyticsSubcategoryDal.GetForBrand(brand_id),
                        AnalyticsOptions = analyticsOptionDal.GetAll(),
                        AnalyticsCategorySummaries =
                            analyticsDAL.GetAnalyticsCategorySummary(Utilities.GetMonthFromDate(from, -6),
                                Utilities.GetMonthFromDate(from, -1), countryFilter, true, incClients, exClients, excludedCustomers:exCustomers),
                        ProductAnalysisModel = PAModel,
                        MonthSpan = monthSpan,
                        From = from,
                        Sections = GetDefaultSectionsForBrandReport(exSections, inSections, includedSectionsOnly),
                        ReportCurrency = useSalesOrders ? "USD" : "GBP",
                        ShortNotes = useSalesOrders
                    };

                    if(FindSection(model.Sections, "brand shipments"))
                        GenerateSalesByMonthChart(900, 290, currentSalesData, previousYearSalesData, chartKey, from,chartPrefix);
                    exSections.Add("sales by displays");                    

                    if(FindSection(model.Sections, "sales by category")) {
                        GenerateSalesByCategoryChart(445, 360, chartKey, true, b, countryFilter);
                        GenerateSalesByCategoryChart(445, 360, chartKey, false, b, countryFilter);
                    }
                    
                    if(FindSection(model.Sections, "sales orders monthly")) {
                        //changed salesOrdersWarehouse for brands
                        model.SalesOrdersMonthlyModel = BuildSalesOrdersMonthlyReportModel(null, salesOrdersWarehouse, Utilities.GetYearStart(from.AddYears(-1*historicalYears)),brand_id,brands);
                        var chart = GenerateSalesOrdersMonthlyChart(model.SalesOrdersMonthlyModel, chartWidth, chartHeight);
                        ViewBag.ChartKey = Guid.NewGuid();
                        SaveChartImage("SalesOrdersMonthlyReport", chart, ViewBag.ChartKey);
                    }


                    return View("Brand", model);
                }
                else
                {
                    ViewBag.message = "Invalid stats key";
                    return View("Message");
                }
            }
            return null;
        }

        private bool FindSection(List<ReportSection> sections, string name)
        {
            return sections.Any(s => s.VisibleSections.Count(sub => sub.Name == name) > 0);
        }

        public ActionResult Universal(DateTime from, Guid statsKey,
            CountryFilter countryFilter = CountryFilter.UKOnly, string excludedSections = "sales,option graph,sales by distributor,dealer displays summary,top20 displaying",
            string includedClients = "", string excludedClients = "",
            string shippedFrom = null, bool showOptions = false, string brand_cat = "", int monthSpan = 6)
        {
            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                var PAModel = BuildProductAnalysisModel(shippedFrom, null, brand_cat, Guid.NewGuid(), showOptions, true,
                countryFilter);
                
                var new_from = from.AddDays(-6).Date;
                var new_to = from;
                ViewBag.from = new_from;
                ViewBag.to = new_to;

                var exSections = excludedSections.Split(',').ToList();

                var model = new AnalyticsModel
                {
                    Sections = GetDefaultSectionsForBrandReport(exSections, null, false),
                    ProductAnalysisModel = PAModel,
                    AnalyticsSubCategories = analyticsSubcategoryDal.GetForBrand(nullBrandOnly:true),
                    From = from
                };
                return View("Brand", model);
            }
            ViewBag.message = "Invalid stats key";
            return View("Message");
        }

        public ActionResult ProductAnalysis(string shippedFrom, int brand_id, bool showOptions = false,
            string brand_cat = "")
        {
            var brand = brandsDAL.GetById(brand_id);
            var chartKey = Guid.NewGuid();
            var analyticOptions = analyticsOptionDal.GetAll();
            if (brand != null)
            {
                var model = BuildProductAnalysisModel(shippedFrom, brand, brand_cat, chartKey, showOptions);
                GenerateProductAnalysisTotalChart(
                    model.SalesData.Where(s => s.brand_user_id == brand.user_id).ToList(), 450, 340, chartKey);
                GenerateProductAnalysisDistributorsCharts(model.Distributors,
                    model.SalesData.Where(s => s.brand_user_id == brand.user_id).ToList(), 150, 170, chartKey);
                return View(model);
            }
            ViewBag.message = "Illegal parameter brand_id";
            return View("Message");
        }

        private ProductAnalysisModel BuildProductAnalysisModel(string shippedFrom, Brand brand, string brand_cat,
            Guid chartKey, bool showOptions, bool useETAForSales = false, CountryFilter countryFilter = CountryFilter.UKOnly,
	        string excludedCustomers = "NK2,CWB", bool useSalesOrders = false, bool useBrandId = false,
	        ProductAnalysisDateMode dateMode = ProductAnalysisDateMode.ETA)
        {
            DateTime dateShipped = GetStartDate(shippedFrom);
            var model = new ProductAnalysisModel
            {
                Brand = brand,
                ChartKey = chartKey.ToString(),
                SalesData = !useSalesOrders ? 
                              brandSalesAnalysis2DAL.GetForBrand(brand?.user_id, dateShipped,useETAForSales,countryFilter, excludedCustomers, 
	                              dateMode == ProductAnalysisDateMode.ETA ? OrderDateMode.Eta : dateMode == ProductAnalysisDateMode.ETD ? OrderDateMode.Etd : OrderDateMode.OrderDate) :
                               GetSalesFromSalesOrders(brand, useBrandId, dateShipped)
                                ,
                StartDate = dateShipped,
                Subcategories = analyticsSubcategoryDal.GetAll(),
                UseEtaForSales = useETAForSales,
				DateMode = dateMode
            };
            if (!string.IsNullOrEmpty(brand_cat))
                model.brand_cats = brand_cat.Split(',').Select(int.Parse).ToList();
            model.Categories = analyticsCategoryDal.GetForBrand(brand?.user_id); 
            model.CustProducts = custproductsDAL.GetProductsForAnalyticsCategories(brand?.user_id, useSalesOrders);
            if (useBrandId)
                model.CustProducts = model.CustProducts.Where(p => p.brand_id == brand.brand_id).ToList();
            model.ProductDisplayCounts = new List<ProductDistributorDisplayCount>();

            model.Distributors =
                model.SalesData.Where(s => (brand == null || s.brand_user_id == brand.user_id) && s.user_country.In("GB", "UK", "IE"))
                    .GroupBy(s => s.customer_code)
                    .Select(
                        s =>
                            new Company
                            {
                                customer_code = s.Key,
                                user_name = s.First().client_name,
                                user_id = s.First().userid1 ?? 0
                            })
                    .ToList();
            

            model.Options = showOptions
                ? model.SalesData.Where(s => (brand == null || s.brand_user_id == brand.user_id) && s.analytics_option != null)
                    .GroupBy(s => s.analytics_option)
                    .Select(s => s.First().Option)
                    .OrderBy(s => s.option_id)
                    .ToList()
                : new List<Analytics_options>();

            return model;
        }

        private List<Brand_sales_analysis2> GetSalesFromSalesOrders(Brand brand, bool useBrandId, DateTime dateShipped)
        {
            var id = useBrandId ? brand.brand_id : brand.user_id;
            var options = analyticsOptionDal.GetAll();
            var sales = useBrandId ? unitOfWork.SalesOrdersRepository.Get(s => s.date_entered >= dateShipped && s.Product.brand_id == id, includeProperties: "Product").ToList() :
                unitOfWork.SalesOrdersRepository.Get(s => s.date_entered >= dateShipped && s.Product.brand_userid == id, includeProperties: "Product").ToList();
            return sales.Select(s => new Brand_sales_analysis2
            {
                cprod_id = s.cprod_id,
                cprod_code1 = s.Product?.cprod_code1,
                customer_code = s.customer,
                analytics_option = s.Product?.analytics_option,
                analytics_category = s.Product?.analytics_category,
                Option = options.FirstOrDefault(o=>o.option_id == s.Product?.analytics_option),
                rowprice_gbp = s.value,
                po_req_etd = s.date_entered,
                req_eta = s.date_entered,
                req_eta_nooffset = s.date_entered,
                orderqty = s.order_qty,
                brand_id = brand.brand_id,
                brand_user_id = brand.user_id,
                user_country = string.Empty
            }).ToList();
        }

        public DateTime GetStartDate(string shippedFrom = null)
        {
            DateTime dateShipped = DateTime.Today.AddYears(-1);
            if (!string.IsNullOrEmpty(shippedFrom))
            {
                if (shippedFrom.StartsWith("T"))
                {
                    var sOffset = shippedFrom.Substring(1);
                    int offset;
                    if (int.TryParse(sOffset, out offset))
                    {
                        dateShipped = DateTime.Today.AddDays(offset);
                    }
                }
                else
                {
                    DateTime.TryParse(shippedFrom, out dateShipped);
                }
            }
            return dateShipped;
        }

        private void GenerateProductAnalysisTotalChart(List<Brand_sales_analysis2> SalesData, int width, int height,
            Guid chartKey)
        {
            var chart = new Chart {Width = width, Height = height};

            var area = new ChartArea();
            area.BackColor = Color.FromArgb(0xEF, 0xEF, 0xEF);
            //area.Area3DStyle.Enable3D = true;
            chart.ChartAreas.Add(area);

            var series = new Series
            {
                ChartType = SeriesChartType.Pie
            };
            series["PieLabelStyle"] = "Outside";
            series["PieLineColor"] = "Transparent";
            series.Label = "#AXISLABEL #PERCENT{P1}";
            foreach (var data in SalesData.Where(d => d.analytics_option != null).GroupBy(d => d.analytics_option))
            {
                var point = new DataPoint();
                var option = data.First().Option;
                point.SetValueXY(option.option_name, new object[] {data.Sum(d => d.orderqty)});
                point.IsValueShownAsLabel = true;
                point.Color = ColorFromOption(option);
                point.LegendText = "#VALX #PERCENT{P1}";
                series.Points.Add(point);
            }
            chart.Series.Add(series);

            SaveChartImage("Chart_ByColorTotal", chart, chartKey);
        }

        

        private Color ColorFromOption(Analytics_options option)
        {
            var result = Color.Empty;
            if (option != null && !string.IsNullOrEmpty(option.color_code))
            {
                return ColorTranslator.FromHtml("#" + option.color_code);
            }
            return result;

        }

        private void GenerateProductAnalysisDistributorsCharts(List<Company> distributors,
            List<Brand_sales_analysis2> salesData, int width, int height, Guid chartKey)
        {

            foreach (var dist in distributors)
            {
                var chart = new Chart {Width = width, Height = height};

                var area = new ChartArea();
                //area.Area3DStyle.Enable3D = true;
                chart.ChartAreas.Add(area);
                area.BackColor = Color.FromArgb(0xEF, 0xEF, 0xEF);
                chart.BackColor = Color.FromArgb(0xEF, 0xEF, 0xEF);

                var series = new Series
                {
                    ChartType = SeriesChartType.Pie
                };
                series["PieLabelStyle"] = "Disabled";
                foreach (
                    var data in
                        salesData.Where(s => s.analytics_option != null && s.customer_code == dist.customer_code)
                            .GroupBy(d => d.analytics_option))
                {
                    var point = new DataPoint();
                    var option = data.First().Option;
                    point.SetValueXY(data.Key, new object[] {data.Sum(d => d.rowprice_gbp)});
                    point.IsValueShownAsLabel = false;
                    point.Color = ColorFromOption(option);
                    //point.LegendText = "#VALX #PERCENT{P1}";
                    series.Points.Add(point);
                }
                chart.Series.Add(series);
                chart.Titles.Add(dist.user_name);
                var font = chart.Titles[0].Font;
                chart.Titles[0].Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);

                SaveChartImage(string.Format("Chart_ByColor_{0}", dist.customer_code), chart, chartKey);
            }
        }

        private void GenerateSalesByDisplaysChart(int width, int height, Guid chartKey)
        {
            var sales = analyticsDAL.GetSalesForCategory(201);
            var displayCounts = analyticsDAL.GetDisplayCountForProducts(201);

            foreach (var productDisplayCount in displayCounts)
            {
                var sale = sales.FirstOrDefault(s => s.cprod_id == productDisplayCount.cprod_id);
                if (sale != null)
                    sale.numOfUnits = productDisplayCount.DisplayCount;
            }

            var chart = new Chart {Width = width, Height = height};

            chart.Customize += (sender, args) =>
            {
                foreach (var label in ((Chart) sender).ChartAreas[0].AxisX.CustomLabels)
                {
                    label.Text = WebUtilities.KiloFormat(double.Parse(label.Text));
                }
            };

            var area = new ChartArea();
            area.AxisY.IsInterlaced = true;
            area.AxisY.InterlacedColor = Color.FromArgb(0xEE, 0xEE, 0xEE);

            area.AxisX.Title = "sales";
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisX.Interval = 200000;


            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.Title = "no. of displays";

            chart.ChartAreas.Add(area);


            var series = new Series
            {
                MarkerStyle = MarkerStyle.Circle,
                Color = Color.Orange,
                ChartType = SeriesChartType.Point,
                BorderWidth = 3,
                XValueType = ChartValueType.Double,
                Name = "series"
            };
            chart.Series.Add(series);

            var trendseries = new Series
            {
                Color = Color.Blue,
                ChartType = SeriesChartType.Line,
                Name = "trend"
            };
            chart.Series.Add(trendseries);

            foreach (var sale in sales.Where(s => s.numOfUnits > 0).OrderBy(s => s.Amount))
            {
                series.Points.AddXY(sale.Amount, sale.numOfUnits);
            }

            chart.DataManipulator.FinancialFormula(FinancialFormula.Forecasting,
                "Linear,3,false,false", series, trendseries);

            SaveChartImage("Chart_SalesByDisplay", chart, chartKey);

        }

        private string GenerateProdLocationPdf( /*List<ProductLocationStatsSummary> productLocationStatSummary*/)
        {
            var fileName = string.Format("products_by_location_{0}.pdf", DateTime.Today.ToString("ddMMyy"));
#if !DEBUG
            IPdfManager pdf = new PdfManager();
            var doc = pdf.CreateDocument();
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("ProdByLocation", 
            new { statsKey = Settings.Default.StatsKey }), "scale=0.78, leftmargin=22,rightmargin=22,media=1",
            "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            var s = doc.Save(Path.Combine(Server.MapPath(Settings.Default.Analytics_ImagesFolder), fileName), true);
            
#endif
            return fileName;
        }

        [AllowAnonymous]
        public ActionResult ProdByLocation(Guid statsKey, int? ageForExclusion = 6, string distCountries = "GB,IE")
        {
            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                var countries = distCountries.Split(',').ToList();
                return View("ProductByLocationList",
                    new ProductLocationModel
                    {
                        ProductLocationStats = analyticsDAL.GetProductLocationStats("C", ageForExclusion, countries)
                        /*,AlternateProducts = AnalyticsDAL.GetAlternateProductsStats("C")*/
                    });
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        public ActionResult Home()
        {

            var result = new ReportHomeModel
            {
                FactoriesBySales =
                    analyticsDAL.GetSalesOfOrdersByMonthForFactories(Utilities.GetMonthFromNow(-6),
                        Utilities.GetMonthFromNow(-1))
            };
            result.FactoriesLines = new List<FactoryLine>();
            //double am = 0.0;
            foreach (
                var line in result.FactoriesBySales.Where(f => f.combined_factory > 0).GroupBy(f => f.combined_factory))
            {
                var idFactory = string.Join(",", line.Select(f => f.user_id));
                var factoryCode = string.Join(",", line.Select(c => c.factoryCode));
                var am = line.Select(f => f.Amount).Sum() ?? 0;
                result.FactoriesLines.Add(new FactoryLine
                {
                    IdFactory = idFactory,
                    Name = factoryCode,
                    Suma = am
                });
            }
            foreach (var s in result.FactoriesBySales.Where(f => f.combined_factory == default(int)))
            {
                result.FactoriesLines.Add(new FactoryLine
                {
                    IdFactory = s.user_id.ToString(),
                    Name = s.factoryCode,
                    Suma = s.Amount ?? 0
                });
            }
            return View("ReportHome", new ReportHomeModel
            {
                Brands = brandsDAL.GetAll(),
                Factories =
                    companyDAL.GetFactories(),
                FactoriesLines = result.FactoriesLines,
                //AnalyticsDAL.GetSalesOfOrdersBy6MonthForFactories(WebUtilities.GetMonthFromNow(-6), WebUtilities.GetMonthFromNow(-1)),

            });
        }

        [AllowAnonymous]
        public ActionResult GetFile(Guid statsKey, string file)
        {
            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                return File(Path.Combine(Server.MapPath(Settings.Default.Analytics_ImagesFolder), file),
                    "application/pdf");
            }
            else
            {
                return null;
            }
        }

        //public ActionResult Chart_SalesByMonth(string param)
        private
            void GenerateSalesByMonthChart(int width, int height, List<SalesByMonth> period2Data,
                List<SalesByMonth> period1Data, Guid chartKey, DateTime from, string chartNamePrefix = "",
                string xaxisTitle = "ETD", bool subtractSpecial = false, List<BudgetSummaryData> budgetData = null,List<SalesByMonth> period3Data = null, string chartNameSuffix = "", IList<int> yearsForGraph = null, DateTime? reportDate = null )
        {
            if (reportDate == null)
                reportDate = DateTime.Today;
            //var jsSerializer = new JavaScriptSerializer();
            //var paramObj = (Dictionary<string,object>) jsSerializer.DeserializeObject(param);

            var chart = new Chart {Width = width, Height = height};
            var nextYearLimit = (from.Year + 1 - 2000) * 100;

            var yearDiff = period1Data.Max(d => d.Month21/100) - period1Data.Min(d => d.Month21)/100 ;

            FormatChart(chart, xaxisTitle, "GBP", showBudget: budgetData != null,
                 showThirdPeriod: period3Data != null, yearSplit: yearDiff > 0, years: yearsForGraph, reportDate: reportDate);


            //var currdata = AnalyticsDAL.GetSalesByMonth(WebUtilities.GetMonthFromNow(-12),
            //                                            WebUtilities.GetMonthFromNow(2));
            //var previousData = AnalyticsDAL.GetSalesByMonth(WebUtilities.GetMonthFromNow(-24),
            //                                                WebUtilities.GetMonthFromNow(-13));

            
            var currMonth = Utilities.GetMonthFromNow(0);
            DataPoint anchorPoint = null;

            foreach (var part in period2Data.OrderBy(d => d.Month21))
            {
                if (part != null)
                {
                    chart.Series[1].Points.AddXY(Utilities.GetDateFromMonth21(part.Month21),
                        part.Amount - (subtractSpecial ? part.SpecialAmount : 0));
                    chart.Series[1].Points.Last().IsValueShownAsLabel = true;
                    if (part.Month21 == currMonth)
                        anchorPoint = chart.Series[1].Points.Last();
                }
            }

            foreach (var part in period1Data.OrderBy(d => d.Month21))
            {
                if (part != null)
                {
                    //add year to actual date to show both series under same X axis
                    chart.Series[0].Points.AddXY(Utilities.GetDateFromMonth21(part.Month21).AddYears(1),
                        part.Amount - (subtractSpecial ? part.SpecialAmount : 0));
                    chart.Series[0].Points.Last().IsValueShownAsLabel = true;
                }
            }

            if (budgetData != null)
            {
                foreach (var part in budgetData.OrderBy(d => d.Month21))
                {
                    if (part != null)
                    {
                        
                        chart.Series[2].Points.AddXY(Utilities.GetDateFromMonth21(part.Month21), part.Amount);
                        chart.Series[2].Points.Last().IsValueShownAsLabel = true;
                    }
                }
            }

            if (period3Data != null)
            {
                foreach (var part in period3Data.OrderBy(d => d.Month21))
                {
                    if (part != null)
                    {
                        chart.Series.Last().Points.AddXY(Utilities.GetDateFromMonth21(part.Month21).AddYears(-1),
                            part.Amount - (subtractSpecial ? part.SpecialAmount : 0));
                        chart.Series.Last().Points.Last().IsValueShownAsLabel = true;
                    }
                }
            }

            AddAnnotation(chart, anchorPoint);

            //return File(StreamChart(chart), "image/jpg");
            SaveChartImage(string.Format("{0}Chart_SalesByMonth{1}", chartNamePrefix,chartNameSuffix), chart, chartKey);

        }

        

        /*private void SaveChartImage(string name, Chart chart, Guid chartKey)
        {
            chart.SaveImage(
                Path.Combine(Server.MapPath(Settings.Default.Analytics_ImagesFolder),
                    string.Format("{0}_{1}.jpg", chartKey, name)),
                ChartImageFormat.Jpeg);
        }*/

        private void AddAnnotation(Chart chart, DataPoint point = null)
        {
            var line = new VerticalLineAnnotation
            {
                LineDashStyle = ChartDashStyle.Dot,
                AxisX = chart.ChartAreas[0].AxisX,
                AxisY = chart.ChartAreas[0].AxisY,
                ClipToChartArea = chart.ChartAreas[0].Name,
                IsInfinitive = true,
                LineWidth = 1,
                LineColor = Color.FromArgb(0xCC, 0xCC, 0xCC)
            };
            if (point == null)
            {
                if (chart.Series[1].Points.Count >= 3)
                {
                    line.SetAnchor(chart.Series[1].Points[chart.Series[1].Points.Count - 3]);
                }
            }
            else
            {
                line.SetAnchor(point);
            }
            chart.Annotations.Add(line);
        }

        private void FormatChart(Chart chart, string xTitle, string yTitle, bool customize = true,
            bool showBudget = false, bool showThirdPeriod = false, bool yearSplit = false, bool nextYear = false, IList<int> years = null, DateTime? reportDate = null, int seriesIndexForLabel = 1)
        {
            if (reportDate == null)
                reportDate = DateTime.Today;
            var colors = new Dictionary<int?, Color>
            {
                {reportDate.Year() - 1, Settings.Default.Analytics_PreviousYearSeriesColor},
                {reportDate.Year(), Settings.Default.Analytics_CurrentYearSeriesColor},
                {reportDate.Year() + 1, Settings.Default.Analytics_NextYearSeriesColor}
            };
            if (customize)
            {
                chart.Customize += (sender, args) =>
                {
                    var c = (Chart) sender;
                    var i = 0;
                    foreach (var point in c.Series[seriesIndexForLabel].Points)
                    {
                        if (c.Series.Count > (showBudget ? 3 : 2) && i < c.Series[showBudget ? 3 : 2].Points.Count)
                            point.IsValueShownAsLabel = false;
                        else
                            point.Label = WebUtilities.KiloFormat(point.YValues[0]);
                        i++;
                    }
                    if (seriesIndexForLabel != 0) {
                        foreach (var point in c.Series[0].Points) {
                            point.IsValueShownAsLabel = false;
                        }
                    }
                    var index = showBudget ? 2 : 1;
                    if (c.Series.Count > index && index != seriesIndexForLabel)
                    {
                        foreach (var point in c.Series[index].Points)
                        {
                            point.IsValueShownAsLabel = false;
                        }
                    }
                    index = showBudget ? 3 : 2;
                    if (c.Series.Count > index)
                    {
                        foreach (var point in c.Series[index].Points)
                        {
                            point.Label = WebUtilities.KiloFormat(point.YValues[0]);
                        }
                    }
                    foreach (var label in c.ChartAreas[0].AxisY.CustomLabels)
                    {
                        label.Text = WebUtilities.KiloFormat(double.Parse(label.Text));
                    }
                };
            }
            var area = new ChartArea();
            area.AxisY.IsInterlaced = true;
            area.AxisY.InterlacedColor = Color.FromArgb(0xEE, 0xEE, 0xEE);

            area.AxisX.Title = xTitle;
            area.AxisX.IntervalType = DateTimeIntervalType.Months;
            area.AxisX.Interval = 1;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;

            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.Title = yTitle;


            area.AxisX.LabelStyle.Format = "MMM";

            chart.ChartAreas.Add(area);

            var year = years != null && years.Count > 0 ? years[0] : reportDate.Value.Year - 1;
            var series = new Series
            {
                MarkerStyle = MarkerStyle.Circle,
                Color = colors.ContainsKey(year) ? colors[year] : Color.Empty,
                MarkerColor = Color.White,
                MarkerBorderColor = Settings.Default.Analytics_PreviousYearSeriesColor,
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                LegendText = FormatLegendText(year,yearSplit),
                //string.Format("{0}/{1}", DateTime.Today.Year - 2, DateTime.Today.Year - 1),
                XValueType = ChartValueType.Date
            };
            chart.Series.Add(series);

            year = years != null && years.Count > 1 ? years[1] : reportDate.Value.Year;
            series = new Series
            {
                MarkerStyle = MarkerStyle.Circle,
                Color = colors.ContainsKey(year) ? colors[year] : Color.Empty,
                MarkerBorderColor = Settings.Default.Analytics_CurrentYearSeriesColor,
                BorderWidth = 3,
                MarkerColor = Color.White,
                ChartType = SeriesChartType.Line,
                LegendText = FormatLegendText(year,yearSplit,nextYear),
                //string.Format("{0}/{1}", DateTime.Today.Year-1, DateTime.Today.Year),
                XValueType = ChartValueType.Date
            };

            chart.Series.Add(series);

            if (showBudget)
            {
                //series = new Series
                //{
                //    MarkerStyle = MarkerStyle.Circle,
                //    Color = Settings.Default.Analytics_PreviousYearBudgetColor,
                //    MarkerBorderColor = Settings.Default.Analytics_PreviousYearBudgetColor,
                //    BorderWidth = 3,
                //    MarkerColor = Color.White,
                //    ChartType = SeriesChartType.Line,
                //    LegendText = FormatLegendText(DateTime.Today.Year-1),//string.Format("{0}/{1}", DateTime.Today.Year - 1, DateTime.Today.Year),
                //    XValueType = ChartValueType.Date
                //};
                //chart.Series.Add(series);

                series = new Series
                {
                    MarkerStyle = MarkerStyle.Circle,
                    Color = Settings.Default.Analytics_CurrentYearBudgetColor,
                    MarkerBorderColor = Settings.Default.Analytics_CurrentYearBudgetColor,
                    BorderWidth = 3,
                    MarkerColor = Color.White,
                    ChartType = SeriesChartType.Line,
                    LegendText = string.Format("Budget {0}", reportDate.Value.Year),
                    // FormatLegendText(DateTime.Today.Year),//string.Format("{0}/{1}", DateTime.Today.Year - 1, DateTime.Today.Year),
                    XValueType = ChartValueType.Date
                };
                chart.Series.Add(series);
            }

            if (showThirdPeriod)
            {
                year = years != null && years.Count > 2 ? years[2] : reportDate.Value.Year + 1;
                series = new Series
                {
                    MarkerStyle = MarkerStyle.Circle,
                    Color = colors.ContainsKey(year) ? colors[year] : Color.Empty,
                    MarkerBorderColor = Settings.Default.Analytics_NextYearSeriesColor,
                    BorderWidth = 3,
                    MarkerColor = Color.White,
                    ChartType = SeriesChartType.Line,
                    LegendText = FormatLegendText(year,yearSplit),
                    //string.Format("{0}/{1}", DateTime.Today.Year - 1, DateTime.Today.Year),
                    XValueType = ChartValueType.Date
                };
                chart.Series.Add(series);
            }

            var legend = new Legend();
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            chart.Legends.Add(legend);
        }

        private string FormatLegendText(int year, bool yearSplit, bool nextYear=false)
        {
            return yearSplit
                ? nextYear
                    ? string.Format("{0}/{1}/{2}", year - 1, year, year + 1)
                    : string.Format("{0}/{1}", year - 1, year)
                : year.ToString();
        }

        //public ActionResult Chart_OrdersByMonth(string param)
        private void GenerateOrdersByMonthChart(int width, int height, Guid chartKey,
            string chartName = "Chart_OrdersByMonth", string chartPrefix = "",
            CountryFilter countryFilter = CountryFilter.UKOnly, bool? brands = true, int[] includedClients = null,
            int[] excludedClients = null, int[] excludedContainerTypes = null, int[] includedContainerTypes = null,
            IList<int> factoryIds = null, int? brand_id = null, DateMode dateMode = DateMode.YearFromNow, DateTime? from = null, 
            int monthsInFuture = 3, bool useOrderDate = false, string excludedCustomersString = "", bool showPreviousYear = true, bool? extendPreviousYearOverride = null)
        {
            var chart = new Chart {Width = width, Height = height};
            if (from == null)
                from = DateTime.Today;
            Month21 month21YearBefore = Month21.FromDate(from.Value) - 12;
            Month21 month21TwoYearsBefore = Month21.FromDate(from.Value) - 24;
            Month21 month21ThisYearStart = Month21.FromDate(Utilities.GetYearStart(from.Value));
            Month21 month21PreviousYearStart = month21ThisYearStart - 12;
            Month21 month21ThisYearEnd = month21ThisYearStart + 11;

            var dataTo = dateMode == DateMode.FromYearStart ? month21ThisYearEnd : Month21.Now;
            if (from.Value.Month >= Settings.Default.Analytics_NextYearThreshold)
                dataTo += monthsInFuture;

            var currDataMonth21From = dateMode == DateMode.FromYearStart ? month21ThisYearStart : month21YearBefore;
            var currDataMonth21To = dataTo;

            var extendPreviousYear = extendPreviousYearOverride ?? dateMode == DateMode.FromYearStart && @from.Value.AddMonths(monthsInFuture).Year == @from.Value.Year;  //if future months in same year, show two years before
            var prevDataMonth21From = dateMode == DateMode.FromYearStart ? month21PreviousYearStart - (extendPreviousYear ? 12 : 0) : month21TwoYearsBefore;
            
            var data = factoryIds == null
                ? analyticsDAL.GetNumOfOrdersByMonth(prevDataMonth21From.Value,currDataMonth21To.Value,
                countryFilter, brands, includedClients, excludedClients, excludedContainerTypes, includedContainerTypes,useOrderDate:useOrderDate,excludedCustomersString:excludedCustomersString)
                : analyticsDAL.GetNumOfOrdersByMonthForFactories(prevDataMonth21From.Value,currDataMonth21To.Value, factoryIds);

            var yearsForGraph = new List<int>();
            var startYear = prevDataMonth21From.Date.Year;
            /*if(showPreviousYear)
                yearsForGraph.Add(startYear);*/
            //yearsForGraph.Add(startYear+1);
            //yearsForGraph.Add(startYear+2);
            while(startYear <= from.Value.Year)
                yearsForGraph.Add(startYear++);


            var dataPeriods = new List<List<SalesByMonth>>();  //groups of sales data  0=first (usually previous year) 1- second (current year) 2- third (next year)

            if (dateMode == DateMode.YearFromNow)
            {
                dataPeriods.Add(data.Where(d => d.Month21 < currDataMonth21From.Value).ToList());
                dataPeriods.Add(data.Where(d => d.Month21 >= currDataMonth21From.Value).ToList());
            }
            else
            {
                foreach (var y in yearsForGraph)
                {
                    var firstInYear = Month21.FromDate(new DateTime(y, 1, 1));
                    dataPeriods.Add(data.Where(d=>d.Month21.Between(firstInYear.Value, (firstInYear+11).Value)).ToList());
                }
            }
            
            var yearDiff = dataPeriods[0].Max(d => d.Month21/100) - dataPeriods[0].Min(d => d.Month21)/100 ;
            var labeledSeriesIndex = showPreviousYear ? 1 : 0;

            FormatChart(chart, useOrderDate ? "Month order created" : "ETD", "Number of orders",showThirdPeriod:dataPeriods.Count > 2,
                yearSplit: yearDiff > 0, years:yearsForGraph,reportDate: from,seriesIndexForLabel: labeledSeriesIndex);
            
            var currMonth = Utilities.GetMonthFromNow(0);
            DataPoint anchorPoint = null;
            
            foreach (var part in dataPeriods[labeledSeriesIndex].OrderBy(d => d.Month21))
            {
                if (part != null)
                {
                    chart.Series[labeledSeriesIndex].Points.AddXY(Month21.GetDate(part.Month21), part.numOfOrders);
                    chart.Series[labeledSeriesIndex].Points.Last().IsValueShownAsLabel = true;
                    if (part.Month21 == currMonth)
                        anchorPoint = chart.Series[labeledSeriesIndex].Points.Last();

                }
            }
            if(showPreviousYear) {
                foreach (var part in dataPeriods[0].OrderBy(d => d.Month21)) {
                    if (part != null) {
                        //add year to actual date to show both series under same X axis
                        chart.Series[0].Points.AddXY(Month21.GetDate(part.Month21).AddYears(1),
                            part.numOfOrders);
                        chart.Series[0].Points.Last().IsValueShownAsLabel = true;
                    }
                }
            }

            var lastSeriesIndex = showPreviousYear ? 2 : 1;
            if (dataPeriods.Count > lastSeriesIndex )
            {
                foreach (var part in dataPeriods[lastSeriesIndex].OrderBy(d => d.Month21))
                {
                    if (part != null)
                    {
                        //add year to actual date to show both series under same X axis
                        chart.Series[lastSeriesIndex].Points.AddXY(Month21.GetDate(part.Month21).AddYears(-1),
                            part.numOfOrders);
                        chart.Series[lastSeriesIndex].Points.Last().IsValueShownAsLabel = true;
                    }
                }
            }

            //AddAnnotation(chart, anchorPoint);
            SaveChartImage(string.Format("{0}{1}", chartPrefix, chartName), chart, chartKey);
        }

        [AllowAnonymous]
        public ActionResult Chart_ProfitByMonth(string param)
        {
            
            var paramObj = JsonConvert.DeserializeObject(param) as Dictionary<string, object>;

            var chart = new Chart {Width = (int) paramObj["width"], Height = (int) paramObj["height"]};

            FormatChart(chart, "ETD", "GBP");

            var currdata = analyticsDAL.GetProfitByMonth(Utilities.GetMonthFromNow(-12),
                Utilities.GetMonthFromNow(2));
            var previousData = analyticsDAL.GetProfitByMonth(Utilities.GetMonthFromNow(-24),
                Utilities.GetMonthFromNow(-13));

            foreach (var part in currdata.OrderBy(d => d.Month21))
            {
                if (part != null)
                {
                    chart.Series[0].Points.AddXY(Utilities.GetDateFromMonth21(part.Month21), part.Amount);
                    chart.Series[0].Points.Last().IsValueShownAsLabel = true;
                }
            }
            foreach (var part in previousData.OrderBy(d => d.Month21))
            {
                if (part != null)
                {
                    //add year to actual date to show both series under same X axis
                    chart.Series[1].Points.AddXY(Utilities.GetDateFromMonth21(part.Month21).AddYears(1), part.Amount);
                    chart.Series[1].Points.Last().IsValueShownAsLabel = true;
                }
            }

            AddAnnotation(chart);

            return File(WebUtilities.StreamChart(chart), "image/jpg");
        }


        private void GenerateSalesByBrandChart(List<BrandSalesByMonthEx> currdata, List<Brand> brands, int width,
            int height, Guid chartKey, CountryFilter countryFilter = CountryFilter.UKOnly,
            BrandSalesChartType type = BrandSalesChartType.Lines, string chartName = "Chart_SalesByBrand",Month21 month21From = null, Month21 month21To = null, DateTime? reportDate = null)
        {

            if (reportDate == null)
                reportDate = DateTime.Today;

            var chart = new Chart {Width = width, Height = height};

            var emptyData = new List<BrandSalesByMonthEx>();

            var currMonth = Utilities.GetMonthFromNow(0);
            DataPoint anchorPoint = null;

            if (type == BrandSalesChartType.Lines)
            {
                FormatChart(chart, "", "GBP", false, reportDate: reportDate);
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "MMM-yy";
                var font = new Font("Microsoft sans serif",7);

                chart.ChartAreas[0].AxisX.LabelStyle.Font = font;
                

                chart.Series.Clear(); //Two default series were created

                if (month21From == null)
                    month21From = Month21.FromDate(new DateTime(DateTime.Today.Year, 1, 1));
                if (month21To == null)
                    month21To = month21From + 11;

                foreach (var brand in brands.OrderBy(b=>b.brandname))
                {
                    //var baseNum = int.Parse(DateTime.Today.Year.ToString().Substring(2, 2))*100 + 1;
                    Month21 counter = month21From;
                    for (; counter <= month21To; counter+=1)
                    {
                        if (currdata.Count(d => d.Month21 == counter.Value && d.brand_id == brand.brand_id) == 0)
                            emptyData.Add(new BrandSalesByMonthEx
                            {
                                brand_id = brand.brand_id,
                                Month21 = counter.Value,
                                brandname = brand.brandname,
                                Amount = 0
                            });
                    }
                    currdata.AddRange(emptyData);

                    var series = new Series
                    {
                        MarkerStyle = MarkerStyle.Circle,
                        BorderWidth = 3,
                        //MarkerColor = Color.White,
                        ChartType = SeriesChartType.Line,
                        LegendText = brand.brandname,
                        XValueType = ChartValueType.Date
                    };

                    foreach (var part in currdata.Where(d => d.brand_id == brand.brand_id).OrderBy(d => d.Month21))
                    {
                        if (part != null)
                        {
                            series.Points.AddXY(Utilities.GetDateFromMonth21(part.Month21), part.Amount);
                            if (part.Month21 == currMonth)
                                anchorPoint = series.Points.Last();
                        }
                    }
                    chart.Series.Add(series);

                }
                AddAnnotation(chart, anchorPoint);
                SaveChartImage(string.Format("{0}_1",chartName), chart, chartKey);
            }
            else
            {
                var area = new ChartArea
                {
                    AxisY = {IsInterlaced = true, InterlacedColor = Color.FromArgb(0xEE, 0xEE, 0xEE)},
                    AxisX = {Title = "Brand", MajorGrid = {Enabled = false}, MinorGrid = {Enabled = false}}
                };

                area.AxisY.MajorGrid.LineColor = Color.LightGray;
                area.AxisY.Title = "GBP";

                var legend = new Legend {Docking = Docking.Bottom, Alignment = StringAlignment.Center};
                chart.Legends.Add(legend);

                chart.ChartAreas.Add(area);

                chart.Customize += (sender, args) =>
                {
                    foreach (var label in ((Chart) sender).ChartAreas[0].AxisY.CustomLabels)
                    {
                        label.Text = WebUtilities.KiloFormat(double.Parse(label.Text));
                    }
                };

                var series = new Series
                {
                    MarkerStyle = MarkerStyle.Circle,
                    BorderWidth = 3,
                    //MarkerColor = Color.White,
                    ChartType = SeriesChartType.Column,
                    LegendText = "previous 12m",
                    XValueType = ChartValueType.String
                };
                chart.Series.Add(series);

                var series1 = new Series
                {
                    MarkerStyle = MarkerStyle.Circle,
                    BorderWidth = 3,
                    //MarkerColor = Color.White,
                    ChartType = SeriesChartType.Column,
                    LegendText = "last 12m",
                    XValueType = ChartValueType.String
                };
                chart.Series.Add(series1);


                var prevFrom = Utilities.GetMonthFromNow(-24);
                var from = Utilities.GetMonthFromNow(-12);
                var to = Utilities.GetMonthFromNow(-1);
                foreach (var brand in brands.OrderBy(b => b.brandname))
                {
                    series1.Points.AddXY(brand.brandname,
                        currdata.Where(d => d.brand_id == brand.brand_id && d.Month21 >= from && d.Month21 <= to)
                            .Sum(d => d.Amount));
                    series.Points.AddXY(brand.brandname,
                        currdata.Where(d => d.brand_id == brand.brand_id && d.Month21 >= prevFrom && d.Month21 < from)
                            .Sum(d => d.Amount));
                }

                SaveChartImage(string.Format("{0}_2",chartName), chart, chartKey);
            }

        }

        private void GenerateSalesByCategoryChart(int width, int height, Guid chartKey, bool current = true,
            Brand b = null, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var chart = new Chart {Width = width, Height = height};

            var area = new ChartArea();
            area.Area3DStyle.Enable3D = true;
            chart.ChartAreas.Add(area);

            var brand_user_id = b != null ? b.user_id : null;
            var category_flag = b != null ? b.category_flag : null;

            var categories = category_flag == 1
                ? analyticsDAL.GetSubCategoriesFromOrders(
                    current ? Utilities.GetMonthFromNow(-12) : Utilities.GetMonthFromNow(-24),
                    current ? Utilities.GetMonthFromNow(-1) : Utilities.GetMonthFromNow(-13), userid: brand_user_id,countryFilter: countryFilter)
                : analyticsDAL.GetCategoriesFromOrders(
                    current ? Utilities.GetMonthFromNow(-12) : Utilities.GetMonthFromNow(-24),
                    current ? Utilities.GetMonthFromNow(-1) : Utilities.GetMonthFromNow(-13), userid: brand_user_id, countryFilter: countryFilter);
            var data = category_flag == 1
                ? analyticsDAL.GetSubCategorySalesByMonth(
                    current ? Utilities.GetMonthFromNow(-12) : Utilities.GetMonthFromNow(-24),
                    current ? Utilities.GetMonthFromNow(-1) : Utilities.GetMonthFromNow(-13), userid: brand_user_id, countryFilter: countryFilter)
                : analyticsDAL.GetCategory1SalesByMonth(
                    current ? Utilities.GetMonthFromNow(-12) : Utilities.GetMonthFromNow(-24),
                    current ? Utilities.GetMonthFromNow(-1) : Utilities.GetMonthFromNow(-13), userid: brand_user_id, countryFilter: countryFilter);
            var series = new Series
            {
                ChartType = SeriesChartType.Pie
            };

            var total = data.Sum(d => d.Amount);
            series["PieLabelStyle"] = "Outside";
            series["CollectedThreshold"] = "5"; // (5 / 100 * total).ToString("N0");
            series["CollectedLegendText"] = "Other";
            series["CollectedLabel"] = "Other #PERCENT{P1}";
            series["PieLineColor"] = "black";

            
            foreach (var cat in categories)
            {
                var dataItem = data.FirstOrDefault(d => d.category_id == cat.category1_id);
                if (dataItem != null)
                {
                    series.Points.AddXY(dataItem.catname, dataItem.Amount);
                    series.Points.Last().Label = "#VALX #PERCENT{P1}";
                }
            }

            chart.Series.Add(series);

            SaveChartImage(
                string.Format("{1}Chart_SalesByCategory_{0}", current ? "1" : "0", b != null ? b.code + "_" : ""), chart,
                chartKey);

        }

        private void GenerateRespondedClaimsChart(List<Returns> claims, int width, int height, Guid chartKey,
            bool current = true)
        {
            var chart = new Chart {Width = width, Height = height};

            var area = new ChartArea();
            area.Area3DStyle.Enable3D = true;
            chart.ChartAreas.Add(area);
            chart.Legends.Add(new Legend());


            var series = new Series
            {
                ChartType = SeriesChartType.Pie
            };

            var title = chart.Titles.Add(string.Format("Total claims: {0}", claims.Count));
            title.Font = new Font(title.Font.FontFamily, title.Font.Size, FontStyle.Bold);


            series["PieLabelStyle"] = "inside";
            //series["CollectedThreshold"] = "5";// (5 / 100 * total).ToString("N0");
            //series["CollectedLegendText"] = "Other";
            //series["CollectedLabel"] = "Other #PERCENT{P1}";
            //series["PieLineColor"] = "black";

            var groupedClaims = claims.Select(
                c => new {c.returnsid, days = (c.cc_response_date - c.request_date).Value.TotalDays})
                .GroupBy(
                    gc =>
                        (gc.days >= 0 && gc.days <= 3
                            ? "0-3 days"
                            : gc.days > 3 && gc.days < 5 ? "3-5 days" : "over 5 days"));

            foreach (var gc in groupedClaims)
            {
                series.Points.AddXY(gc.Key, gc.Count());
                series.Points.Last().Label = "#PERCENT{P1}";
                series.Points.Last().LegendText = "#VALX #PERCENT{P1} #VALY";
            }
            chart.Series.Add(series);

            SaveChartImage(string.Format("Chart_RespondedClaims_{0}", current ? "1" : "0"), chart, chartKey);

        }

        private void GenerateClaimsDecisionChart(List<Returns> claims, int width, int height, Guid chartKey,
            bool current = true, bool separatebbUK = true)
        {
            var chart = new Chart {Width = width, Height = height};

            var area = new ChartArea();
            area.Area3DStyle.Enable3D = true;
            chart.ChartAreas.Add(area);

            var series = new Series
            {
                ChartType = SeriesChartType.Pie
            };
            chart.Legends.Add(new Legend());
            //chart.Palette = ChartColorPalette.None;
            //chart.PaletteCustomColors = new Color[4]
            //    {
            //        Color.FromKnownColor(KnownColor.LightBlue), Color.FromKnownColor(KnownColor.LightYellow),
            //        Color.FromKnownColor(KnownColor.Orange), Color.FromKnownColor(KnownColor.DarkBlue)
            //    };

            var title =
                chart.Titles.Add(string.Format("Total claims: £ {0:N0}",
                    claims.Sum(c => c.claim_type == 2 ? c.claim_value : c.return_qty*c.credit_value)));
            title.Font = new Font(title.Font.FontFamily, title.Font.Size, FontStyle.Bold);


            series["PieLabelStyle"] = "Disabled";
            
            if (claims.Count(c => c.ebuk == 1) == 0)
            {
                claims.Add(new Returns {ebuk = 1, return_qty = 0, credit_value = 0, decision_final = 1});
            }
            var groupedClaims = claims.Where(c => c.decision_final > 0).GroupBy(
                c => (c.decision_final == 1
                    ? (separatebbUK ? (c.ebuk == 1 ? "accepted CompanyUK" : "accepted") : "accepted")
                    : c.decision_final == 999 ? "declined" : c.decision_final == 500 ? "replacement" : "not specified"))
                .OrderBy(g => g.Key);

            foreach (var gc in groupedClaims)
            {
                series.Points.AddXY(gc.Key, gc.Sum(c => c.claim_type == 2 ? c.claim_value : c.return_qty*c.credit_value));
                series.Points.Last().IsValueShownAsLabel = false;
                series.Points.Last().LegendText = "#VALX #PERCENT{P1} #VALY{N0}";
            }
            chart.Series.Add(series);

            SaveChartImage(string.Format("Chart_ClaimsDecision_{0}", current ? "1" : "0"), chart, chartKey);

        }

        private void FormatPieChart(Chart chart)
        {

        }

        //[AllowAnonymous]
        //public ActionResult Image(string name, string key)
        //{
        //    if (key == Properties.Settings.Default.StatsKey)
        //    {
        //        return File(Path.Combine(Server.MapPath(Properties.Settings.Default.Analytics_ImagesFolder), name + ".jpg"),
        //                    "image/jpg");
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        [AllowAnonymous]
        public ActionResult Image(string param)
        {
            var parts = param.Split('#');
            if (parts.Length > 1)
            {
                var key = parts[1];
                var name = parts[0];
                if (key == Settings.Default.StatsKey)
                {
                    return
                        File(
                            Path.Combine(Server.MapPath(Settings.Default.Analytics_ImagesFolder),
                                name + ".jpg"),
                            "image/jpg");
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        [AllowAnonymous]
        public ActionResult ImagePng(string param)
        {
            var parts = param.Split('#');
            if (parts.Length > 1)
            {
                var key = parts[1];
                var name = parts[0];
                if (key == Settings.Default.StatsKey)
                {
                    return
                        File(
                            Path.Combine(Server.MapPath(Settings.Default.Analytics_ImagesFolder),
                                name + ".png"),
                            "image/png");
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        [AllowAnonymous]
        public ActionResult ForFactory(string factoryIdstring, Guid statsKey, string from = "T-1",
            CountryFilter countryFilter = CountryFilter.All, string excludedSections = "", string includedClients = "",
            string excludedClients = "")
        {
            if (!string.IsNullOrEmpty(factoryIdstring))
            {
                var factoryIds = factoryIdstring.Split(',').Select(int.Parse).ToList();
                var factories = companyDAL.GetFactories(factoryIds);
                var chartKey = Guid.NewGuid();
                var dateFrom = GetStartDate(from);
                if (statsKey == new Guid(Settings.Default.StatsKey))
                {
                    var incClients = !string.IsNullOrEmpty(includedClients)
                        ? includedClients.Split(',').Select(int.Parse).ToArray()
                        : null;
                    var exClients = !string.IsNullOrEmpty(excludedClients)
                        ? excludedClients.Split(',').Select(int.Parse).ToArray()
                        : null;


                    var currentSalesData6 = analyticsDAL.GetSalesByMonth(Utilities.GetMonthFromDate(dateFrom, -6),
                        Utilities.GetMonthFromDate(dateFrom, 2), countryFilter, factory_ids: factoryIds);


                    var currentSalesData = analyticsDAL.GetSalesByMonth(Utilities.GetMonthFromDate(dateFrom, -12),
                        Utilities.GetMonthFromDate(dateFrom, 2), countryFilter,
                        factory_ids: factoryIds);
                    var previousYearSalesData = analyticsDAL.GetSalesByMonth(Utilities.GetMonthFromDate(dateFrom, -24),
                        Utilities.GetMonthFromDate(dateFrom, -13),
                        countryFilter, factory_ids: factoryIds);

                    var excludedDistributors =
                        Settings.Default.Analytics_ExcludedDistributors.Split(',').Select(int.Parse);
                    var chartPrefix = factories[0].factory_code + "_";

                    var exSections = excludedSections.Split(',').ToList();

                    GenerateSalesByMonthChart(900, 290, currentSalesData, previousYearSalesData, chartKey, dateFrom,
                        chartPrefix);
                    GenerateOrdersByMonthChart(900, 290, chartKey, chartPrefix: chartPrefix,
                        countryFilter: countryFilter, includedClients: incClients, excludedClients: exClients,
                        brands: null, factoryIds: factoryIds);

                    var distributors = companyDAL.GetDistributors();
                    var new_from = dateFrom.AddDays(-6).Date;
                    var new_to = dateFrom;
                    ViewBag.from = new_from;
                    ViewBag.to = new_to;
                    return View("ForFactory", model: new AnalyticsModel
                    {
                        CurrentSalesData = currentSalesData,
                        PreviousYearSalesData = previousYearSalesData,
                        Factories = factories,

                        Distributors = distributors,

                        CurrentCountrySalesData =
                            analyticsDAL.GetCountrySales(Utilities.GetMonthFromDate(dateFrom, -12),
                                Utilities.GetMonthFromDate(dateFrom, -1), countryFilter, factoryIds),
                        PreviousCountrySalesData =
                            analyticsDAL.GetCountrySales(Utilities.GetMonthFromDate(dateFrom, -24),
                                Utilities.GetMonthFromDate(dateFrom, -13), countryFilter, factoryIds),
                        CurrentProductSalesData = analyticsDAL.GetProductSales(fromDate: DateTime.Today.AddYears(-1),
                            toDate: DateTime.Today, countryFilter: countryFilter, factories: factoryIds),
                        PreviousProductSalesData =
                            analyticsDAL.GetProductSales(fromDate: DateTime.Today.AddYears(-2),
                                toDate: DateTime.Today.AddYears(-1).AddDays(-1), countryFilter: countryFilter,
                                factories: factoryIds),
                        ChartKey = chartKey,
                        ExcludedSections = exSections
                    });
                }
                else
                {
                    ViewBag.message = "Invalid stats key";
                    return View("Message");
                }
            }
            return null;
        }

        [AllowAnonymous]
        public ActionResult ClaimsInvestigations(DateTime from, Guid statsKey,
            CountryFilter countryFilter = CountryFilter.UKOnly, string excludedSections = "",
            ReportType type = ReportType.Brands, string includedClients = "", string excludedClients = "",
            int? brand_id = null)
        {


            return View();
        }

        

        

        public ActionResult BrandStockReport(double upperFactor = 1.5, double lowerFactor = 1.5, bool showRegularProducts = false, int? brand_id = null, string customerCode = null, int productShowThreshold = 60, int productShowThresholdBrand = 20)
        {
            BrandStockReportRow.LowerFactor = lowerFactor;
            BrandStockReportRow.UpperFactor = upperFactor;
            var model = new BrandStockReportModel{Brands = brandsDAL.GetAll().Where(b=>b.brand_id == brand_id || brand_id == null).ToList(),
                //CustomerData = AnalyticsDAL.GetBrandSalesByMonth(fromDate: DateTime.Today.AddMonths(-6), toDate: DateTime.Today,customerLevel: true),
                ProductData = analyticsDAL.GetBrandSalesByMonth(Utilities.GetMonthFromNow(-6), productLevel: true,customerLevel: true, useETA:true,
                    brand_id: brand_id,customerCode:customerCode),
                UpperFactor = upperFactor,LowerFactor = lowerFactor,ShowRegularProducts = showRegularProducts,
                AnalyticsSubcategories = analyticsSubcategoryDal.GetForBrand(brand_id),CustomerCode = customerCode,ProductShowThreshold = productShowThreshold,ProductShowThresholdBrand = productShowThresholdBrand
            };
            
            return View(model);
        }

        

        [Authorize]
        public ActionResult GetBrandStockReport()
        {
            
            var model = new GetBrandStockReportModel
            {
                CustomerCodes = companyDAL.GetDistributors().Where(d=>d.user_country.In("GB","IE") && d.distributor > 0).Select(d=>d.customer_code).OrderBy(d=>d).ToList(),
                Brands = brandsDAL.GetAll(),
                ShowRegularProducts = true,
                LowerFactor = 1.5,
                UpperFactor = 3,
                ProductShowThreshold = 20
            };
            
            return View(model);
        }

        public OrdersAnalysisReport GetLiveOrdersAnalysis(Guid statsKey, Guid chartKey, DateTime from, CountryFilter countryFilter, string excludedCustomersString = "", int monthsHistoryForNewProducts = 6,
            IList<int> includedNonDistributorsIds = null, bool showNewProducts = true, bool increaseDecreaseSplit = true,
			string summaryDataPoints = "100,50", bool brands = true, string startLateGroup = "C")
        {
            //var lines = OrderRepository.GetLinesForOrdersAnalysis(from, countryFilter);
            var lines = orderLinesDAL.GetLiveOrdersAnalysis(from, countryFilter,includedNonDistributorsIds:includedNonDistributorsIds, brands: brands);

	        var lateGroups = GetLateGroups(startLateGroup);
            var excludedProducts = Settings.Default.Product_HideInReports.Split(',').ToList();
            var excludedCustomers = excludedCustomersString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var prods =
                custproductsDAL.GetAllProductsWithSameCode(
                    lines.Where(l => lateGroups.Contains(l.Cust_Product.MastProduct.product_group))
                        .Select(l => l.cprod_id ?? 0)
                        .Distinct()
                        .ToList());


            var  newProductsIds = new List<int?>();
	        var newProducts = new List<Cust_products>();
            var productSalesBefore = analyticsDAL.GetProductSales(fromDate: null,
                    toDate: from.AddMonths(-1 * monthsHistoryForNewProducts).AddSeconds(-1), countryFilter: countryFilter,
                    useLineDate: true,
                    prodIds:
                        prods.Select(p => p.cprod_id)
                            .ToList(),
                    shippedOrdersOnly: false, excludedCustomers: excludedCustomers);
            newProductsIds.AddRange(productSalesBefore.Where(s => s.numOfUnits <= 0).Select(s => (int?) s.cprod_id));
            newProducts = prods.Where(p => newProductsIds.Contains(p.cprod_id)).ToList();
            
            lines =
                lines.Where(
                    l => (excludedCustomers.Length == 0 || !excludedCustomers.Contains(l.Header.Client.customer_code)) && (!excludedProducts.Contains(l.Cust_Product.cprod_code1)) 
                       )
                    .ToList();
            var result = 
                new OrdersAnalysisReport
                {
                    OrdersAnalysisRows =
                        lines.GroupBy(l => new {l.Header.userid1, l.Header.Client.customer_code}).
                            Select(
                                g =>
                                    new OrdersAnalysisRow
                                    {
                                        client_id = g.Key.userid1 ?? 0,
                                        customer_code = g.Key.customer_code,
                                        NumOfOrders = g.Distinct(new OrderLineByHeaderComparer()).Count(),
										NumOfOrdersLateShipping = g.Where(l=>l.Header.original_eta > l.Header.system_eta).Distinct(new OrderLineByHeaderComparer()).Count(),
                                        NumOfProducts = g.Distinct(new OrderLineByMastProductComparer()).Count(),
                                        NumOfLateProducts =
                                            g.Where(l => lateGroups.Contains(l.Cust_Product.MastProduct.product_group))
                                                .Distinct(new OrderLineByMastProductComparer())
                                                .Count(),
	                                    NumOfLateNewProducts = g.Where(l => newProductsIds.Contains(l.cprod_id) && lateGroups.Contains(l.Cust_Product.MastProduct.product_group))
		                                    .Distinct(new OrderLineByMastProductComparer())
		                                    .Count(),
                                        NumOfOrdersWithLateProducts =
                                            g.Where(l => lateGroups.Contains(l.Cust_Product.MastProduct.product_group))
                                                .Distinct(new OrderLineByHeaderComparer())
                                                .Count()
                                    })
                            .ToList(),
                    NumOfProducts = lines.Distinct(new OrderLineByMastProductComparer()).Count(),
                    NumOfLateProducts =
                        lines.Where(l => lateGroups.Contains(l.Cust_Product.MastProduct.product_group))
                            .Distinct(new OrderLineByMastProductComparer())
                            .Count(),
					NumOfNewProducts = newProducts.Distinct(new CustProductMastProductDistinctComparer()).Count()
                };
            IPdfManager pdf = new PdfManager();
            string fileName;
            IPdfDocument doc;
            foreach (var r in result.OrdersAnalysisRows)
            {
                doc = pdf.CreateDocument();
                doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("AnalysisProductInfo", 
				new { statsKey, from, countryFilter, r.client_id, excludedCustomersString, monthsHistoryForNewProducts,
					showNewProducts,increaseDecreaseSplit,summaryDataPoints, brands, startLateGroup }), 
                    "scale=0.78, leftmargin=22,rightmargin=22,media=1,Timeout=1000",
                    "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value
                    );
                fileName = string.Format("AnalysisProduct_{0}_{1}.pdf",chartKey , r.client_id);
                doc.Save(Path.Combine(Server.MapPath(Settings.Default.Analytics_ImagesFolder), fileName), true);
            }
            doc = pdf.CreateDocument();
            doc.ImportFromUrl(WebUtilities.GetSiteUrl() + Url.Action("AnalysisProductInfo", 
				new { statsKey, from, countryFilter, excludedCustomersString, monthsHistoryForNewProducts,
					showNewProducts,increaseDecreaseSplit, summaryDataPoints, brands, startLateGroup }), 
                    "scale=0.78, leftmargin=22,rightmargin=22,media=1,Timeout=1000",
                    "Cookie:" + FormsAuthentication.FormsCookieName, Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            fileName = string.Format("AnalysisProduct_{0}.pdf", chartKey);
            doc.Save(Path.Combine(Server.MapPath(Settings.Default.Analytics_ImagesFolder), fileName), true);
            return result;
        }

        public OrdersAnalysisProductReport GetLiveOrdersAnalysisProductInfo(DateTime from,
            CountryFilter countryFilter, int? client_id = null, string excludedCustomersString = "", 
			int monthsHistoryForNewProducts = 6, bool brands = true, string startLateGroup = "C")
        {
            //var lines = OrderRepository.GetLinesForOrdersAnalysis(from, countryFilter,client_id);
            var excludedCustomers = excludedCustomersString.Split(',').Select(s => s.Trim()).ToList();
            var lines = orderLinesDAL.GetLiveOrdersAnalysis(from, countryFilter, client_id, brands: brands);
            if (!string.IsNullOrEmpty(excludedCustomersString))
            {
                lines = lines.Where(l => !excludedCustomers.Contains(l.Header.Client.customer_code)).ToList();
            }
            var excludedProducts = Settings.Default.Product_HideInReports.Split(',').ToList();
            lines =
               lines.Where(
                   l => !excludedProducts.Contains(l.Cust_Product.cprod_code1))
                   .ToList();
	        var lateGroups = GetLateGroups(startLateGroup);
            var factories = companyDAL.GetFactories();

            var prodIds = lines.Where(l => lateGroups.Contains(l.Cust_Product.MastProduct.product_group))
                .Select(l => l.cprod_id ?? 0)
                .Distinct()
                .ToList();

            var prods =
                custproductsDAL.GetAllProductsWithSameCode(prodIds);

            var productSalesBeforeNMonths = prodIds.Count > 0 ? analyticsDAL.GetProductSales(fromDate: null,
                    toDate: from.AddMonths(-1 * monthsHistoryForNewProducts).AddSeconds(-1), countryFilter: countryFilter,
                    useLineDate: true,
                    prodIds: prodIds,
                    shippedOrdersOnly: false, excludedCustomers: excludedCustomers) : new List<ProductSales>();
            var newProducts = new List<int?>();
            foreach (var prod in prods)
            {
                if(productSalesBeforeNMonths.Where(p=>p.cprod_id == prod.cprod_id).Sum(p=>p.numOfUnits) <= 0)
                    newProducts.Add(prod.cprod_id);
            }

            var dictSalesPrev3m = analyticsDAL.GetProductSales(fromDate: from.AddMonths(-6),
                                toDate: from.AddMonths(-3).AddSeconds(-1), countryFilter: countryFilter, useLineDate: true,
                                prodIds: prodIds,
                                shippedOrdersOnly: false, excludedCustomers: excludedCustomers).ToDictionary(s => s.cprod_id);
            var dictSalesLast3m = analyticsDAL.GetProductSales(fromDate: from.AddMonths(-3),
                                toDate: from, countryFilter: countryFilter, useLineDate: true,
                                prodIds: prodIds,
                                shippedOrdersOnly: false, excludedCustomers: excludedCustomers).ToDictionary(s => s.cprod_id);

            lines =
                lines.Where(
                    l => (excludedCustomers.Count == 0 || !excludedCustomers.Contains(l.Header.Client.customer_code)) && (!excludedProducts.Contains(l.Cust_Product.cprod_code1)))
                    .ToList();

            return new OrdersAnalysisProductReport
            {
                Products = lines.Where(l => lateGroups.Contains(l.Cust_Product.MastProduct.product_group)).GroupBy(l => l.Cust_Product.cprod_id)
                    .Select(g => new OrdersAnalysisProductRow
                    {
                        Product = g.First().Cust_Product,
                        Location =
                            (company.Common.Extensions.IfNotNull(factories.FirstOrDefault(f => f.user_id == g.First().Cust_Product.MastProduct.factory_id), f => f.consolidated_port)),
                        Qty = g.Sum(l => l.orderqty),
                        NumOfOrders = g.Distinct(new OrderLineByHeaderComparer()).Count(),
                        Distributors = g.Select(l => l.Header.Client.customer_code).Distinct().ToList(),
                        SalesPrev3m = dictSalesPrev3m.ContainsKey(g.First().cprod_id ?? 0) ? dictSalesPrev3m[g.First().cprod_id ?? 0].numOfUnits : 0,
                        SalesLast3m = dictSalesLast3m.ContainsKey(g.First().cprod_id ?? 0) ? dictSalesLast3m[g.First().cprod_id ?? 0].numOfUnits : 0,
                        SalesBeforeLastnMonths = 
                            productSalesBeforeNMonths.Where(p=>p.cprod_id == g.Key).Sum(s => s.numOfUnits),
                    }).ToList(),
                NumOfOrders = lines.Distinct(new OrderLineByHeaderComparer()).Count(),
                NumOfProducts = lines.Distinct(new OrderLineByMastProductComparer()).Count(),
                NumOfLateProducts =
                    lines.Where(l => lateGroups.Contains(l.Cust_Product.MastProduct.product_group) && !newProducts.Contains(l.cprod_id))
                        .Distinct(new OrderLineByMastProductComparer())
                        .Count(),
                NumOfOrdersWithLateProducts =
                    lines.Where(l => lateGroups.Contains(l.Cust_Product.MastProduct.product_group) && !newProducts.Contains(l.cprod_id))
                        .Distinct(new OrderLineByHeaderComparer())
                        .Count(),
				NewProductIds = newProducts
            };
        }

	    private string[] GetLateGroups(string startLateGroup = "C")
	    {
		    var sabcData = sabcSortDal.GetAll();
		    var lateSabcData = GetLateSabcData(sabcData, startLateGroup);
		    return lateSabcData != null ? lateSabcData.Select(s=>s.SABC).ToArray() :  new[] { "C", "C+" };
	    }

	    public static IList<Sabc_sort> GetLateSabcData(IList<Sabc_sort> sabcData, string startLateGroup)
	    {
		    var sabcStart = sabcData.FirstOrDefault(s => s.SABC == startLateGroup);
			return sabcStart != null ? sabcData.Where(s=>s.prod_days >= sabcStart.prod_days).ToArray() :  null;
	    }


        private TwoPeriods CalculatePeriods(DateTime? from)
        {
            if (from == null)
                from = DateTime.Today;
            return new TwoPeriods
            {
                FirstPeriodStart = from.Value.AddMonths(-12),
                FirstPeriodEnd = from.Value.AddMonths(-6),
                LastPeriodStart = from.Value.AddMonths(-6).AddDays(1),
                LastPeriodEnd = from.Value
            };
            
        }

        


        
        [AllowAnonymous]
        public ActionResult SalesOrdersMonthlySalesReport(Guid statsKey, string warehouse = "IR", int chartWidth=900, int chartHeight = 400, DateTime? forDate = null, int historicalYears = 3, int? brand_id = null)
        {
            if(statsKey == new Guid(Settings.Default.StatsKey)) {
                
                var model = BuildSalesOrdersMonthlyReportModel(forDate, warehouse, brand_id: brand_id);
                var chart = GenerateSalesOrdersMonthlyChart(model, chartWidth, chartHeight);
                ViewBag.ChartKey = Guid.NewGuid();
                SaveChartImage("SalesOrdersMonthlyReport", chart, ViewBag.ChartKey);
                return View(model);
            }
            ViewBag.message = "No key";
            return View("Message");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult BudgetActual()
        {
            return View();
        }

        private SalesOrdersMonthlyReportModel BuildSalesOrdersMonthlyReportModel(DateTime? forDate,string warehouse, DateTime? chartFrom = null, int? brand_id = null, string brands = "CW")
        {
            DateTime budgetForDate = forDate != null ? forDate.Value : DateTime.Today;

            if (forDate == null)
                forDate = Utilities.GetMonthEnd(DateTime.Today.AddMonths(-1));
            if(chartFrom == null)
                chartFrom = new DateTime(forDate.Value.AddYears(-3).Year, 1, 1);
            var month21From = chartFrom.Value.ToMonth21Value();
            int month21BudgetFrom = Utilities.GetYearStart(budgetForDate).ToMonth21Value(), month21BudgetTo = Utilities.GetYearEnd(budgetForDate).ToMonth21Value();
            return new SalesOrdersMonthlyReportModel
            {
                ForDate = forDate.Value ,
                Warehouse = WebUtilities.MapStockOrdersWarehouse(warehouse),
                BudgetData = unitOfWork.BudgetActualDataRepository.Get(b=>b.month21 >= month21BudgetFrom && b.month21 <= month21BudgetTo && b.record_type == "B" && (b.brand_id == brand_id || brand_id == null)).ToList(),
                Rows = unitOfWork.SalesOrdersRepository.Get(s => s.brand == brands && s.date_entered >= chartFrom).
                    GroupBy(s => new { s.date_entered.Value.Year, s.date_entered.Value.Month }).
                    Select(g => new { g.Key, Value = g.Sum(s => s.value) }).ToList().
                    Select(s => new SalesOrdersMonthlyReportRow
                    {
                        Month21 = Month21.FromDate(new DateTime(s.Key.Year, s.Key.Month, 1)),
                        Amount = s.Value
                    }).ToList()
            };
        }

        private List<BrandStockSummaryChartItem>GenerateBrandStockSummaryDataForGraph(BrandStockSummaryModel model)
        {

            var costDataForGraphUSA = model.CostValueItemListUSA.GroupBy(m => new { m.date_entered.Year, m.date_entered.Month })
                    .Select(x => new BrandStockSummaryChartItem
                    {
                        Year = x.FirstOrDefault().date_entered.Year,
                        Month = x.FirstOrDefault().date_entered.Month,
                        DateValue = string.Format("{0}.{1}", x.Key.Month, x.Key.Year),
                        cost = x.Sum(b => b.order_qty * b.price)
                    }).ToList();

            var stockReceiptDataForGraphUSA = model.StockReceiptItemListUSA.GroupBy(m => new { m.booked_in_date.Year, m.booked_in_date.Month })
                                .Select(x => new BrandStockSummaryChartItem
                                {
                                    Year = x.FirstOrDefault().booked_in_date.Year,
                                    Month = x.FirstOrDefault().booked_in_date.Month,
                                    DateValue = string.Format("{0}.{1}", x.Key.Month, x.Key.Year),
                                    receipts = x.Sum(b => b.orderqty * b.unit_price)
                                }).ToList();

            var costDataForGraphOthers = model.CostValueItemListOthers.GroupBy(m => new { m.date_entered.Year, m.date_entered.Month })
                            .Select(x => new BrandStockSummaryChartItem
                            {
                                Year = x.FirstOrDefault().date_entered.Year,
                                Month = x.FirstOrDefault().date_entered.Month,
                                DateValue = string.Format("{0}.{1}", x.Key.Month, x.Key.Year),
                                cost = x.Sum(b => b.order_qty * b.price)
                            }).ToList();

            var stockReceiptDataForGraphOthers = model.StockReceiptItemListOthers.GroupBy(m => new { m.booked_in_date.Year, m.booked_in_date.Month })
                                .Select(x => new BrandStockSummaryChartItem
                                {
                                    Year = x.FirstOrDefault().booked_in_date.Year,
                                    Month = x.FirstOrDefault().booked_in_date.Month,
                                    DateValue = string.Format("{0}.{1}", x.Key.Month, x.Key.Year),
                                    receipts = x.Sum(b => b.orderqty * b.unit_price)
                                }).ToList();

            var dataListForChart = new List<BrandStockSummaryChartItem>();

            GetDataForChart(model.ForDate, (double)model.StockValueUSA, costDataForGraphUSA, stockReceiptDataForGraphUSA, BrandStockSummaryItemType.US, dataListForChart);

            GetDataForChart(model.ForDate, (double)model.StockValueOthers, costDataForGraphOthers, stockReceiptDataForGraphOthers, BrandStockSummaryItemType.Others, dataListForChart);


            return dataListForChart;

            /*

            double totalcalctempUSA = (double)model.StockValueUSA;
            var dateForChart = model.ForDate;

            //initial value - last value, current state
            BrandStockSummaryChartItem dataListForChartItem = new BrandStockSummaryChartItem
            { Year = dateForChart.Year, Month = dateForChart.Month, DateValue = string.Format("{0}.{1}", dateForChart.Month, dateForChart.Year), totalcalc = (double)model.StockValueUSA, ItemType = BrandStockSummaryItemType.US  };

            dataListForChart.Add(dataListForChartItem);

            for (int i = 1; i < 7; i++)
            {
                dateForChart = model.ForDate.AddMonths(i * -1);

                double tempCostCalc = costDataForGraphUSA.Where(c => c.Month == dateForChart.Month && c.Year == dateForChart.Year).Select(m => m.cost).FirstOrDefault();
                double tempReceiptCalc = stockReceiptDataForGraphUSA.Where(c => c.Month == dateForChart.Month && c.Year == dateForChart.Year).Select(m => m.receipts).FirstOrDefault();

                var test = totalcalctempUSA - (tempReceiptCalc != 0 ? tempReceiptCalc : 0) + (tempCostCalc != 0 ? tempCostCalc : 0);

                totalcalctempUSA = test;

                dataListForChartItem = new BrandStockSummaryChartItem
                {
                    Year = dateForChart.Year,
                    Month = dateForChart.Month,
                    DateValue = string.Format("{0}.{1}",
                            dateForChart.Month, dateForChart.Year),
                    totalcalc = totalcalctempUSA,
                    receipts = tempReceiptCalc,
                    cost = tempCostCalc,
                    ItemType = BrandStockSummaryItemType.US
                };

                dataListForChart.Add(dataListForChartItem);
            }

            

            */
        }

        private void GetDataForChart(DateTime dateForChart, double stockValue, List<BrandStockSummaryChartItem> cost, List<BrandStockSummaryChartItem> receipt, BrandStockSummaryItemType type, List<BrandStockSummaryChartItem> itemList)
        {
            double totalcalctemp = stockValue;

            var dateForChartTemp = dateForChart;

            //initial value  last value, current state
            BrandStockSummaryChartItem dataListForChartItem = new BrandStockSummaryChartItem
            { Year = dateForChart.Year, Month = dateForChart.Month, DateValue = string.Format("{0}.{1}", dateForChart.Month, dateForChart.Year), totalcalc = stockValue, ItemType = type };

            itemList.Add(dataListForChartItem);

            for (int i = 1; i < 7; i++)
            {
                dateForChartTemp = dateForChart.AddMonths(i * -1);

                double tempCostCalc = cost.Where(c => c.Month == dateForChartTemp.Month && c.Year == dateForChartTemp.Year).Select(m => m.cost).FirstOrDefault();
                double tempReceiptCalc = receipt.Where(c => c.Month == dateForChartTemp.Month && c.Year == dateForChartTemp.Year).Select(m => m.receipts).FirstOrDefault();

                var test = totalcalctemp - (tempReceiptCalc != 0 ? tempReceiptCalc : 0) + (tempCostCalc != 0 ? tempCostCalc : 0);

                totalcalctemp = test;

                dataListForChartItem = new BrandStockSummaryChartItem
                {
                    Year = dateForChartTemp.Year,
                    Month = dateForChartTemp.Month,
                    DateValue = string.Format("{0}.{1}",
                            dateForChartTemp.Month, dateForChartTemp.Year),
                    totalcalc = totalcalctemp,
                    receipts = tempReceiptCalc,
                    cost = tempCostCalc,
                    ItemType = type
                };

                itemList.Add(dataListForChartItem);
            }
        }

        private Chart GenerateBrandStockSummaryChart(BrandStockSummaryModel model, int chartWidth, int chartHeight)
        {
            var chart = new Chart { Width = chartWidth, Height = chartHeight};
            
            var area = new ChartArea();
            
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisX.Title = "Month";
            area.AxisX.IntervalType = DateTimeIntervalType.Months;
            area.AxisX.Interval = 1;
            area.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            area.AxisX.LabelStyle.Format = "MMM.yy";

            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.LabelStyle.Format = "$ #,###";
            area.AxisY.Title = "$";
            area.AxisY.IsStartedFromZero = true;
            area.AxisY.Minimum = 0; 

            chart.ChartAreas.Add(area);

            var legend = new Legend();
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;

            chart.Legends.Add(legend);
            
            Series series;
            Series seriesOther;

            series = new Series
            {
                ChartType = SeriesChartType.Line,
                LegendText = "Stock value US",
                XValueType = ChartValueType.Date,
                BorderWidth = 2,
                Color = Color.Red
            };

            seriesOther = new Series
            {
                ChartType = SeriesChartType.Line,
                LegendText = "Stock value UK",
                XValueType = ChartValueType.Date,
                BorderWidth = 2,
                Color = Color.Blue
            };

            if (model.DataForChart != null)
            {

                foreach (var o in model.DataForChart.Where(o => o.ItemType == BrandStockSummaryItemType.US).OrderBy(c => c.Year).ThenBy(n => n.Month))
                {
                    var offsetDate = new DateTime(o.Year, o.Month, 1);
                    series.Points.AddXY(offsetDate, o.totalcalc);
                }
                foreach (var o in model.DataForChart.Where(o => o.ItemType == BrandStockSummaryItemType.Others).OrderBy(c => c.Year).ThenBy(n => n.Month))
                {
                    var offsetDate = new DateTime(o.Year, o.Month, 1);
                    seriesOther.Points.AddXY(offsetDate, o.totalcalc);
                }    
            }

            chart.Series.Add(series);
            chart.Series.Add(seriesOther);

            return chart;
        }

        private Chart GenerateSalesOrdersMonthlyChart(SalesOrdersMonthlyReportModel model, int chartWidth, int chartHeight)
        {
            var chart = new Chart { Width = chartWidth, Height = chartHeight };

            var area = new ChartArea();
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.Title = "Month";
            area.AxisY.LabelStyle.Format = "$ #,###";
            area.AxisX.IntervalType = DateTimeIntervalType.Months;
            //area.AxisX.IntervalOffset = 1;
            area.AxisX.Interval = 1;
            area.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            //area.AxisX.IntervalOffsetType = DateTimeIntervalType.Months;
            area.AxisX.LabelStyle.Format = "MMM";
            area.AxisY.Title = "$";            
            chart.ChartAreas.Add(area);
            var legend = new Legend();
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            chart.Legends.Add(legend);            

            //chart.Legends.Add(new Legend());
            chart.Titles.Add($"{model.Warehouse} Sales");
            var rows = model.Rows.Where(r => r.Month21 < Month21.Now).ToList();
            var years = rows.GroupBy(r => r.Month21.Date.Year).Select(g => g.Key).OrderBy(g => g).ToList();
            var dictValues = rows.ToDictionary(r => r.Month21.Value, r => r.Amount);
            int? startYear = null;
            if (years.Count > 0)
                startYear = years[0];

            Series series;
            if (startYear != null) {
                foreach (var y in years) {
                    series = new Series
                    {
                        ChartType = SeriesChartType.Line,
                        LegendText = y.ToString(),
                        XValueType = ChartValueType.Date,
                        BorderWidth = 2
                    };
                    for (int i = 1; i <= 12; i++) {
                        var date = new DateTime(y, i, 1);
                        var offsetDate = new DateTime(startYear.Value, i, 1);
                        var m21 = new Month21(date).Value;
                        double? value;
                        //Don't add points in the last year for non-existing data (usually current uncompleted month)
                        var addPoint = y < DateTime.Today.Year || dictValues.ContainsKey(m21);
                        value = dictValues.ContainsKey(m21) ? dictValues[m21] : 0.0;
                        if (addPoint)
                            series.Points.AddXY(offsetDate, value);
                    }
                    chart.Series.Add(series);
                }
                dictValues = model.BudgetData.ToDictionary(b => b.month21, b => b.value);
                if (model.BudgetData != null && model.BudgetData.Count > 0) {
                    //budget
                    var budgetYear = Month21.GetDate(model.BudgetData[0].month21).Year;
                    series = new Series
                    {
                        ChartType = SeriesChartType.Line,
                        LegendText = $"Budget {budgetYear.ToString()}",
                        XValueType = ChartValueType.Date,
                        BorderWidth = 2
                    };
                    for (int i = 1; i <= 12; i++) {
                        var date = new DateTime(budgetYear, i, 1);
                        var offsetDate = new DateTime(startYear.Value, i, 1);
                        var m21 = new Month21(date).Value;
                        double? value;
                        value = dictValues.ContainsKey(m21) ? dictValues[m21] : 0.0;
                        series.Points.AddXY(offsetDate, value);
                    }
                    chart.Series.Add(series);
                }
                
            }
            

            return chart;
        }

        private Chart GenerateReturnsChart(int chartType,int chartWidth, int chartHeight, List<Returns> claims)
        {
            var chart = new Chart { Width = chartWidth, Height = chartHeight };

            var area = new ChartArea();
            area.Area3DStyle.Enable3D = true;
            chart.ChartAreas.Add(area);

            var series = new Series
            {
                ChartType = SeriesChartType.Pie
            };
            chart.Legends.Add(new Legend());

            series["PieLabelStyle"] = "Disabled";

            if (chartType == 1)
            {
                chart.Titles.Add("Claims by customer");
                //units claimed by customer
                foreach (var g in claims.GroupBy(cl=>cl.client_id))
                {
                    series.Points.AddXY(g.First().Client.customer_code, g.Sum(c=>c.return_qty));
                    series.Points.Last().IsValueShownAsLabel = false;
                    series.Points.Last().LegendText = "#VALX #PERCENT{P1} #VALY{N0}";
                }
            }
            else
            {
                chart.Titles.Add("Claims by reason");
                foreach (var g in claims.GroupBy(c=>c.reason))
                {
                    series.Points.AddXY(CommonController.GetReturnReasonFullName(g.Key), g.Sum(c=>c.return_qty));
                    series.Points.Last().IsValueShownAsLabel = false;
                    series.Points.Last().LegendText = "#VALX #PERCENT{P1} #VALY{N0}";
                }    
            }

            
            chart.Series.Add(series);
            return chart;
            //SaveChartImage("Chart_ProductDetail_Claims", chart, chartKey.ToString());
        }

        private Chart GenerateSalesUnitsChart(int chartWidth, int chartHeight, List<ProductSales> sales)
        {
            var chart = new Chart { Width = chartWidth, Height = chartHeight };

            var area = new ChartArea {Area3DStyle = {Enable3D = true}};
            chart.ChartAreas.Add(area);

            var series = new Series
            {
                ChartType = SeriesChartType.Pie
            };
            chart.Legends.Add(new Legend());
            chart.Titles.Add("Unit sales by customer");
            series["PieLabelStyle"] = "Disabled";
            
            foreach (var g in sales.GroupBy(s => s.customer_code))
            {
                series.Points.AddXY(g.Key, g.Sum(s => s.numOfUnits));
                series.Points.Last().IsValueShownAsLabel = false;
                series.Points.Last().LegendText = "#VALX #PERCENT{P1} #VALY{N0}";
            }
            
            chart.Series.Add(series);
            return chart;
            //SaveChartImage("Chart_ProductDetail_Claims", chart, chartKey.ToString());
        }

        private Chart GenerateClaimsLineChart(DateTime from,int chartWidth, int chartHeight, List<Returns> returns)
        {
            var chart = new Chart { Width = chartWidth, Height = chartHeight };

            var area = new ChartArea();
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.Title = "Month";
            area.AxisY.Title = "units";
            chart.ChartAreas.Add(area);

            var series = new Series{ChartType = SeriesChartType.Line};
            //chart.Legends.Add(new Legend());
            chart.Titles.Add("Claims by month");

            foreach (var g in returns.GroupBy(s => -1*(from.Month > s.request_date.Month() ? from.Month - s.request_date.Month() : 12+from.Month - s.request_date.Month()) ))
            {
                series.Points.AddXY(from.AddMonths(g.Key ?? 0).ToString("MM/yy"), g.Sum(s => s.return_qty));
                series.Points.Last().IsValueShownAsLabel = true;
                //series.Points.Last().LegendText = "#VALX #PERCENT{P1} #VALY{N0}";
            }

            chart.Series.Add(series);
            return chart;
        }

        private void GenerateStockSummaryChart(List<StockSummary> stockSummaryData, int width, int height, Guid chartKey)
        {
            var chart = new Chart { Width = width, Height = height };

            var area = new ChartArea();
            area.Area3DStyle.Enable3D = true;
            chart.ChartAreas.Add(area);

            var series = new Series
            {
                ChartType = SeriesChartType.Pie
            };
            chart.Legends.Add(new Legend());
            
            series["PieLabelStyle"] = "Inside";

            series.Points.AddXY("In production GBP", stockSummaryData.Sum(s=>s.InProduction));
            series.Points.Last().IsValueShownAsLabel = true;
            series.Points.Last().Label = "#VALY{N0}";
            series.Points.Last().LegendText = "#VALX #PERCENT{P1}";

            series.Points.AddXY("Ready at factory GBP", stockSummaryData.Sum(s => s.ReadyAtFactory));
            series.Points.Last().IsValueShownAsLabel = true;
            series.Points.Last().Label = "#VALY{N0}";
            series.Points.Last().LegendText = "#VALX #PERCENT{P1}";

            series.Points.AddXY("On Water GBP", stockSummaryData.Sum(s => s.OnWater));
            series.Points.Last().IsValueShownAsLabel = true;
            series.Points.Last().Label = "#VALY{N0}";
            series.Points.Last().LegendText = "#VALX #PERCENT{P1}";

            series.Points.AddXY("Warehouse GBP", stockSummaryData.Sum(s => s.Warehouse));
            series.Points.Last().IsValueShownAsLabel = true;
            series.Points.Last().Label = "#VALY{N0}";
            series.Points.Last().LegendText = "#VALX #PERCENT{P1}";
            
            chart.Series.Add(series);

            SaveChartImage("Chart_StockSummary", chart, chartKey);
        }

        private void GenerateStockSummaryChart2(List<StockSummary> stockSummaryData, int width, int height, Guid chartKey)
        {
            var chart = new Chart { Width = width, Height = height };

            var area = new ChartArea();
            //area.Area3DStyle.Enable3D = true;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.LabelStyle.Format = "N0";
            chart.ChartAreas.Add(area);
            chart.Legends.Add(new Legend());

            var seriesUnall = new Series {ChartType = SeriesChartType.StackedBar, LegendText = "unallocated stock at factory"};
            var seriesAll = new Series { ChartType = SeriesChartType.StackedBar, LegendText = "allocated stock at factory" };
            var seriesWater = new Series { ChartType = SeriesChartType.StackedBar, LegendText = "stock on water" };
            var seriesStockUk = new Series { ChartType = SeriesChartType.StackedBar, LegendText = "stock in uk" };
            chart.Series.Add(seriesUnall);
            chart.Series.Add(seriesAll);
            chart.Series.Add(seriesWater);
            chart.Series.Add(seriesStockUk);

            var factories = stockSummaryData.Select(s => s.factory_code).Distinct().ToList();
            foreach (var f in factories)
            {
                var factoryData = stockSummaryData.Where(s => s.factory_code == f).ToList();
                seriesUnall.Points.AddXY(f, factoryData.Sum(d => d.UnallocatedAtFactoryValue ?? 0));
                seriesAll.Points.AddXY(f, factoryData.Sum(d => d.AllocatedAtFactory ?? 0));
                seriesWater.Points.AddXY(f, factoryData.Sum(d => d.OnWater ?? 0));
                seriesStockUk.Points.AddXY(f, factoryData.Sum(d => d.Warehouse ?? 0));    
                
            }
           

            SaveChartImage("Chart_StockSummary", chart, chartKey);
        }

        

        /// <summary>
        /// If there are specific factory target weeks, compute weighted target depending on total stock value of factory products
        /// </summary>
        /// <param name="code"></param>
        /// <param name="products"></param>
        /// <param name="stockCodesFactories"></param>
        /// <returns></returns>
        private int AdjustTargetWeeks(Stock_code code, List<Cust_products> products, List<Stock_code_factory> stockCodesFactories)
        {
            var factories = stockCodesFactories.Where(sc => sc.stock_code_id == code.stock_code_id).Select(sc=>sc.factory_id).ToList();
            double? result = 0.0;

            var productsForStockCode = products.Where(p => p.cprod_stock_code == code.stock_code_id).ToList();
            
            var total = productsForStockCode.Sum(p => p.cprod_stock*p.MastProduct.GetPriceGBP());
            foreach (
                var g in
                    productsForStockCode.Where(p => factories.Contains(p.MastProduct.factory_id))
                        .GroupBy(p => p.MastProduct.factory_id))
            {
                result += g.Sum(p => p.cprod_stock*p.MastProduct.GetPriceGBP())/total*
                          company.Common.Extensions.IfNotNull(stockCodesFactories.FirstOrDefault(
		                          sc => sc.stock_code_id == code.stock_code_id && sc.factory_id == g.Key), sc => sc.target_weeks);
            }
            result +=
                productsForStockCode.Where(p => !factories.Contains(p.MastProduct.factory_id))
                    .Sum(p => p.cprod_stock*p.MastProduct.GetPriceGBP())/total*code.target_weeks;
            return Convert.ToInt32(result);
        }

        public void CreateStockChart( List<StockSummaryRow> rows,Guid chartKey)
        {
            var chart = new Chart { Width = Settings.Default.StockSummaryChart_width, Height = Settings.Default.StockSummaryChart_height };

            FormatStockChart(chart, "TYPE", "WEEKS SALES");
           
            foreach (var row in rows.OrderBy(r => r.Stock_code.stock_code_id > 0 ? r.Stock_code.stock_code_id : 100)) {
                if (row != null) {

                    chart.Series[0].Points.AddXY(row.Stock_code.stock_code_name, row.Stock_code.target_weeks);
                    chart.Series[1].Points.AddXY(row.Stock_code.stock_code_name, row.WeeksSalesActual);
                }
            }
            
            SaveChartImage("Chart_StockSummary_all",chart,chartKey);

            if (rows.Count > 0) {
                for (int i = 0; i < rows[0].FutureData.Count; i++) {
                    chart = new Chart { Width = Settings.Default.StockSummaryChart_width, Height = Settings.Default.StockSummaryChart_height };
                    FormatStockChart(chart, "TYPE", "WEEKS SALES");

                    foreach (var row in rows.OrderBy(r => r.Stock_code.stock_code_id > 0 ? r.Stock_code.stock_code_id : 100)) {
                        if (row != null) {

                            chart.Series[0].Points.AddXY(row.Stock_code.stock_code_name, row.Stock_code.target_weeks);
                            chart.Series[1].Points.AddXY(row.Stock_code.stock_code_name, row.FutureData[i].WeeksSales);
                        }
                    }

                    SaveChartImage(string.Format("Chart_StockSummary_{0}_all",1), chart, chartKey);
                    
                }
            }
            
        }

        private static void FormatStockChart(Chart chart, string xTitle, string yTitle, bool customize = true)
        {
            
            var area = new ChartArea { AxisY = { IsInterlaced = true, InterlacedColor = Color.FromArgb(0xEE, 0xEE, 0xEE) } };

            area.AxisX.Title = xTitle;
            //area.AxisX.IntervalType = DateTimeIntervalType.Months;
            area.AxisX.Interval = 1;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;

            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.Title = yTitle;


            //area.AxisX.LabelStyle.Format = "MMM yy";

            chart.ChartAreas.Add(area);

            var series = new Series
            {
                Color = Color.Orange,
                MarkerColor = Color.White,
                MarkerBorderColor = Color.Orange,
                ChartType = SeriesChartType.Column,
                BorderWidth = 3,
                LegendText = "Target",//string.Format("{0}/{1}", DateTime.Today.Year - 2, DateTime.Today.Year - 1),
                XValueType = ChartValueType.String
            };
            chart.Series.Add(series);

            series = new Series
            {
                Color = Color.LightSkyBlue,
                MarkerBorderColor = Color.LightSkyBlue,
                BorderWidth = 3,
                MarkerColor = Color.White,
                ChartType = SeriesChartType.Column,
                LegendText = "Actual",//string.Format("{0}/{1}", DateTime.Today.Year - 1, DateTime.Today.Year),
                XValueType = ChartValueType.String
            };

            chart.Series.Add(series);

            var legend = new Legend { Docking = Docking.Bottom, Alignment = StringAlignment.Center };
            chart.Legends.Add(legend);
        }

        

        private void GenerateLineFillRateChart(List<Order_line_detail2_v6> data, int chartHeight, int chartWidth,Guid chartKey, string chartName, string chartTitle)
        {
            var chart = new Chart
            {
                Width = chartWidth,
                Height = chartHeight,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };


            var minValueY = data.Any() ? data.Min(m => m.line_fill_rate) : 0;

            if (minValueY != 0)
                minValueY -= 0.01;

            var area = new ChartArea();
            
            area.BackColor = Color.FromArgb(91, 155, 213);
            chart.BorderSkin.BackColor = Color.FromArgb(91, 155, 213);
            chart.BorderSkin.PageColor = Color.FromArgb(91, 155, 213);
            chart.BackColor = Color.FromArgb(91, 155, 213);


            //area.AxisY.IsInterlaced = true;
            //area.AxisY.InterlacedColor = Color.FromArgb(0xEE, 0xEE, 0xEE);

            area.AxisX.IntervalType = DateTimeIntervalType.Months;

            //area.AxisX.Title = "xTest";
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;

            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            //area.AxisY.Title = "yTest";

            area.AxisX.LabelStyle.Format = "MMM.yy";
            area.AxisY.LabelStyle.Format = "{P0}";

            area.AxisX.TitleForeColor = Color.White;
            area.AxisY.TitleForeColor = Color.White;
            area.AxisX.LabelStyle.ForeColor = Color.White;
            area.AxisY.LabelStyle.ForeColor = Color.White;

            area.AxisY.IsStartedFromZero = false;
            area.AxisY.Minimum = minValueY;
            area.AxisY.Interval = 0.02;
            
            //area.AxisY.Interval = 0;
            area.AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;

            area.AxisY.IntervalType = DateTimeIntervalType.Number;
            //area.AxisY.IntervalOffset = 0.01;
            //area.AxisY.IntervalOffsetType = DateTimeIntervalType.Number;
            
            chart.ChartAreas.Add(area);
            chart.Titles.Add(chartTitle);
            chart.Titles[0].ForeColor = Color.White;

            var series = new Series
            {
                MarkerStyle = MarkerStyle.Circle,
                Color = Color.White,
                MarkerColor = Color.White,
                MarkerBorderColor = Settings.Default.Analytics_PreviousYearSeriesColor,
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                LegendText = "LegendTest",
                XValueType = ChartValueType.Date,
            };

            //set chart data
            foreach (var orderdata in data)
            {
                if (!Double.IsInfinity(orderdata.line_fill_rate))
                {
                    var point = new DataPoint();

                    point.SetValueXY(Utilities.GetDateFromMonth21(orderdata.Month22), new object[] { orderdata.line_fill_rate });
                    point.Color = Color.White;

                    series.Points.Add(point);
                }
            }

            chart.Series.Add(series);

            Bitmap bmp = new Bitmap(chartWidth, chartHeight);
            bmp.SetResolution(1920, 1080);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                chart.Paint(g, new Rectangle(new Point(0, 0), new Size(chartWidth, chartHeight)));
            }

            var folder = Settings.Default.Analytics_ImagesFolder;

            chart.SaveImage(
                Path.Combine(Server.MapPath(folder),
                    string.Format("{0}_{1}.png", /*DateTime.Today.ToString("yyyyMMdd")*/chartKey, chartName)),
                ChartImageFormat.Png);


            //SaveChartImage(chartName, chart, chartKey);
        }

        private void GenerateAverageActualLeadTime(List<Order_line_detail2_v6> data, int chartHeight, int chartWidth, Guid chartKey, string chartName, string chartTitle, bool showTrendLine = true)
        {
            var chart = new Chart {
                Width = chartWidth,
                Height = chartHeight,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };

            var area = new ChartArea();

            area.BackColor = Color.FromArgb(64, 64, 64);
            chart.BorderSkin.BackColor = Color.FromArgb(64, 64, 64);
            chart.BorderSkin.PageColor = Color.FromArgb(64, 64, 64);
            chart.BackColor = Color.FromArgb(64, 64, 64);

            //area.AxisY.IsInterlaced = true;
            //area.AxisY.InterlacedColor = Color.FromArgb(0xEE, 0xEE, 0xEE);

            area.AxisX.IntervalType = DateTimeIntervalType.Days;
            
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;

            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;

            area.AxisX.LabelStyle.Format = "MMM.yy";
            
            area.AxisX.TitleForeColor = Color.White;
            area.AxisY.TitleForeColor = Color.White;
            area.AxisX.LabelStyle.ForeColor = Color.White;
            area.AxisY.LabelStyle.ForeColor = Color.White;
            
            //area.AxisY.LabelStyle.Format = "{P0}";

            area.AxisY.IsStartedFromZero = false;
            area.AxisY.Maximum = 100;
            area.AxisY.Minimum = 0;
            area.AxisY.Interval = 20;
            area.AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;
            area.AxisY.IntervalType = DateTimeIntervalType.Number;

            area.AxisX.Title = "order date";
            area.AxisY.Title = "days from order date till shipment";

            chart.ChartAreas.Add(area);
            chart.Titles.Add(chartTitle);
            chart.Titles[0].ForeColor = Color.White;

            var series = new Series
            {
                MarkerStyle = MarkerStyle.Circle,
                Color = Color.FromArgb(157, 195, 230),
                MarkerColor = Color.FromArgb(157, 195, 230),
                MarkerBorderColor = Color.FromArgb(157, 195, 230),
                ChartType = SeriesChartType.Point,
                BorderWidth = 3,
                XValueType = ChartValueType.Date
            };

            var series_trending = new Series
            {
                MarkerStyle = MarkerStyle.None,
                Color = Color.FromArgb(157, 195, 230),
                MarkerColor = Color.FromArgb(157, 195, 230),
                MarkerBorderColor = Color.FromArgb(157, 195, 230),
                ChartType = SeriesChartType.Line,
                BorderWidth = 1,
                XValueType = ChartValueType.Date
            };

            //set chart data
            foreach (var orderdata in data)
            {
                var point = new DataPoint();

                point.SetValueXY(orderdata.orderdate.Value.Date, new object[] { orderdata.lead_days });
                point.Color = Color.White;

                series.Points.Add(point);
            }

            chart.Series.Add(series);
            
            if (showTrendLine)
            {
                chart.Series.Add(series_trending);

                if (series != null && series.Points != null && series.Points.Count >= 5)
                    chart.DataManipulator.FinancialFormula(FinancialFormula.Forecasting, "5,15,false,false", series, series_trending);
            }
        
            //extract this stuff into method later
            Bitmap bmp = new Bitmap(chartWidth, chartHeight);
            bmp.SetResolution(1920, 1080);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                chart.Paint(g, new Rectangle(new Point(0, 0),new Size(chartWidth,chartHeight)));
            }

            var folder = Settings.Default.Analytics_ImagesFolder;

            chart.SaveImage(
                Path.Combine(Server.MapPath(folder),
                    string.Format("{0}_{1}.png", /*DateTime.Today.ToString("yyyyMMdd")*/chartKey, chartName)),
                ChartImageFormat.Png);

            //SaveChartImage(chartName, chart, chartKey);
        }

	    public static int GetDaysLeadTime(IList<Sabc_sort> sabcData, string startLateGroup)
	    {
		    var daysLeadTime = 0;
		    var sabc = sabcData.FirstOrDefault(s => s.SABC == startLateGroup);
		    if (sabc != null)
		    {
			    sabc = sabcData.FirstOrDefault(s => s.sort_code == sabc.sort_code - 1);
			    if (sabc != null)
			    {
				    daysLeadTime = sabc.prod_days ?? 0;
			    }
		    }
		    return daysLeadTime;
	    }

        private List<Order_line_Summary> GetBrandSummaryForPeriod(DateTime? from, DateTime? to, int? brand_user_id = null,
            string cprod_code = null, bool brands_only = false)
        {

            return analyticsDAL.GetBrandSummary(from, to, brands_only);            
        }

        public List<ReturnAggregateDataPrice> GetTotalsPerBrand(DateTime? dateFrom = null,
            DateTime? dateTo = null, IList<int> incClients = null,
            IList<int> exClients = null, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var ukCountries = new[] { "GB", "IE", "UK" };

            
            var asianCountries = countriesDAL.GetForContinent("AS").Select(c => c.ISO2).ToList();
            int?[] decisions = { 1, 999, 500 };
                
            var include = incClients != null;
            if (incClients == null)
                incClients = new List<int>();
            if (exClients == null)
                exClients = new List<int>();
            var exclude = exClients != null;

            return
                unitOfWork.ReturnRepository.GetQuery(r=> 
                        r.status1 == 1 && 
                        (dateFrom == null || r.request_date >= dateFrom) && 
                        r.Client.distributor > 0 && r.Client.hide_1 == 0 &&
                        (dateTo == null || r.request_date <= dateTo) && 
                        decisions.Contains(r.decision_final) &&
                        r.claim_type != null && 
                        r.claim_type != 5 && 
                        (!include || incClients.Contains(r.client_id.Value)) &&
                        (!exclude || !exClients.Contains(r.client_id.Value) &&
                        (countryFilter == CountryFilter.UKOnly && ukCountries.Contains(r.Client.user_country))
                            || (countryFilter == CountryFilter.NonUK && !ukCountries.Contains(r.Client.user_country))
                            || (countryFilter == CountryFilter.NonUKExcludingAsia && !ukCountries.Contains(r.Client.user_country) 
                                && !asianCountries.Contains(r.Client.user_country))
                            )
                        ,  
                            includeProperties: "Client, Product")
                    .GroupBy(r => new { r.Product.brand_userid, r.claim_type })
                    .ToList()
                    .OrderBy(g => g.Key.brand_userid).ToList()
                    .Select(g => new ReturnAggregateDataPrice
                    {
                        id = g.Key.brand_userid,
                        claim_type = g.Key.claim_type ?? 0,
                        TotalAccepted = g.Where(r => r.decision_final == 1).Sum(r => ReturnsSelector(r)) ?? 0,
                        TotalAcceptedEBUK = g.Where(r => r.ebuk == 1 && r.decision_final == 1).Sum(r => ReturnsSelector(r)) ?? 0,
                        TotalRejected = g.Where(r => r.decision_final == 999).Sum(r => ReturnsSelector(r)) ?? 0,
                        TotalReplacementParts = g.Where(r => r.decision_final == 500).Sum(r => ReturnsSelector(r)) ?? 0
                    }).ToList();

        }

        public List<BudgetActualData> GetBudgetActualDataByCriteria(int? from = null, int? to = null,
            CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2,CWB,LK", string includedCustomers = null, string budgetBrands = null)
        {
            
            var ukCountries = new[] { "GB", "IE", "UK" };
            var asianCountries = countriesDAL.GetForContinent("AS").Select(c => c.ISO2).ToList();

            var exCusts = excludedCustomers != null ? excludedCustomers.Split(',').ToList() : new List<string>();
            var incCusts = includedCustomers != null ? includedCustomers.Split(',').ToList() : new List<string>();

            var budgetBrandsIds = !string.IsNullOrEmpty(budgetBrands) ? budgetBrands.Split(',').Select(int.Parse).ToList() : new List<int>();

            return unitOfWork.BudgetActualDataRepository.Get(b =>
               (b.month21 >= from || from == null) &&
               (b.month21 <= to || to == null) &&
               (exCusts.Count == 0 || !exCusts.Contains(b.Distributor.customer_code)) &&
               (incCusts.Count == 0 || incCusts.Contains(b.Distributor.customer_code)) &&

               ((b.brand_id == null) || (budgetBrands == null || budgetBrands == "" || budgetBrandsIds.Contains(b.brand_id.Value))) &&
               (b.Distributor == null
                   || countryFilter == CountryFilter.All
                   || (countryFilter == CountryFilter.UKOnly && ukCountries.Contains(b.Distributor.user_country))
                   || (countryFilter == CountryFilter.NonUK && !ukCountries.Contains(b.Distributor.user_country))
                   || (countryFilter == CountryFilter.NonUKExcludingAsia && !ukCountries.Contains(b.Distributor.user_country) && !asianCountries.Contains(b.Distributor.user_country))
                   ), includeProperties: "Distributor,Brand").ToList();
            
        }

        private static double? ReturnsSelector(Returns r)
        {
            return r.claim_type == 2 ? r.claim_value : r.return_qty * r.credit_value;
        }

    }
}
