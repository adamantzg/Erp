using erp.DAL.EF.New;
using erp.Model.Dal.New;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using erp.Model;
using backend.ApiServices;
using System.Web;
using Newtonsoft.Json.Linq;

namespace backend.Controllers
{
	[Authorize]
	[RoutePrefix("api/product")]
	public class ProductApiController : ApiController
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ICategory1DAL category1DAL;
		private readonly IProductService productService;
		private readonly FileService fileService;
		private readonly ICustproductsDAL custproductsDAL;
		private readonly FileTypeDal fileTypeDal;
		private readonly IBrandCategoriesDal brandCategoriesDal;
		private readonly IBrandsDAL brandsDAL;

		public ProductApiController(IUnitOfWork unitOfWork, ICategory1DAL category1DAL, IProductService productService, FileService fileService,
			ICustproductsDAL custproductsDAL, FileTypeDal fileTypeDal, IBrandCategoriesDal brandCategoriesDal, IBrandsDAL brandsDAL)
		{
			this.unitOfWork = unitOfWork;
			this.category1DAL = category1DAL;
			this.productService = productService;
			this.fileService = fileService;
			this.custproductsDAL = custproductsDAL;
			this.fileTypeDal = fileTypeDal;
			this.brandCategoriesDal = brandCategoriesDal;
			this.brandsDAL = brandsDAL;
		}

		[Route("searchProducts")]
		[HttpGet]
		public object Products(int? factory_id = null, int? client_id = null, int? category1_id = null, string status = "N",
			int? brand_userId = null, int? brand_cat_id = null, string searchText = null)
		{
			return unitOfWork.CustProductRepository.Get(p => (p.brand_userid == client_id || client_id == null)
			&& (p.MastProduct.factory_id == factory_id || factory_id == null)
			&& (p.MastProduct.category1 == category1_id || category1_id == null)
			&& (p.brand_userid == brand_userId || brand_userId == null)
			&& (p.cprod_brand_cat == brand_cat_id || brand_cat_id == null)
			&& (p.cprod_status == status || status == null) && p.cprod_code1 != null
			&& (searchText == null || p.cprod_name.Contains(searchText) || p.cprod_code1.Contains(searchText) || p.MastProduct.factory_ref.Contains(searchText)),
			includeProperties: "BrandCompany,Mastproduct.Factory,ExtraData").
			Select(p => GetUIObject(p));
		}
		[Route("SearchProductsForBrands")]
		[HttpGet]
		public object SearchProductForBrands(string searchText)
		{
			//searchText = searchText.Replace("%20", " ");
			searchText = HttpUtility.UrlDecode(searchText);
			var CprodUserIds = Properties.Settings.Default.Cprod_UserIds_ProductSpares;

			//CprodUserIds = st
			//Get cprod_userIds from config_setting
			//REturns products that meet criteria (cprod_name or cprod_code contains searchText), use CustProductRepository
			var products = unitOfWork.CustProductRepository
							.Get(p => (p.cprod_id.ToString().Contains(searchText) || p.cprod_name.Contains(searchText) || p.cprod_code1.Contains(searchText)) && p.cprod_status == "N" && CprodUserIds.Any(s => p.cprod_user == s))
							.Take(40)
							.OrderBy(c => c.cprod_code1)
							.Select(p => new { p.cprod_id, p.cprod_name, p.cprod_code1, p.cprod_user, combined_name = $"{p.cprod_code1 } - {p.cprod_name}" })
							.ToList();


			var prods = products.Where(c => c.cprod_code1 == searchText.ToUpper()).Select(s => s).ToList();
			if (prods.Count() > 0)
			{
				foreach (var item in prods)
				{
					products.Remove(item);
					products.Insert(0, item);
				}
			}
			return products;
		}

