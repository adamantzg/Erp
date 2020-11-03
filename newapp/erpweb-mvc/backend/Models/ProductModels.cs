using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;
using company.Common;

namespace backend.Models
{
    public class ProductExportModel
    {
        public List<CheckBoxItem> Clients { get; set; }
        public List<CheckBoxItem> Factories { get; set; }
        public bool IncludeSpares { get; set; }
        public bool IncludeDiscontinued { get; set; }
        public bool ShowFactoryPricing { get; set; }
        public bool ShowSalesHistory { get; set; }
        public bool ShowLogistics { get; set; }
        public List<Brand> Brands { get; set; }
        public bool GroupByMastProduct { get; set; }  
        public List<ProductRow> Products { get; set; }
        public List<BrandCategory> BrandCategories { get; set; }
        
        public ProductExportModel()
        {
            ShowFactoryPricing = false;
            ShowSalesHistory = true;
            GroupByMastProduct = false;
        }

    }

    public class ProductRow
    {
        public Cust_products Product { get; set; }
        public int? Last6MonthQty { get; set; }
        public int? Previous6MonthQty { get; set; }
        public double? Last6MonthAmount { get; set; }
        public double? Previous6MonthAmount { get; set; }
        public Category1 Category1 { get; set; }
    }

    public class FirstShipmentsModel
    {
        public List<OrderMgtmDetail> NewLines { get; set; }
        public List<OrderMgtmDetail> ChangedLines { get; set; }
        public string Product { get; set;}
        public string PO { get; set; }
        public List<Company> Factories { get; set; }
        public int? Factory_id { get; set; }
        public List<Company> Clients { get; set; }
        public int? Client_id { get; set; }
        public DateTime? ETDFrom { get; set; }
        public DateTime? ETDTo { get; set; }
        public List<Lookup> Categories { get; set; }
        public List<Lookup> Orderby { get; set; }
        public int? Category_id { get; set; }
        public int OrderBy_Id { get; set; }

        public Dictionary<string,Inspections> Inspections { get; set; }

    }

    public class ProductPricesExportModel
    {
        public List<Company> Factories { get; set; }
        public int? factory_id { get; set; }
        
    }

    public class StockSummaryModel
    {
        public List<StockSummaryRow> Rows { get; set; }
        public List<Company> Clients { get; set; }
        public List<StockSummaryRow> TotalRows { get; set; }
        public Dictionary<int, List<StockProductRow>> ProductData { get; set; }
        public Company HeaderCompany { get; set; }
        public int WorstLimit { get; set; }
        public List<Stock_code> StockCodes { get; set; }
    }

    public class StockSummaryRow
    {
        public Company Factory { get; set; }
        public Stock_code Stock_code { get; set; }
        public double? WeeksSalesActual { get; set; }
        public bool WeeksSalesActualOverflow { get; set; }
        public int ProductCount { get; set; }
        public int ProductCountSpares { get; set; }
        public double? CBM_Actual { get; set; }
        public double? GBP_Actual { get; set; }
        public double? CBM_Target { get; set; }
        public double? GBP_Target { get; set; }
                     
        public double? CBM_Water { get; set; }
        public double? GBP_Water { get; set; }
                     
        public double? CBM_Factory_InLimits { get; set; }
        public double? GBP_Factory_InLimits { get; set; }
        public double? CBM_Factory_Overdue { get; set; }
        public double? GBP_Factory_Overdue { get; set; }
                     
        public double? CBM_Production { get; set; }
        public double? GBP_Production { get; set; }
        
        public List<StockSummaryPair> FutureData { get; set; }
    }

    public class StockProductRow
    {
        public Cust_products Product { get; set; }
        public double? GBP_Actual { get; set; }
        public double? WeeksSalesActual { get; set; }
        public bool WeeksSalesActualOverflow { get; set; }
        public double? GBP_Water { get; set; }
        public double? Qty_Water { get; set; }
        public double? GBP_Factory { get; set; }
        public double? Qty_Factory { get; set; }
        public double? GBP_Production { get; set; }
        public double? Qty_Production { get; set; }
        public int NumOfOrders { get; set; }
    }

    public class StockSummaryPair
    {
        public DateTime? StockDate { get; set; }
        public double? CBM { get; set; }
        public double? GBP { get; set; }
        public double? WeeksSales { get; set; }
        public bool WeeksSalesActualOverflow { get; set; }
    }

