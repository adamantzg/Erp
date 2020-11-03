using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using asaq2.Model.DAL;
using asaq2back.Models;
using asaq2.Model;

namespace asaq2back.Controllers
{
    [Authorize]
    public class LocalizationController : BaseController
    {
        //
        // GET: /Localization/

        public ActionResult Index(string what, int? brand_id, string lang, int? id)
        {
            var model = brand_id != null && !string.IsNullOrEmpty(lang) ? BuildModel(brand_id.Value, lang) : new LocalizationListModel { SiteEdits = CompanyDAL.GetSiteEdits(WebUtilities.GetCurrentUser().company_id) };
            ViewBag.What = what;
            ViewBag.id = id;
            //model.SiteEdits = CompanyDAL.GetSiteEdits(WebUtilities.GetCurrentUser().company_id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LocalizationListModel m)
        {
            if (m.brand_id != null)
            {
                var brand_id = m.brand_id.Value;
                return View(BuildModel(brand_id,m.lang));
            }
            else
            {
                ViewBag.message = "Brand not specified";
                return View("Message");
            }
        }

        private LocalizationListModel BuildModel(int brand_id, string lang)
        {
            Brand b = BrandsDAL.GetById(brand_id);
            var m = new LocalizationListModel{brand_id = brand_id, lang = lang};
            m.SiteEdits = CompanyDAL.GetSiteEdits(WebUtilities.GetCurrentUser().company_id);
            m.Categories = CategoriesDAL.GetBrandCategories(b.user_id.Value, lang);
            m.Categories_Original = CategoriesDAL.GetBrandCategories(b.user_id.Value);
            m.SubCategories = CategoriesDAL.GetAllBrandSubCategories(b.user_id.Value, lang);
            m.SubCategories_Original = CategoriesDAL.GetAllBrandSubCategories(b.user_id.Value);
            m.SubsubCategories = Brand_categories_sub_subDAL.GetAllForBrand(b.user_id.Value, lang);
            m.SubsubCategories_Original = Brand_categories_sub_subDAL.GetAllForBrand(b.user_id.Value);
            m.BrandGroups = BrandGroupDAL.GetBrandGroups(b.user_id.Value, lang);
            m.BrandGroups_Original = BrandGroupDAL.GetBrandGroups(b.user_id.Value);

            m.WebProducts = WebProductsDAL.GetAllMinimal(b.code, lang);
            //m.WebProducts_Original = WebProductsDAL.GetAll(b.code);
            return m;
        }

        public ActionResult Edit(string what, int id, string language, int brand_id)
        {
            ViewBag.What = what;
            ViewBag.lang = language;
            ViewBag.brand_id = brand_id;
            ViewBag.id = id;
            Brand b = BrandsDAL.GetById(brand_id);
            if (b != null)
                ViewBag.brand_code = b.code;
            var model = new LocalizationEditModel();
            switch (what)
            {
                case "cat":
                    model.Category = CategoriesDAL.GetCategory(id);
                    ViewBag.Title = "Edit category";
                    ViewBag.recordTitle = model.Category.web_description;
                    model.LocCategory = Brand_categories_translateDAL.GetById(id, language);
                    if (model.LocCategory == null)
                        model.LocCategory = new Brand_categories_translate {};
                    break;
                case "subcat":
                    model.Subcategory = CategoriesDAL.GetSubCategory(id);
                    ViewBag.Title = "Edit subcategory";
                    ViewBag.recordTitle = model.Subcategory.brand_sub_desc;
                    model.LocSubcategory = Brand_categories_sub_translateDAL.GetById(id, language);
                    if (model.LocSubcategory == null)
                        model.LocSubcategory = new Brand_categories_sub_translate() {};
                    break;
                case "subsubcat":
                    model.SubsubCategory = Brand_categories_sub_subDAL.GetById(id);
                    ViewBag.Title = "Edit subsubcategory";
                    ViewBag.recordTitle = model.SubsubCategory.brand_sub_sub_desc;
                    model.LocSubsubCategory = Brand_categories_sub_sub_translateDAL.GetById(id, language);
                    if (model.LocSubsubCategory == null)
                        model.LocSubsubCategory = new Brand_categories_sub_sub_translate {  };
                    break;
                case "group":
                    model.Brandgroup = BrandGroupDAL.GetById(id);
                    ViewBag.Title = "Edit brand group";
                    ViewBag.recordTitle = model.Brandgroup.group_desc;
                    model.LocBrandGroup = Brand_grouping_translateDAL.GetById(id, language);
                    if (model.LocBrandGroup == null)
                        model.LocBrandGroup = new Brand_grouping_translate {  };
                    break;
                case "prod":
                    model.Product = WebProductsDAL.GetWebProduct(id);
                    ViewBag.Title = "Edit product";
                    ViewBag.recordTitle = model.Product.web_name;
                    model.LocProduct = Web_products_translateDAL.GetById(id, language);
                    model.LocComponents = new List<Cust_products_translate>();
                    if (model.LocProduct == null)
                    {
                        model.LocProduct = new Web_products_translate {};
                    }
                    //eliminate duplicates
                    foreach (var comp in model.Product.Components.GroupBy(c=>c.cprod_id))
                    {
                        var comp_translate = Cust_products_translateDAL.GetById(comp.Key, language);
                        if (comp_translate == null)
                            comp_translate = new Cust_products_translate {cprod_id = comp.Key, lang = language};
                        if(model.LocComponents.Count(c=>c.cprod_id == comp_translate.cprod_id) == 0)
                            model.LocComponents.Add(comp_translate);
                    }
                    
                    break;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(string what, int id, string language,int brand_id, LocalizationEditModel m)
        {
            switch (what)
            {
                case "cat":
                    m.LocCategory.why_so_good = HttpUtility.HtmlDecode(m.LocCategory.why_so_good);
                    if(m.LocCategory.brand_cat_id > 0)
                        Brand_categories_translateDAL.Update(m.LocCategory);
                    else
                    {
                        m.LocCategory.brand_cat_id = id;
                        m.LocCategory.lang = language;
                        Brand_categories_translateDAL.Create(m.LocCategory);
                    }
                    break;
                case "subcat":
                    m.LocSubcategory.sub_details = HttpUtility.HtmlDecode(m.LocSubcategory.sub_details);
                    if(m.LocSubcategory.brand_sub_id>0)
                        Brand_categories_sub_translateDAL.Update(m.LocSubcategory);
                    else
                    {
                        m.LocSubcategory.brand_sub_id = id;
                        m.LocSubcategory.lang = language;
                        Brand_categories_sub_translateDAL.Create(m.LocSubcategory);
                    }
                    break;
                case "subsubcat":
                    if (m.LocSubsubCategory.brand_sub_sub_id > 0)
                        Brand_categories_sub_sub_translateDAL.Update(m.LocSubsubCategory);
                    else
                    {
                        m.LocSubsubCategory.brand_sub_sub_id = id;
                        m.LocSubsubCategory.lang = language;
                        Brand_categories_sub_sub_translateDAL.Create(m.LocSubsubCategory);
                    }
                    break;
                case "group":
                    if (m.LocBrandGroup.brand_group > 0)
                        Brand_grouping_translateDAL.Update(m.LocBrandGroup);
                    else
                    {
                        m.LocBrandGroup.brand_group = id;
                        m.LocBrandGroup.lang = language;
                        Brand_grouping_translateDAL.Create(m.LocBrandGroup);
                    }
                    break;
                case "prod":
                    m.LocProduct.web_description = HttpUtility.HtmlDecode(m.LocProduct.web_description);
                    m.LocProduct.tech_compliance1 = HttpUtility.HtmlDecode(m.LocProduct.tech_compliance1);
                    m.LocProduct.tech_compliance2 = HttpUtility.HtmlDecode(m.LocProduct.tech_compliance2);
                    m.LocProduct.tech_compliance3 = HttpUtility.HtmlDecode(m.LocProduct.tech_compliance3);
                    m.LocProduct.tech_compliance4 = HttpUtility.HtmlDecode(m.LocProduct.tech_compliance4);
                    m.LocProduct.tech_compliance5 = HttpUtility.HtmlDecode(m.LocProduct.tech_compliance5);
                    m.LocProduct.tech_additional1 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional1);
                    m.LocProduct.tech_additional2 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional2);
                    m.LocProduct.tech_additional3 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional3);
                    m.LocProduct.tech_additional4 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional4);
                    m.LocProduct.tech_additional5 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional5);
                    m.LocProduct.tech_additional6 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional6);
                    m.LocProduct.tech_additional7 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional7);
                    m.LocProduct.tech_additional8 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional8);
                    m.LocProduct.tech_additional9 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional9);
                    m.LocProduct.tech_additional10 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional10);
                    m.LocProduct.tech_additional11 = HttpUtility.HtmlDecode(m.LocProduct.tech_additional11);

                    if(m.LocProduct.web_unique > 0)
                        Web_products_translateDAL.Update(m.LocProduct);
                    else
                    {
                        m.LocProduct.web_unique = id;
                        m.LocProduct.lang = language;
                        Web_products_translateDAL.Create(m.LocProduct);
                    }
                    //handle lines
                    foreach (var key in Request.Form.Keys)
                    {
                        string k = key.ToString();
                        if (!k.StartsWith("component_")) continue;
                        int cprod_id = int.Parse(k.Replace("component_", ""));
                        var comp = Cust_products_translateDAL.GetById(cprod_id, language);
                        var bCreateNew = false;
                        if (comp == null)
                        {
                            comp = new Cust_products_translate();
                            comp.cprod_id = cprod_id;
                            comp.lang = language;
                            bCreateNew = true;
                        }
                        comp.cprod_name = Request.Form[k];
                        if(bCreateNew)
                            Cust_products_translateDAL.Create(comp);
                        else
                        {
                            Cust_products_translateDAL.Update(comp);
                        }
                    }
                    break;
            }
            return RedirectToAction("Index","Localization", new {what,lang = language,brand_id, id});
        }
    }
}