		[Route("GetSparesOrRelated")]
		[HttpGet]
		public object GetSparesOrRelated(string cprod_code)
		{
			//Returns spares if product not spare, returns related products if cprod_id = spare
			//Check for spare by looking at mast_products.category1 == Category1.category1_spares
			var decoded = HttpUtility.UrlDecode(cprod_code);  //cprod_code.Replace("%20", " ");
			var product = unitOfWork.CustProductRepository.Get(c => c.cprod_code1 == decoded, includeProperties: "MastProduct").FirstOrDefault();
			//var mustProduct = unitOfWork.MastProductRepository.GetByID(product.cprod_mast);
			if (product != null)
			{
				bool isSpare = product.MastProduct.category1 == Category1.category1_spares;
				if (!isSpare)
				{
					//if not spare, load products from spares table that are spares of that product
					//CustproductsDAL.GetSpares
					//var products = unitOfWork.MastProductRepository.Get(mp => mp.category1 == Category1.category1_spares);
					var prods = custproductsDAL.GetSpares(new List<int> { product.cprod_id });//.Select(s=>new { s.SpareProduct.cprod_code1, s.SpareProduct.cprod_name     }); /*cprod ids*/
					/*Vraća cust_products*/

					return prods.Select(s => new { s.SpareProduct.cprod_id, s.SpareProduct.cprod_code1, s.SpareProduct.cprod_name, isSpare = true }).ToList();
				}
				else
				{
					//else , load products that are related to spare (spareparents)
					//CustproductsDAL.GetSpareParents
					return custproductsDAL.GetSpareParents(product.cprod_id).Select(s => new { s.cprod_id, s.cprod_code1, s.cprod_name, isSpare = false });
				}
			}
			return null;

		}




		[Route("getCategory1List")]
		[HttpGet]
		public object GetCategory1List()
		{
			return category1DAL.GetAll();
		}

		[Route("get")]
		[HttpGet]
		public object Get(int id)
		{
			return productService.Get(id);
		}

		[Route("create")]
		[HttpPost]
		public object Create(Cust_products prod)
		{
			return productService.Create(prod);
		}

		[Route("update")]
		[HttpPost]
		public object Update(Cust_products prod)
		{
			return productService.Update(prod);
		}

		[Route("updatebulk")]
		[HttpPut]
		public object UpdateBulk(List<Cust_products> products)
		{

			var files = products.SelectMany(p => p.OtherFiles).Distinct(new FileComparer()).ToList();
			fileService.HandleFiles(files);
			foreach (var p in products)
			{
				var ids = p.OtherFiles.Select(f => f.id).ToList();
				var file_ids = p.OtherFiles.Where(f => !string.IsNullOrEmpty(f.file_id)).Select(f => f.file_id).ToList();
				p.OtherFiles = files.Where(f => ids.Contains(f.id) || file_ids.Contains(f.file_id)).ToList();
				custproductsDAL.UpdateOtherFiles(p, File_type.Certificate);
				//unitOfWork.MastProductRepository.Update(p);
			}
			//unitOfWork.Save();
			var types = fileTypeDal.GetAll();
			return products.Select(p => GetUIObject(p, types));
		}

		[Route("removeFile")]
		[HttpDelete]
		public void removeFile(int productId, int fileId)
		{
			custproductsDAL.RemoveFile(productId, fileId);
		}

		[Route("getBrandCategories")]
		[HttpGet]
		public List<BrandCategory> GetBrandCategories(int brand_id)
		{
			return brandCategoriesDal.GetBrandCategories(new[] { brand_id }).OrderBy(c => c.brand_cat_desc).ToList();
		}

		[Route("updatePartial")]
		[HttpPost]
		public void UpdatePartial([FromBody] Dictionary<object, object> prodData)
		{
			if (prodData.ContainsKey("cprod_id"))
			{
				var cprod_id = Convert.ToInt32(prodData["cprod_id"]);
				var product = unitOfWork.CustProductRepository.Get(p => p.cprod_id == cprod_id, includeProperties: "MastProduct, ExtraData").FirstOrDefault();
				var skipFields = new[] { "cprod_id" };
				if (product != null)
				{
					foreach (var key in prodData.Keys)
					{
						if (key.Equals("mastProduct"))
						{
							var mastData = prodData[key] as JObject;
							foreach (var kv in mastData)
							{
								var value = kv.Value;
								if (value != null)
								{
									var mProp = product.MastProduct.GetType().GetProperty(kv.Key);
									if (mProp != null)
									{
										if (value.Type == JTokenType.String)
											mProp.SetValue(product.MastProduct, value.Value<string>());
										else if (value.Type == JTokenType.Integer)
											mProp.SetValue(product.MastProduct, value.Value<int>());
									}
								}
							}
						}
						else if (key.Equals("extraData"))
						{
							var extraData = prodData[key] as JObject;
							foreach (var kv in extraData)
							{
								var value = kv.Value;
								if (value != null)
								{
									var mProp = product.ExtraData.GetType().GetProperty(kv.Key);
									if (mProp != null)
									{
										if (value.Type == JTokenType.String)
											mProp.SetValue(product.ExtraData, value.Value<string>());
										else if (value.Type == JTokenType.Integer)
											mProp.SetValue(product.ExtraData, value.Value<int>());
										else if (value.Type == JTokenType.Boolean)
											mProp.SetValue(product.ExtraData, value.Value<bool>());
									}
								}
							}
						}
						else if (!skipFields.Contains(key.ToString()))
						{
							var prop = product.GetType().GetProperty(key.ToString());
							if (prop != null)
							{
								if (prop.PropertyType == typeof(int?))
									prop.SetValue(product, Convert.ToInt32(prodData[key]));
								else
									prop.SetValue(product, prodData[key]);
							}
						}

					}
					unitOfWork.Save();
				}
			}
		}