    public class ProductStock
    {
        public DateTime? StockDate { get; set; }
        public Cust_products Product { get; set; }
        public double? Stock { get; set; }
    }

    public class ProductDownloadModel
    {
        //public List<Cust_products> Products { get; set; }
        public List<Cust_product_doctype> DocTypes { get; set; }
        public List<ProductFit> Products { get; set; }
        public int factory_id { get; set; }
        public bool ShowProducts { get; set; }
    }
    
    public class TechnicalProductDataModel
    {
        public List<Company> Users { get; set; }
    }


    public class DisplayingProductModel
    {
        public List<Web_product_new> WebProduct { get; set; }
    }

    public class ProductDetailModel
    {
        public Cust_products CustProduct { get; set; }
        public Mast_products MastProduct { get; set; }
        public List<ProductSales> YearlySalesData { get; set; }
        public List<ProductSales> Previous6mSales { get; set; }
        public List<ProductSales> Last6mSales { get; set; }
        public List<ProductSales> YearToDateSales { get; set; }
         
        public List<ReturnAggregateDataByMonth> ReturnsYearlyData { get; set; }
        public ReturnAggregateDataByMonth Previous6mReturns { get; set; }
        public ReturnAggregateDataByMonth Last6mReturns { get; set; }
        public ReturnAggregateDataByMonth YearToDateReturns { get; set; }
        public int ChartWidth { get; set; }
        public int ChartHeight { get; set; }
        public DateTime? ChartDataFrom { get; set; }
        public DateTime? ChartDataTo { get; set; }
        public List<int> cprod_ids { get; set; }

        public List<Returns> Feedbacks { get; set; }
        public Brand Brand { get; set; }
    }

    public class SparesModel
    {
        public List<Spares> Spares { get; set; }
        public List<string> FactoryCodes { get; set; }
        public string FactoryCode { get; set; }
    }
    public class SpareProductModel
    {
        public List<SparesProducts> SpareProductList { get; set; }

        public IEnumerable<Amendments> Amendments { get; set; }

        public List<Brand> Brands { get; set; }
    }
    public class SalesPricesPercentgesModel
    {
        public List<Web_site> WebSites { get; set; }

        public List<Web_product_new> Products { get; set; }

        public List<Web_category> CategoriesAllChildren { get; set; }

        public List<MaxSalePercentage> CategoryMax { get; set; }

        public List<Web_category> Categories { get; set; }
    }

