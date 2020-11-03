using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using erp.Model.Dal.New;
using erp.DAL.EF.New;

namespace backend.ApiServices
{
	public class ProductService : IProductService
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ICustproductsDAL custproductsDAL;
		private readonly IMastProductsDal mastProductsDal;

		public ProductService(IUnitOfWork unitOfWork, ICustproductsDAL custproductsDAL, IMastProductsDal mastProductsDal)
		{
			this.unitOfWork = unitOfWork;
			this.custproductsDAL = custproductsDAL;
			this.mastProductsDal = mastProductsDal;
		}

		public object Get(int id, bool includeProductPricingData = false)
		{
			/*var includes = includeProductPricingData ? "MastProduct.Factory, MarketData, SalesForecast, MastProduct.ProductPricingData, MastProduct.Prices, MastProduct.ClientPrices" 
				: "MastProduct.Factory";
			return unitOfWork.CustProductRepository.Get(p => p.cprod_id == id, includeProperties: includes)
				.Select(GetUIObject).FirstOrDefault();*/
			return GetUIObject((includeProductPricingData ? custproductsDAL.GetByIdFull(id) :
					unitOfWork.CustProductRepository.Get(p => p.cprod_id == id, includeProperties: "MastProduct.Factory").FirstOrDefault()));
		}

		public object Create(Cust_products prod)
		{
			HandleFiles(prod);
			unitOfWork.CustProductRepository.Insert(prod);			
			unitOfWork.Save();
			return GetUIObject(prod);
		}

		
		public object Update(Cust_products prod)
		{
			HandleFiles(prod);
			unitOfWork.CustProductRepository.Update(prod);
			unitOfWork.Save();
			return GetUIObject(prod);
		}

		private void HandleFiles(Cust_products prod)
		{
			//handle cust product files - TODO

			//Mastproduct
			if(prod.MastProduct != null)
			{
				foreach (var file in prod.MastProduct?.Files)
				{
					if (!string.IsNullOrEmpty(file.file_id))
					{
						var data = WebUtilities.GetTempFile(file.file_id);
						if (data != null)
						{
							file.filename = GetRelativePath(Properties.Settings.Default.imagesRootFolder, 
								company.Common.Utilities.WriteFile(file.filename, HttpContext.Current.Server.MapPath(Path.Combine(Properties.Settings.Default.imagesRootFolder, "prod")), data));
						}
					}
				}
			}
			
			WriteToLegacyFields(prod);
		}

		private void WriteToLegacyFields(Cust_products prod)
		{
			//Cust product - TODO

			//Mast product
			if(prod.MastProduct != null)
			{
				var mp = prod.MastProduct;
				if(mp.Files != null)
				{
					
					foreach (var file in mp.Files)
					{
						if(!string.IsNullOrEmpty(file.filename))
						{
							switch (file.file_type_id)
							{
								case mast_product_file_type.Picture:
									mp.prod_image1 = file.filename;
									break;
								default:
									break;
							}
						}
					}
				}
			}
		}

		private string GetRelativePath(string root, string fullPath)
		{
			return fullPath.Substring(fullPath.IndexOf(root.Replace("/", "\\"))).Replace("\\", "/");
		}

		internal static object GetUIObject(Cust_products p)
		{
			if(p.MastProduct != null)
				p.MastProduct.Files = GetFromLegacyFields(p.MastProduct);
			return p;
			//return new
			//{
			//	p.analysis_d,
			//	p.analytics_category,
			//	p.analytics_option,
			//	p.bin_location,
			//	p.brand_id,
			//	p.brand_userid,
			//	p.bs_visible,
			//	p.client_range,
			//	p.color_id,
			//	p.cprod_brand_cat,
			//	p.cprod_dwg,
			//	p.cprod_id,
			//	p.cprod_code1,
			//	p.cprod_instructions,
			//	p.cprod_label,
			//	p.cprod_packaging,
			//	p.cprod_status,
			//	p.cprod_stock,
			//	p.sale_retail,
			//	p.whitebook_cprod_name,
			//	p.cprod_name,
			//	p.cprod_retail,
			//	p.cprod_mast,
			//	p.barcode,
			//	p.cprod_user,
			//	mastProduct = p.MastProduct != null ? GetMastProductUIObject(p.MastProduct) : null,
			//	marketData = p.MarketData?.Select(m => new
			//	{
			//		m.cprod_id,
			//		m.market_id,
			//		m.retail_price
			//	}),
			//	salesForecast = p.SalesForecast?.Select(s => new {
			//		s.cprod_id,
			//		s.month21,
			//		s.sales_qty,
			//		s.sales_unique
			//	})
			//};
		}

		internal static object GetMastProductUIObject(Mast_products p)
		{
			return new
			{
				p.mast_id,
				p.factory_id,
				p.factory_ref,
				p.units_per_40pallet_gp,
				p.units_per_20pallet,
				p.units_per_pallet_single,
				p.asaq_name,
				p.asaq_ref,
				p.category1,
				p.price_dollar,
				p.price_euro,
				p.price_pound,
				p.tariff_code,
				p.lme,
				p.carton_height,
				p.carton_length,
				p.carton_width,
				p.category1_sub,
				p.comments,
				p.factory_moq,
				p.factory_stock,
				p.pallet_height,
				p.pallet_length,
				p.pallet_width,
				p.prod_image1,
				p.prod_image2,
				p.prod_image3,
				p.product_group,				
				factory = new
				{
					p.Factory?.user_id,
					p.Factory?.factory_code,
					p.Factory?.consolidated_port,
				},
				productPricingData = p.ProductPricingData != null ? new
				{
					p.ProductPricingData.display_qty,
					p.ProductPricingData.initial_stock,
					p.ProductPricingData.tooling_cost,
					p.ProductPricingData.tooling_currency_id,
					p.ProductPricingData.mastproduct_id
				} : null,
				p.Prices,
				files = GetFromLegacyFields(p)
			};
		}

		internal static List<MastProductFile> GetFromLegacyFields(Mast_products p)
		{
			var result = new List<MastProductFile>();
			if (!string.IsNullOrEmpty(p.prod_image1))
				result.Add(new MastProductFile { filename = p.prod_image1, id = p.mast_id * 10 + 1, file_type_id = mast_product_file_type.Picture });
			return result;
		}

		public string GetCombinedCode(string cprod_code1, string factory_ref)
		{
			if (string.IsNullOrEmpty(factory_ref))
				return cprod_code1;
			return $"{cprod_code1} ({factory_ref})";
		}
	}
}