		[Route("addProducts")]
		[HttpPost]
		public object AddProducts(int brand_id, int brand_category_id, [FromBody] List<int> mast_ids)
		{
			var mastProducts = unitOfWork.MastProductRepository.Get(p => mast_ids.Contains(p.mast_id)).ToList();
			var result = new List<Cust_products>();
			var brand = brandsDAL.GetById(brand_id);
			foreach (var mp in mastProducts)
			{
				var cp = new Cust_products
				{
					cprod_mast = mp.mast_id,
					cprod_price1 = 0,
					cprod_price2 = 0,
					cprod_price3 = 0,
					cprod_price4 = 0,
					cprod_user = brand.user_id,
					cprod_brand_cat = brand_category_id,
					cprod_name = mp.asaq_name
				};

				if (cp.cprod_user == 32)
				{
					cp.cprod_user = 42;
					cp.cprod_cgflag = 1;
				}
				unitOfWork.CustProductRepository.Insert(cp);
				result.Add(cp);
			}
			unitOfWork.Save();
			return result.Select(cp => GetUIObject(cp, null));

		}

		protected object GetUIObject(Cust_products cp, IList<File_type> fileTypes = null)
		{
			return new
			{
				cp.cprod_id,
				cp.cprod_code1,
				cp.cprod_name,
				cp.brand_userid,
				cp.cprod_brand_cat,
				cp.product_group_id,
				cp.cprod_mast,
				cp.cprod_dwg,
				cp.cprod_instructions,
				cp.cprod_spares,
				cp.moq,
				cp.cprod_loading,
				cp.barcode,
				combined_code = productService.GetCombinedCode(cp.cprod_code1, cp.MastProduct?.factory_ref),
				otherFiles = cp.OtherFiles?.Select(f => new
				{
					f.id,
					f.name,
					f.type_id,
					url = fileService.GetFileUrl(fileTypes, f)
				}),
				BrandCompany = cp.BrandCompany != null ? new { cp.BrandCompany?.customer_code } : null,
				mastProduct = cp.MastProduct != null ?
					new
					{
						cp.MastProduct.mast_id,
						cp.MastProduct.asaq_name,
						cp.MastProduct.factory_ref,
						cp.MastProduct.factory_id,
						cp.MastProduct.product_group,
						cp.MastProduct.prod_image1,
						cp.MastProduct.prod_image3,
						cp.MastProduct.prod_image2,
						cp.MastProduct.prod_instructions,
						cp.MastProduct.category1,
						cp.MastProduct.units_per_carton,
						cp.MastProduct.units_per_pallet_single,
						cp.MastProduct.pack_length,
						cp.MastProduct.pack_width,
						cp.MastProduct.pack_height,
						cp.MastProduct.carton_length,
						cp.MastProduct.carton_width,
						cp.MastProduct.carton_height,
						cp.MastProduct.prod_length,
						cp.MastProduct.prod_width,
						cp.MastProduct.prod_height,
						cp.MastProduct.pallet_length,
						cp.MastProduct.pallet_width,
						cp.MastProduct.pallet_height,
						cp.MastProduct.tariff_code,
						factory = cp.MastProduct.Factory != null ? new { cp.MastProduct.Factory.factory_code } : null,
						cp.MastProduct.price_dollar,
						cp.MastProduct.price_euro,
						cp.MastProduct.price_pound
					} : null,
				extraData = cp.ExtraData != null ? new
				{
					cp.ExtraData.cprod_id,
					cp.ExtraData.analysis_e,
					cp.ExtraData.lvh_stock_type,
					cp.ExtraData.lvh_terms,
					cp.ExtraData.removed_brochure,
					cp.ExtraData.removed_distributor,
					cp.ExtraData.removed_website
				} : null
			};
		}

	}
}