    public class MaxSalePercentage
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double Percentage { get; set; }
    }

    public class TechnicalDataReportModel
    {
        public string ClientIds { get; set; }
        public List<Category1_sub> Subcategories { get; set; }
        public int? sub_category { get; set; }
        
    }

    public class TechnicalDataGenerationReportModel
    {
        public List<Cust_products> CustProducts { get; set; }
        public List<string> ClientCodes { get; set; }
        public Category1_sub SubCategory { get; set; }
        public List<Technical_subcategory_template> TechnicalDataTypes { get; set; }
        public List<Technical_product_data> TechnicalProductData { get; set; }
    }

    public class TechnicalDataEditRow
    {
        public int TechnicalDataTypeId { get; set; }
        public string TypeName { get; set; }
        public int Sequence { get; set; }
        public string Value { get; set; }
    }

    public class ProductReviewModel
    {
        public List<AnalyticsSubCatSummaryRow> AnalyticsSubCatRows { get; set; }
        public List<Brand> Brands { get; set; }
        public Dictionary<int, List<Analytics_options>> BrandOptions { get; set; }
        //public bool ShowTabs { get; set; }
    }

    public class LVHStockMonitoringModel
    {
        public List<Cust_products> lvhProducts { get; set; }
    }

	public class ProductPricingProjectReportModel
	{
		public ProductPricingProject Project { get; set; }
		public Market Market { get; set; }
		public ProductPricingCalculation Calculation { get; set; }
		public List<Currencies> CurrencyList { get; set; }
		public List<ProductPricingProjectReportRow> Rows { get; set; }
		public List<Freightcost> FreightCosts { get; set; }
		public List<Tariffs> Tariffs { get; set; }

		public Dictionary<int?, Dictionary<int?, double?>> ConversionTable { get; set; }
		private Dictionary<int, ProductPricing_settings> dictSettings;

		public ProductPricingProjectReportModel(ProductPricingProject project, Market market, ProductPricingCalculation calculation, 
			List<Currencies> currencies, List<Freightcost> freightCosts, List<Tariffs> tariffs )
		{
			this.Project = project;
			this.Market = market;
			this.Calculation = calculation;
			this.CurrencyList = currencies;
			this.FreightCosts = freightCosts;
			this.Tariffs = tariffs;
			dictSettings = Project.Settings.ToDictionary(s => s.id);
			ConversionTable = new Dictionary<int?, Dictionary<int?, double?>>();
			var gbp_eur = Project.Settings.FirstOrDefault(s => s.id == (int?)ProductPricingSettingId.GbpEurRate)?.numValue;
			var gbp_usd = Project.Settings.FirstOrDefault(s => s.id == (int?)ProductPricingSettingId.GbpUsdRate)?.numValue;
			ConversionTable[Currencies.EUR] = new Dictionary<int?, double?>();
			ConversionTable[Currencies.EUR][Currencies.EUR] = 1;
			ConversionTable[Currencies.EUR][Currencies.USD] = 1 / gbp_eur * gbp_usd;
			ConversionTable[Currencies.EUR][Currencies.GBP] = 1 / gbp_eur;
			ConversionTable[Currencies.USD] = new Dictionary<int?, double?>();
			ConversionTable[Currencies.USD][Currencies.EUR] = 1 / gbp_eur * gbp_usd;
			ConversionTable[Currencies.USD][Currencies.USD] = 1;
			ConversionTable[Currencies.USD][Currencies.GBP] = 1 / gbp_usd;
			ConversionTable[Currencies.GBP] = new Dictionary<int?, double?>();
			ConversionTable[Currencies.GBP][Currencies.EUR] = gbp_eur;
			ConversionTable[Currencies.GBP][Currencies.USD] = gbp_usd;
			ConversionTable[Currencies.GBP][Currencies.GBP] = 1;

			Rows = project?.Products.Select(p => new ProductPricingProjectReportRow(p, this)).ToList();
		}

		public double? GetSetting(ProductPricingSettingId id)
		{
			return dictSettings[(int) id]?.numValue;
		}

		public double? DiscountFactor
		{
			get
			{
				double? discountFactor = 1.0;
				foreach (var l in Project.PricingModel.Levels)
				{
					discountFactor *= (1 - l.value);
				}
				return discountFactor;
			}
		}

		public double? ToMarketCurrency(int? currencyFrom, double? amount)
		{
			if (currencyFrom != null && Market != null)
			{
				if (ConversionTable[currencyFrom] != null)
				{
					return ConversionTable[currencyFrom][Market.currency_id] * amount;
				}
			}
			return 1;
		}

		
	}


	public enum ProductPricingCalculation
	{
		Sage = 1,
		Gateway = 2
	}



	public class ProductPricingProjectReportRow
	{
		private int? currency_id;
		private Cust_products product;
		private ProductPricingProjectReportModel report;
		private Tariffs tariff;

		public ProductPricingProjectReportRow(Cust_products product, ProductPricingProjectReportModel report)
		{
			this.product = product;
			this.report = report;
			currency_id = product.MastProduct?.CurrencyId;
			tariff = report?.Tariffs?.FirstOrDefault(t => t.tariff_id == product.MastProduct.tariff_code);
		}

		public Cust_products Product {
			get
			{
				return product;
			}
			/*set {
				product = value;				
			}*/
		}	

		public int? CurrencyId
		{
			get
			{
				return currency_id;
			}
		}


		//public double? freightCost { get; set; }
		/*public double? commission { get; set; }
		public double? fiscalAgent { get; set; }*/
		//public string dutyCode { get; set; }
		//public double? dutyPercentage { get; set; }
		//public double? internalCost { get; set; }
		public ProductPricingProjectReportModel ProjectReport
		{
			get
			{
				return report;
			}
		}

		public Tariffs Tariff
		{
			get
			{
				return tariff;
			}

		}

		public double? FOBPrice
		{
			get
			{
				return Product.MastProduct?.DefaultPrice?.price;
			}
		}

		public double? DutyPercentage
		{
			get
			{
				return Tariff?.tariff_rate ?? 0;
			}
		}		

		public double? Duty
		{
			get
			{
				return FOBPrice * DutyPercentage ?? 0;
			}
		}

		//public double? TrueLandedCost
		//{
		//	get
		//	{
		//		return FOBPrice * (1 + DutyPercentage) + Commission + FreightCost + FiscalAgent;
		//	}
		//}

		public double? Commission
		{
			get
			{
				return FOBPrice * ProjectReport.GetSetting(ProductPricingSettingId.Commision) ?? 0;
			}
		}

		public double? FiscalAgent
		{
			get
			{
				return FOBPrice * ProjectReport.GetSetting(ProductPricingSettingId.FiscalAgent) ?? 0;
			}
		}

		public int? ContainerType
		{
			get
			{
				if (Product.MastProduct != null)
				{
					if (Product.MastProduct.units_per_40pallet_gp > 0)
					{
						return Container_types.Gp40;
					}
					else if (Product.MastProduct.units_per_20pallet > 0)
					{
						return Container_types.Gp20;
					}
					else
					{
						return null;
					}
				}
				return null;
			}
		}

		public double? FreightCost
		{
			get
			{
				if (ContainerType != null && ProjectReport.Market != null && Product.MastProduct.Factory != null)
				{
					var cost = ProjectReport.FreightCosts.FirstOrDefault(c => c.container_id == ContainerType
					  && c.market_id == ProjectReport.Market.id && c.location_id == Product.MastProduct.Factory.consolidated_port);
					if (cost != null)
					{
						return cost.value ?? 0;
					}
				}
				return 0;
			}
		}

		public double? FreightCostPerUnit
		{
			get
			{

				var mp = Product?.MastProduct;
				if (mp?.units_per_40pallet_gp > 0)
					return FreightCost / mp.units_per_40pallet_gp ?? 0;
				if (mp?.units_per_20pallet > 0)
					return FreightCost / mp.units_per_20pallet ?? 0;
				if( mp?.units_per_pallet_single > 0)
					return FreightCost / mp.units_per_pallet_single ?? 0;
				return 0;
			}
		}

		public double? FreighCostSage
		{
			get
			{
				return FOBPrice * ProjectReport.GetSetting(ProductPricingSettingId.SageFreight) ?? 0;
			}
		}

		public double? RetailPrice
		{
			get
			{
				return Product.MarketData.FirstOrDefault(d => d.market_id == ProjectReport.Market.id)?.retail_price ?? Product.cprod_retail / 1.2 ?? 0;
			}
		}

		public double? DiscountedRetailPrice
		{
			get
			{
				return RetailPrice * ProjectReport.DiscountFactor ?? 0;
			}
		}

		public double? RetailPriceVAT
		{
			get
			{
				return RetailPrice * (1 + ProjectReport.Market.vat) ?? 0;
			}
		}

		public double? DiscountedRetailPriceVAT
		{
			get
			{
				return RetailPriceVAT * ProjectReport.DiscountFactor ?? 0;
			}
		}

		public double? LandedCostSage
		{
			get
			{
				return FOBPrice + FreighCostSage + Commission + Duty;			
			}			
		}

		public double? InternalCostGateway
		{
			get
			{
				return (FOBPrice + FiscalAgent + FreightCostPerUnit + Duty) * (ProjectReport.Market.internal_cost ?? 0);
			}
		}

		public double? LandedCostGateway
		{
			get
			{
				return FOBPrice  + FiscalAgent + FreightCostPerUnit + Duty + InternalCostGateway + Commission;
			}
			
		}

		public double? LandedCost
		{
			get
			{
				return ProjectReport.Calculation == ProductPricingCalculation.Sage ? LandedCostSage : LandedCostGateway;
			}
		}

		public int? ForecastQty
		{
			get
			{
				return Product.SalesForecast.Where(s => s.month21 >= (Month21.Now + 1).Value && s.month21 <= (Month21.Now + 12).Value).Sum(s => s.sales_qty);
			}
		}

	}

	public class ProductSabcRecord
	{
		public Cust_products CustProduct { get; set; }
		public List<Order_header> Orders { get; set; }
		public string CalculatedProductGroup { get; set; }
		public int NumOfOrdersForCalculation { get; set; }
	}

	public class ProductSabcReportModel
	{
		public Company Factory { get; set; }
		public List<ProductSabcRecord> Products { get; set; }
	}
}