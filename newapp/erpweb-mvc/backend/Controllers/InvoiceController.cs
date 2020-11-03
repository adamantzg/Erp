using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web.Mvc;
using System.Web.Routing;
using company.Common;
using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using backend.ApiServices;
using backend.Models;
using NLog;
using Utilities = company.Common.Utilities;

namespace backend.Controllers
{
    [Authorize]
    public class InvoiceController : BaseController
    {

        
        IOrderLinesDAL orderLinesDAL;
        ICompanyDAL companyDAL;
        ICurrenciesDAL currenciesDAL;
        IBrandsDAL brandsDAL;
        private IInvoicesDAL invoicesDal;
        private IInvoiceTypeDAL invoiceTypeDal;
        private IPaymentDetailsDAL paymentDetailsDal;
        private IDeliveryLocationsDAL deliveryLocationsDal;
        private ILogger logger;
        private readonly IAccountService accountService;

        public InvoiceController(IUnitOfWork unitOfWork, IOrderLinesDAL orderLinesDAL, ICompanyDAL companyDAL, ICurrenciesDAL currenciesDAL,
            IBrandsDAL brandsDAL, IInvoicesDAL invoicesDal, IInvoiceTypeDAL invoiceTypeDal, IPaymentDetailsDAL paymentDetailsDal,
            IDeliveryLocationsDAL deliveryLocationsDal, IRoleDAL roleDal, IUserDAL userDal, IPermissionDAL permissionDal,
            IAdminPagesDAL adminPagesDal, IAdminPagesNewDAL adminPagesNewDal, IClientPagesAllocatedDAL clientPagesAllocatedDal,
            ILoginHistoryDetailDAL loginHistoryDetailDal, ILoginhistoryDAL loginhistoryDal, ILogger logger, IAccountService accountService) :
            base(unitOfWork,loginHistoryDetailDal, companyDAL,adminPagesDal, adminPagesNewDal, clientPagesAllocatedDal, accountService)
        {
            this.logger = logger;
            this.accountService = accountService;
            this.deliveryLocationsDal = deliveryLocationsDal;
            this.paymentDetailsDal = paymentDetailsDal;
            this.invoiceTypeDal = invoiceTypeDal;
            this.invoicesDal = invoicesDal;            
            this.orderLinesDAL = orderLinesDAL;
            this.companyDAL = companyDAL;
            this.currenciesDAL = currenciesDAL;
            this.brandsDAL = brandsDAL;
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Index(int? custid, int? month21 = null)
        {
            List<int> custIds = null;
            if (!User.IsInRole(UserRole.Administrator.ToString()))
            {
                custIds = GetUserCompanies();
            }
            var date = month21 != null ? Month21.GetDate(month21.Value) : DateTime.Today;
            var from = Utilities.GetMonthStart(date);
            var to = Utilities.GetMonthEnd(date);
            var user = accountService.GetCurrentUser();
            return
                View(new InvoiceListModel
                {
                    Invoices = invoicesDal.GetByCriteria(custIds, from, to, invoice_sequence: user?.Company?.invoice_sequence)
                        .Where(i => i.invoice_type_id != Invoice_type.CreditNoteReturn || i.AmountCN > 0).ToList(),
                    From = from,
                    To = to,
                    custid = custid,
                    Month21 = month21 ?? Utilities.GetMonth21FromDate(DateTime.Today),
                    Brands = brandsDAL.GetAll(),
                    UrlReturnTo = Url.Action("Index", new { month21 }),
                    AllowEdit = true,
                    InvoiceTypes = invoiceTypeDal.GetAll().Where(it => it.showOnForm == true).ToList()
                });
        }

        private List<int> GetUserCompanies()
        {
            List<int> custIds = null;

            var company = companyDAL.GetById(accountService.GetCurrentUser().company_id);
            if (company.combined_factory > 0)
            {
                var companies = companyDAL.GetAllSiblings(company.user_id);
                custIds = companies.Select(c => c.user_id).ToList();
            }
            else
            {
                custIds = new List<int>();
                custIds.Add(company.user_id);
            }

            return custIds;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult Index(InvoiceListModel m)
        {
            var from = m.From;
            var to = m.To;

            List<int> custIds = null;
            if (!User.IsInRole(UserRole.Administrator.ToString()))
            {
                custIds = GetUserCompanies();
            }

            return
                View(new InvoiceListModel
                {
                    Invoices = invoicesDal.GetByCriteria(custIds, from, to),
                    From = from,
                    To = to,
                    custid = m.custid,
                    InvoiceTypes = invoiceTypeDal.GetAll()
                });
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View("Edit", BuildModel(new Invoices { Client = new Company(), invdate = DateTime.Today }));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult Create(InvoiceModel m)
        {
            //List<Invoice_lines> d;

            if (/*ModelState.IsValid ||*/ CheckETA())
            {
                HandleLines(m.Invoice);
                foreach (var line in m.Invoice.Lines)
                {
                    line.unitcurrency = m.Currency_id;
                }
                var clientExceptions =
                    Properties.Settings.Default.EBInvoiceClientExceptions.Split(',').Select(int.Parse).ToList();
                var from = unitOfWork.CompanyRepository.GetByID(m.Invoice.invoice_from ?? 0); //CompanyDAL.GetById(m.Invoice.invoice_from ?? 0);
                int? invoice_sequence = null;
                if (from != null)
                    invoice_sequence = from.invoice_sequence;
                if (m.Invoice.userid1 != null)
                    m.Invoice.Client = unitOfWork.CompanyRepository.Get(c => c.user_id == m.Invoice.userid1.Value).FirstOrDefault();
                else
                    m.Invoice.userid1 = Properties.Settings.Default.Invoice_DefaultClient;

                if (m.Invoice.Client != null && m.Invoice.Client.customer_code != null && m.Invoice.invoice_user.StartsWith(m.Invoice.Client.customer_code))
                    m.Invoice.invoice_user = m.Invoice.Client.user_name;
                m.Invoice.Client = null;
                unitOfWork.InvoiceRepository.Insert(m.Invoice, !clientExceptions.Contains(m.Invoice.userid1 ?? 0), invoice_sequence);
                unitOfWork.Save();
                //InvoicesDAL.Create(m.Invoice, !clientExceptions.Contains(m.Invoice.userid1 ?? 0),invoice_sequence);
                return RedirectToAction("Edit", new { id = m.Invoice.invoice });
            }

            return View("Edit", BuildModel(m.Invoice));
        }



        /// <summary>
        /// Clean up mvctoolkit fuckup with dates and null
        /// </summary>
        /// <returns></returns>
        private bool CheckETA()
        {
            return ModelState.Where(ms => ms.Value.Errors.Count > 0).All(ms => ms.Key.Contains("eta"));
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id, string urlToReturnTo = "")
        {
            if (string.IsNullOrEmpty(urlToReturnTo))
                urlToReturnTo = Url.Action("Index");
            var invoice = GetInvoiceById(id);

            List<int> custIds = null;
            if (!User.IsInRole(UserRole.Administrator.ToString()))
            {
                custIds = GetUserCompanies();
            }
            if (User.IsInRole(UserRole.Administrator.ToString()) || custIds.Contains(invoice.userid1.Value))
                return View(BuildModel(invoice, EditMode.Edit, urlToReturnTo));
            ViewBag.Message = "No rights";
            return View("Message");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult Edit(int id, InvoiceModel m)
        {
            var invoice = GetInvoiceById(id);//InvoicesDAL.GetById(id);
            //List<Invoice_lines> d;
            //m.Invoice.Lines = invoice.Lines;
            HandleLines(m.Invoice);
            foreach (var line in m.Invoice.Lines)
            {
                line.unitcurrency = m.Currency_id;
            }

            if (m.Invoice.Client == null && m.Invoice.userid1 != null)
                m.Invoice.Client = unitOfWork.CompanyRepository.GetByID(m.Invoice.userid1.Value);

            if (m.Invoice.Client != null && m.Invoice.Client.customer_code != null && m.Invoice.invoice_user.StartsWith(m.Invoice.Client.customer_code))
                m.Invoice.invoice_user = m.Invoice.Client.user_name;

            //InvoicesDAL.Update(m.Invoice, d);
            unitOfWork.InvoiceRepository.Update(m.Invoice);
            unitOfWork.Save();
            return RedirectToAction("Edit", new { id, urlToReturnTo = m.UrlToReturnTo });
        }

        private void HandleLines(Invoices invoice/*, out List<Invoice_lines> deletedLines*/)
        {
            if (invoice.Lines == null)
                invoice.Lines = new List<Invoice_lines>();
            //var collectedLines = new List<Invoice_lines>();
            //deletedLines = new List<Invoice_lines>();
            foreach (var key in Request.Form.Keys)
            {
                string k = key.ToString();
                if (!k.StartsWith("hid_")) continue;
                var line_id = int.Parse(Request[k]);
                if (line_id != 0)
                {
                    var prod = string.Empty + Request["prod_" + line_id.ToString()];
                    var scost = Request["cost_" + line_id.ToString()];
                    var sqty = Request["quantity_" + line_id.ToString()];
                    var sqtytype = Request["qtytype_" + line_id.ToString()];
                    double? qty = 0;
                    double? cost = null;
                    if (!string.IsNullOrEmpty(sqty))
                        qty = double.Parse(sqty);
                    if (!string.IsNullOrEmpty(scost))
                        cost = double.Parse(scost);
                    int? qtytype = 1;
                    if (!string.IsNullOrEmpty(sqtytype))
                        qtytype = int.Parse(sqtytype);
                    var line = new Invoice_lines
                    {
                        cprod_id = prod,
                        unitprice = cost,
                        invoice_id = invoice.invoice,
                        qty_type = qtytype,
                        orderqty = qty
                    };

                    if (line_id > 0)
                        line.linenum = line_id;
                    invoice.Lines.Add(line);
                }
            }
        }

        private InvoiceModel BuildModel(Invoices inv, EditMode mode = EditMode.New, string urlToReturnTo = "")
        {
            var model = new InvoiceModel
            {
                Companies = companyDAL.GetCompaniesByType(Company_User_Type.Base),
                Invoice = inv,
                InvoiceTypes = invoiceTypeDal.GetAll(),
                Currencies = currenciesDAL.GetAll(),
                EditMode = mode,
                UrlToReturnTo = urlToReturnTo,
                QuantityTypes = new List<QuantityType>(new[] { new QuantityType { id = QuantityType.QuantityTypeNormal, name = "Units" }, new QuantityType { id = QuantityType.QuantityTypePercentage, name = "Perc." } }),
                Currency_id = 1 //GBP
            };
            if (model.Companies.Count > 0)
                model.PaymentDetails = paymentDetailsDal.GetForCompany(model.Companies[0].user_id);
            if (mode == EditMode.Edit)
            {
                var client = model.Invoice?.Client;
                model.DeliveryLocations = new List<Delivery_locations>();

                if (inv.userid1 != null)
                {
                    model.DeliveryLocations.Add(new Delivery_locations { unique_id = 0, del1 = client?.user_name, del2 = client?.user_address1, del3 = client?.user_address2, del4 = client?.user_address3, del5 = client?.user_address4, inv_flag = 0 });
                    model.DeliveryLocations.AddRange(deliveryLocationsDal.GetForClient(inv.userid1.Value));
                    model.DeliveryLocations.Add(new Delivery_locations { unique_id = -1, del1 = "New address", inv_flag = 0 });
                }


            }
            return model;
        }


        public ActionResult DeliveryLocations(int client_id)
        {
            return Json(deliveryLocationsDal.GetForClient(client_id));
        }

        public ActionResult PaymentDetails(int from_id)
        {
            return Json(paymentDetailsDal.GetForCompany(from_id));
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Print(int id)
        {
            var inv = GetInvoiceById(id);//InvoicesDAL.GetById(id);
            List<int> custIds = null;
            if (!User.IsInRole(UserRole.Administrator.ToString()))
            {
                custIds = GetUserCompanies();
            }
            if (User.IsInRole(UserRole.Administrator.ToString()) || custIds.Contains(inv.userid1.Value))
                return View(new InvoiceModel { Invoice = inv, Currencies = currenciesDAL.GetAll() });
            ViewBag.Message = "No rights";
            return View("Message");

        }

        private Invoices GetInvoiceById(int id)
        {
            return unitOfWork.InvoiceRepository.Get(i => i.invoice == id, includeProperties: "Lines,Client,From,Payment,Sequences").FirstOrDefault(); //InvoicesDAL.GetById(id);
        }


        public ActionResult Delete(int id)
        {
            invoicesDal.Delete(id);
            return Json("ok");
        }

        [AllowAnonymous]
        public ActionResult CreateReturnCreditNotes(DateTime? from, DateTime? to, Guid key, string excludedClientsIds = null, string includedClientsIds = null, DateTime? invoiceDate = null, string brandIds = null)
        {
            if (key == new Guid(Properties.Settings.Default.StatsKey))
            {
                var excludedClients = excludedClientsIds != null
                                          ? Utilities.GetIdsFromString(excludedClientsIds)
                                          : null;
                var includedClients = includedClientsIds != null ? Utilities.GetIdsFromString(includedClientsIds) : null;
                var brands = brandIds != null ? Utilities.GetIdsFromString(brandIds) : null;
                var ukDistributors_Exceptions =
                    Utilities.GetIdsFromString(Properties.Settings.Default.CreditNote_Create_UKDistributors_Exceptions);
                var mappings = Properties.Settings.Default.CreditNotes_CompanyMappings;
                Dictionary<int, int> mappingDict = null;
                if (!string.IsNullOrEmpty(mappings))
                {
                    mappingDict = new Dictionary<int, int>();
                    foreach (var entry in mappings.Split('|'))
                    {
                        var values = entry.Split(';').Select(int.Parse).ToList();
                        mappingDict[values[0]] = values[1];
                    }
                }

                invoicesDal.CreateReturnCreditNotes(from, to, excludedClients, includedClients, invoiceDate, mappingDict,
                    Utilities.GetIdsFromString(Properties.Settings.Default.EBInvoiceClientExceptions),
                    Properties.Settings.Default.CreditNote_Create_InvoiceFromId,
                    Properties.Settings.Default.CreditNote_Create_InvoiceFromId_NonUK, ukDistributors_Exceptions, brands);
                ViewBag.message = "OK";
                return View("Message");
            }
            ViewBag.message = "Error";
            return View("Message");
        }

        [Authorize(Roles = "Distributor,AccountUser, Administrator, AccountsCustomer")]
        public ActionResult PrintCreditNote(int? client_id, int? brand_user_id, DateTime invoiceDate)
        {
            var user = accountService.GetCurrentUser();

            var usersExFromVAT = Properties.Settings.Default.UsersExcludedFromVAT;

            if (client_id == null)
                client_id = user.company_id;

            if (User.IsInRole(UserRole.Administrator.ToString()) || user.company_id == client_id)
            {
                var model = new CreditNotePrintModel
                {
                    Brands = brandsDAL.GetAll(false),
                    //Invoice = InvoicesDAL.GetCreditNoteByCriteria(client_id.Value, brand_user_id, invoiceDate)
                    Invoice = unitOfWork.InvoiceRepository.Get(i => i.userid1 == client_id && i.invoice_type_id == Invoice_type.CreditNoteReturn
                                        && (i.cprod_user == brand_user_id || brand_user_id == null) && i.invdate == invoiceDate
                                        && i.status != "T", includeProperties: "Client, From,CreditnoteLines, Sequences").FirstOrDefault()
                };
                //if (model.Invoice != null && model.Invoice.Client != null && model.Invoice.Client.distributor > 0 && model.Invoice.Client.user_country.In("GB", "UK","IE"))
                if (model.Invoice?.Client?.distributor > 0 && model.Invoice?.From?.user_country.In("GB", "UK", "IE") == true)
                    model.Invoice.vat_applicable = true;

                if (usersExFromVAT.Contains(client_id))
                    model.Invoice.vat_applicable = false;

                //if (model.Invoice.Sequences != null && model.Invoice.Sequences.Count > 0)
                //    model.Invoice.eb_invoice = model.Invoice.Sequences.First().sequence;
                return View("CreditNotePrint", model);
            }
            ViewBag.message = "No rights";
            return View("Message");
        }

        [Authorize(Roles = "AccountUser,Administrator")]
        public ActionResult BrandDocuments(int? month21 = null, OrderDateMode dateMode = OrderDateMode.EtaPlusWeek, bool? UK = null)

        {
            var includedClients = Properties.Settings.Default.BrandDocuments_IncludedClients.Split(',')
                                            .Select(int.Parse).ToList();
            var model = new BrandDocumentsListModel
            {
                Month21 = month21 ?? Utilities.GetMonth21FromDate(DateTime.Today),
                SearchType = BrandDocumentsListModel.SearchTypeOrder,
                DateMode = dateMode,
                UKOnly = UK
            };
            FillBrandDocumentsModel(model);
            model.Clients = BuildClientCheckboxList(model.Lines, includedClients, true);
            model.Factories = BuildFactoryCheckboxList(model.Lines);
            model.ClientIds = model.Clients.Where(c => c.Code > 0).Select(c => c.Code).ToList();
            model.ShowClientSelection = true;

            var user = accountService.GetCurrentUser();
            model.ShowBBSInvoice = UK == true && (Properties.Settings.Default.PendingInvoicesException_Companies?.Contains(user?.company_id ?? 0) ?? false);
            return View(model);
        }

        private void FillBrandDocumentsModel(BrandDocumentsListModel model)
        {
            var includedClients = Properties.Settings.Default.BrandDocuments_IncludedClients.Split(',')
                                            .Select(int.Parse);
            var user = accountService.GetCurrentUser();
            model.Lines = GetInvoicedLines(model, invoice_sequence: user?.Company?.invoice_sequence).Where(l => l.Header.Client.distributor > 0 || includedClients.Contains(l.Header.userid1 ?? 0)).ToList();
            var brands = brandsDAL.GetAll();
            model.Brands =
                model.Lines.Where(
                    l => l.Cust_Product.brand_userid != null && brands.Count(b => b.user_id == l.Cust_Product.brand_userid) > 0)
                     .GroupBy(l => l.Cust_Product.brand_userid)
                     .Select(g => new Brand { user_id = g.Key.Value })
                     .OrderBy(b => b.user_id)
                     .ToList();

            foreach (var b in model.Brands)
            {
                var brand = brands.FirstOrDefault(br => br.user_id == b.user_id);
                if (brand != null)
                {
                    b.brand_id = brand.brand_id;
                    b.code = brand.code;
                    b.user_id = brand.user_id;
                }
            }
            model.Title = "Brand documents";
            model.ActionName = "BrandDocuments";
            model.InvoiceLinkType = 1;
            model.ApplyVAT = true;
            model.Invoices_NewCalculationStartDate = Properties.Settings.Default.Invoices_NewCalculationStartDate;
            model.Invoices_NewCalculationExceptions_OrderIds =
                Properties.Settings.Default.Invoices_NewCalculationExceptions_OrderIds;
            model.ExchangeRates = unitOfWork.ExchangeRateRepository.Get().ToList();


            //ProcessAllocationBasedPriceCalculation(model);
        }



        [Authorize(Roles = "Administrator, AccountUser")]
        public ActionResult ExportBrandsTable(int? month21, int type, bool showEUR = false, OrderDateMode dateMode = OrderDateMode.Eta, bool? UK = null, bool BBSDataOnly = false)
        {
            var model = new BrandDocumentsTableModel
            {
                ListModel = new BrandDocumentsListModel
                {
                    Month21 = month21 ?? Utilities.GetMonth21FromDate(DateTime.Today),
                    SearchType = BrandDocumentsListModel.SearchTypeOrder,
                    DateMode = dateMode,
                    ShowEUR = showEUR,
                    UKOnly = UK,
                    BBSDataOnly = BBSDataOnly
                },

            };
            if (type == 1)
                model.LinesPredicate = l => l.Header.process_id > 200;
            else
            {
                model.LinesPredicate = l => l.Header.process_id <= 200;
            }
            FillBrandDocumentsModel(model.ListModel);

            model.ApplyVAT = model.ListModel.ApplyVAT;

            model.DateMode = dateMode;
            model.ShowInvoiceLink = false;
            model.InvoiceLinkType = 1;
            var user = accountService.GetCurrentUser();
            model.ShowBBSInvoice = UK == true && (Properties.Settings.Default.PendingInvoicesException_Companies?.Contains(user?.company_id ?? 0) ?? false);
            Response.AddHeader("Content-Disposition", $"attachment;filename=BrandDocuments_{(type == 1 ? "finalised" : "pending")}{(BBSDataOnly ? "_bbs" : "")}.xls");
            Response.ContentType = "application/vnd.ms-excel";
            return PartialView("_BrandDocumentsTable", model);
        }

        [Authorize(Roles = "AccountUser,Administrator")]
        public ActionResult NonBrandInvoices(int? month21 = null, OrderDateMode dateMode = OrderDateMode.Eta)
        {
            var includedClients = Properties.Settings.Default.BrandDocuments_IncludedClients.Split(',')
                                            .Select(int.Parse).ToList();
            var model = new BrandDocumentsListModel
            {
                Month21 = month21 ?? Utilities.GetMonth21FromDate(DateTime.Today),
                ExcludedClientIds = Utilities.GetIdsFromString(Properties.Settings.Default.NonBrandInvoices_IgnoredClients),
                ShowInvoiceNumber = true,
                SearchType = BrandDocumentsListModel.SearchTypeOrder,
                DateMode = dateMode
            };

            var user = accountService.GetCurrentUser();
            var company = unitOfWork.CompanyRepository.Get(c => c.user_id == user.company_id, includeProperties: "ExcludedClients").FirstOrDefault();
            if (company.ExcludedClients != null && company.ExcludedClients.Count > 0)
                model.ExcludedClientIds.AddRange(company.ExcludedClients.Select(c => c.user_id));

            model.Lines = GetInvoicedLines(model, false, user.Company.invoice_sequence);
            model.Clients = BuildClientCheckboxList(model.Lines, includedClients);

            model.Factories = BuildFactoryCheckboxList(model.Lines);

            model.ClientIds = model.Clients.Where(c => c.Code > 0).Select(c => c.Code).ToList();
            //Filter lines by non brand clients
            model.Lines = model.Lines.Where(l => model.ClientIds.Count(c => c == l.Header.userid1) > 0).ToList();
            model.Brands = new List<Brand>();
            model.Title = "Non brand invoices";
            model.ActionName = "NonBrandInvoices";
            model.ShowClientSelection = true;
            model.DateMode = dateMode;
            model.ShowFactorySelection = true;
            model.InvoiceLinkType = 2;
            model.ShowEUR = true;
            model.ShowFactoryCode = true;
            model.Invoices_AllocationBasedValueCalculationStartDate = Properties.Settings.Default.Invoices_AllocationBasedValueCalculationStartDate;
            model.Invoices_AllocationBasedValueCalculationClientsIds = Properties.Settings.Default.Invoices_AllocationBasedValueCalculationClients;
            model.Invoices_AllocationBasedValueCalculationFactoryIds = Properties.Settings.Default.Invoices_AllocationBasedValueCalculationFactories;
            ProcessAllocationBasedPriceCalculation(model);
            return View("BrandDocuments", model);
        }

        private List<CheckBoxItem> BuildFactoryCheckboxList(List<Order_lines> lines)
        {
            var result = new List<CheckBoxItem> { new CheckBoxItem { Code = 0, IsChecked = true, Label = "All" } };
            result.AddRange(GetFactoriesFromLines(lines).OrderBy(c => c.factory_code).Select(c => new CheckBoxItem { Code = c.user_id, IsChecked = true, Label = c.factory_code }).ToList());
            return result;
        }

        private List<CheckBoxItem> BuildClientCheckboxList(List<Order_lines> lines, IList<int> includedClients, bool distributors = false)
        {
            var result = new List<CheckBoxItem> { new CheckBoxItem { Code = 0, IsChecked = true, Label = "All" } };
            result.AddRange(GetClientsFromLines(lines).Where(c => (distributors ? c.distributor > 0 : c.distributor == 0) && !includedClients.Contains(c.user_id)).OrderBy(c => c.customer_code).Select(c => new CheckBoxItem { Code = c.user_id, IsChecked = true, Label = c.customer_code }).ToList());
            return result;
        }

        private List<Company> GetClientsFromLines(List<Order_lines> lines)
        {
            var result = new List<Company>();
            var exclusions = Properties.Settings.Default.NonBrandInvoices_IgnoredClients.Split(',').Select(int.Parse);
            foreach (var g in lines.Where(l => l.Header.userid1 != null).GroupBy(l => l.Header.userid1))
            {
                if (!exclusions.Contains(g.Key.Value) && (g.Sum(l => l.RowPrice) > 0 || g.Any(l => l.Header.ManualLines != null && l.Header.ManualLines.Sum(ml => ml.RowPrice) > 0)))
                {
                    result.Add(companyDAL.GetById(g.Key.Value));
                }
            }
            return result;
        }

        private List<Company> GetFactoriesFromLines(List<Order_lines> lines)
        {
            var result = new List<Company>();
            foreach (var g in lines.GroupBy(l => l.Cust_Product.MastProduct.factory_id))
            {
                result.Add(companyDAL.GetById(g.Key.Value));
            }
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "AccountUser,Administrator")]
        public ActionResult BrandDocuments(BrandDocumentsListModel m)
        {
            var includedClients = Properties.Settings.Default.BrandDocuments_IncludedClients.Split(',')
                                            .Select(int.Parse).ToList();
            m.Brands = brandsDAL.GetAll();
            FillBrandDocumentsModel(m);
            //m.Clients = BuildClientCheckboxList(m.Lines, includedClients, true);
            if (m.IgnoreClients && m.Clients != null)
                foreach (var c in m.Clients)
                {
                    c.IsChecked = true;
                }


            FillClientsFactories(m, true);
            //m.Lines = GetInvoicedLines(m);
            m.IgnoreClients = false;

            return View(m);
        }

        [HttpPost]
        [Authorize(Roles = "AccountUser,Administrator")]
        public ActionResult NonBrandInvoices(BrandDocumentsListModel m)
        {
            m.Brands = new List<Brand>();
            m.ExcludedClientIds = Utilities.GetIdsFromString(Properties.Settings.Default.NonBrandInvoices_IgnoredClients);

            var user = accountService.GetCurrentUser();
            var company = unitOfWork.CompanyRepository.Get(c => c.user_id == user.company_id, includeProperties: "ExcludedClients").FirstOrDefault();
            if (company.ExcludedClients != null && company.ExcludedClients.Count > 0)
                m.ExcludedClientIds.AddRange(company.ExcludedClients.Select(c => c.user_id));
            m.Lines = GetInvoicedLines(m).Where(l => l.Header.Client.distributor == 0).ToList();
            ModelState.Clear();
            FillClientsFactories(m);
            return View("BrandDocuments", m);
        }

        private void FillClientsFactories(BrandDocumentsListModel m, bool distributors = false)
        {
            m.ExcludedClientIds =
                Properties.Settings.Default.NonBrandInvoices_IgnoredClients.Split(',').Select(int.Parse).ToList();
            var includedClients = Properties.Settings.Default.BrandDocuments_IncludedClients.Split(',')
                                            .Select(int.Parse).ToList();
            bool allFactoriesSelected = false, allClientSelected = false;

            if (m.Factories != null && m.Factories.All(f => f.IsChecked))
            {
                allFactoriesSelected = true;
                m.Factories = null;
            }
            if (m.Clients != null && m.Clients.All(c => c.IsChecked))
            {
                allClientSelected = true;
                m.Clients = null;
            }

            if (allFactoriesSelected)
                m.Factories = BuildFactoryCheckboxList(m.Lines);
            if (allClientSelected)
                m.Clients = BuildClientCheckboxList(m.Lines, includedClients, distributors);

            var clients = allClientSelected ? GetClientsFromLines(m.Lines) : companyDAL.GetClients();
            var factories = allFactoriesSelected ? GetFactoriesFromLines(m.Lines) : companyDAL.GetFactories();

            foreach (var c in m.Clients)
            {
                if (c.Code == 0)
                    c.Label = "All";
                var client = clients.FirstOrDefault(cl => cl.user_id == c.Code);
                if (client != null)
                    c.Label = client.customer_code;
            }
            if (m.Factories != null)
            {
                foreach (var f in m.Factories)
                {
                    if (f.Code == 0)
                        f.Label = "All";
                    var factory = factories.FirstOrDefault(cl => cl.user_id == f.Code);
                    if (factory != null)
                        f.Label = factory.factory_code;
                }
            }
        }

        [Authorize(Roles = "Administrator,AccountUser")]
        public ActionResult Admin(string custIds, bool? brands = null, int? month21 = null)
        {
            var clienttIds = !string.IsNullOrEmpty(custIds) ? custIds.Split(',').Select(int.Parse).ToList() : null;
            var date = month21 != null ? Utilities.GetDateFromMonth21(month21.Value) : DateTime.Today;
            var from = Utilities.GetMonthStart(date);
            var to = Utilities.GetMonthEnd(date);
            var user = accountService.GetCurrentUser();

            var company = unitOfWork.CompanyRepository.Get(c => c.user_id == user.company_id, includeProperties: "ExcludedClients").FirstOrDefault();
            var model = new InvoiceListModel
            {
                Invoices = invoicesDal.GetByCriteria(clienttIds, from, to, brands, invoice_sequence: user?.Company?.invoice_sequence),
                From = from,
                To = to,
                Month21 = month21 ?? Utilities.GetMonth21FromDate(DateTime.Today),
                InvoiceTypes = invoiceTypeDal.GetAll().Where(it => it.showOnForm == true).ToList(),
                AllowEdit = false,
                brands = brands,
                Brands = brandsDAL.GetAll(),
                ActionName = "Admin",
                custIds = custIds,
                ShowTotals = true,
                ReferencePrefix = user?.Company?.invoice_sequence != null ? user.Company.customer_code : "EB"
            };
            if (company.ExcludedClients != null && company.ExcludedClients.Count > 0)
            {
                model.Invoices = model.Invoices.Where(i => company.ExcludedClients.Count(cl => cl.user_id == i.userid1) == 0).ToList();
            }
            return View("Index", model);
        }

        [Authorize(Roles = "Administrator,AccountUser")]
        public ActionResult ExportNonBrandTable(int? month21, int type, string clients = null, OrderDateMode dateMode = OrderDateMode.Eta, bool showEUR = false, string factories = null)
        {
            var model = new BrandDocumentsTableModel
            {
                ListModel = new BrandDocumentsListModel
                {
                    Month21 = month21 ?? Utilities.GetMonth21FromDate(DateTime.Today),
                    SearchType = BrandDocumentsListModel.SearchTypeOrder,
                    ExcludedClientIds = Properties.Settings.Default.NonBrandInvoices_IgnoredClients.Split(',').Select(int.Parse).ToList(),
                    ShowInvoiceNumber = true,
                    DateMode = dateMode,
                    ShowEUR = showEUR
                }
            };
            var user = accountService.GetCurrentUser();

            model.ListModel.Lines = GetInvoicedLines(model.ListModel, false, user.Company.invoice_sequence);
            model.ListModel.ClientIds = clients == null ? GetClientsFromLines(model.ListModel.Lines).Where(c => c.distributor == 0).Select(c => c.user_id).ToList() : clients.Split(',').Select(int.Parse).ToList();
            model.ListModel.FactoryIds = factories == null ? GetFactoriesFromLines(model.ListModel.Lines).Select(f => f.user_id).ToList() : factories.Split(',').Select(int.Parse).ToList();
            //Filter lines by non brand clients
            model.ListModel.Lines = model.ListModel.Lines.Where(l => model.ListModel.ClientIds.Count(c => c == l.Header.userid1) > 0 && model.ListModel.FactoryIds.Count(f => f == l.Cust_Product.MastProduct.factory_id) > 0).ToList();
            model.ListModel.Brands = new List<Brand>();
            model.ListModel.Title = "Non brand invoices";
            model.ListModel.ActionName = "NonBrandInvoices";
            model.InvoiceLinkType = 2;
            model.ShowFactoryCode = true;
            model.DateMode = dateMode;
            model.ShowEUR = showEUR;
            model.Factories = companyDAL.GetFactories();

            if (type == 1)
                model.LinesPredicate = l => l.Header.process_id > 200;
            else
            {
                model.LinesPredicate = l => l.Header.process_id <= 200;
            }

            Response.AddHeader("Content-Disposition", string.Format("attachment;filename=NonBrandDocuments_{0}.xls", type == 1 ? "finalised" : "pending"));
            Response.ContentType = "application/vnd.ms-excel";
            return PartialView("_BrandDocumentsTable", model);
        }

        [Authorize(Roles = "AccountUser,Administrator")]
        public ActionResult InvoiceLog()
        {
            return View(new InvoiceLogModel { From = DateTime.Today.AddMonths(-1), To = DateTime.Today });
        }

        [HttpPost]
        [Authorize(Roles = "AccountUser,Administrator")]
        public ActionResult InvoiceLog(InvoiceLogModel m)
        {
            var specialInvoices = invoicesDal.GetByCriteria(null, m.From, m.To);
            var invoicedLines = orderLinesDAL.GetInvoicedOrderLines2(string.Empty, ciDateFrom: m.From, ciDateTo: m.To);

            m.InvoiceTypes = invoiceTypeDal.GetAll();
            m.Currencies = currenciesDAL.GetAll();
            m.Invoices = specialInvoices.Union(invoicedLines.GroupBy(il => il.orderid)
                                                            .Select(
                                                                il =>
                                                                new Invoices
                                                                {
                                                                    eb_invoice = il.First().Header.order_eb_invoice,
                                                                    Amount = il.Sum(l => l.RowPrice),
                                                                    currency = il.First().unitcurrency,
                                                                    invdate = il.First().Header.Invoice.cidate,
                                                                    orderid = il.Key,
                                                                    Client = new Company { customer_code = il.First().Header.Client.customer_code },
                                                                    OrderHeader = il.First().Header
                                                                })).ToList();
            Response.AddHeader("Content-Disposition", "attachment;filename=InvoiceLog.xls");
            Response.ContentType = "application/vnd.ms-excel";
            return View("InvoiceLogExport", m);
        }

        private List<Order_lines> GetInvoicedLines(BrandDocumentsListModel model, bool brands = true, int? invoice_sequence = null)
        {
            DateTime? from = model.Month21 != null
                           ? Utilities.GetDateFromMonth21(model.Month21.Value)
                           : Utilities.GetMonthStart(DateTime.Today);
            DateTime? to = model.Month21 != null
                           ? Utilities.GetMonthEnd(Utilities.GetDateFromMonth21(model.Month21.Value))
                           : Utilities.GetMonthEnd(DateTime.Today);

            if (model.ShowClientSelection && !model.IgnoreClients && model.Clients != null)
                model.ClientIds = model.Clients.Where(c => c.IsChecked && c.Code > 0).Select(c => c.Code).ToList();
            List<int> factoryIds = null;
            var user = accountService.GetCurrentUser();
            if (model.Factories != null)
                factoryIds = model.Factories.Where(c => c.IsChecked).Select(c => c.Code).ToList();
            var lines = orderLinesDAL.GetInvoicedOrderLines2(model.PoRef, model.SearchType == BrandDocumentsListModel.SearchTypeInvoice ? @from : null,
                                                        model.SearchType == BrandDocumentsListModel.SearchTypeInvoice ? to : null,
                                                        model.SearchType == BrandDocumentsListModel.SearchTypeOrder ? @from : null,
                                                        model.SearchType == BrandDocumentsListModel.SearchTypeOrder ? to : null,
                                                        model.ClientIds, model.DateMode, model.ExcludedClientIds,
                                                        model.IncludePoLines, factoryIds, model.DateMode == OrderDateMode.Sale,
                                                        invoice_sequence, model.UKOnly, Properties.Settings.Default.PendingInvoicesException_Companies?.Contains(user?.company_id ?? 0));
            ProcessComponents(lines, model.Month21);


            var users = companyDAL.GetAll();

            foreach (var l in lines)
            {
                if (l.Header.Invoice != null)
                {
                    if (string.IsNullOrEmpty(l.Header.Invoice.invoice_number))
                    {
                        l.Header.Invoice.invoice_number = string.Format("BRS{0}{1}", l.Header.custpo,
                                                                    l.Header.Client.customer_code);
                    }

                    if (l.Header.Invoice.eb_invoice == null)
                        l.Header.Invoice.eb_invoice = l.Header.order_eb_invoice;

                    if (l.Header.Invoice?.eb_invoice == null)
                        l.Header.Invoice.eb_invoice = l.Header.Invoice.sequence;
                    l.Header.Invoice.From = users.FirstOrDefault(u => u.user_id == l.Header.Invoice.invoice_from);
                }
            }

            /*
            if(invoice_sequence == null) //no client sequence
                lines = lines.Where(m => m.Header.Invoice.eb_invoice != null).ToList();
            */

            return lines;
        }

        /// <summary>
        /// Adds components to mast_products (updated prices) 
        /// </summary>
        /// <param name="lines"></param>
        private void ProcessComponents(List<Order_lines> lines, int? month21)
        {
            var mastProductComponents = unitOfWork.MastProdComponentsRepository.Get(includeProperties: "Component").
                Where(m => m.show_on_invoice == 1).GroupBy(m => m.mast_id).ToDictionary(g => g.Key, g => g.ToList());
            var exchangeRates = unitOfWork.ExchangeRateRepository.Get().ToList();
            var rate = exchangeRates.FirstOrDefault(r => r.month21 == month21) ?? exchangeRates.Last();
            var date = Properties.Settings.Default.Invoice_ComponentCalculation_Start;
            foreach (var l in lines)
            {
                if ((date != null && l.Header.po_req_etd >= date) && mastProductComponents.ContainsKey(l.Cust_Product.cprod_mast)
                        && (l.Header.po_req_etd < mastProductComponents[l.Cust_Product.cprod_mast].FirstOrDefault().Component.cutoff_ETD_date
                                            || mastProductComponents[l.Cust_Product.cprod_mast].FirstOrDefault().Component.cutoff_ETD_date == null))
                {
                    l.PO_USD += mastProductComponents[l.Cust_Product.cprod_mast].
                        Sum(m => m.qty * (m.Component?.component_price_dollar > 0 ? m.Component.component_price_dollar : m.Component?.component_price_pound * rate?.usd_gbp));
                }
            }
        }

        [Authorize(Roles = "Administrator,AccountUser")]
        public ActionResult ExportCostsBrand(int? month21, int type, string clients, OrderDateMode dateMode, bool showEUR)
        {
            var model = new BrandDocumentsTableModel
            {
                ListModel = new BrandDocumentsListModel
                {
                    Month21 = month21 ?? Utilities.
                    GetMonth21FromDate(DateTime.Today),
                    SearchType = BrandDocumentsListModel.SearchTypeOrder,
                    DateMode = dateMode,
                    ShowEUR = showEUR,
                    IncludePoLines = true
                },

            };
            if (type == 1)
                model.LinesPredicate = l => l.Header.process_id > 200;
            else
            {
                model.LinesPredicate = l => l.Header.process_id <= 200;
            }
            FillBrandDocumentsModel(model.ListModel);
            model.DateMode = dateMode;
            model.ShowInvoiceLink = false;
            model.Factories = companyDAL.GetFactories();
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename=BrandDocuments_costs_{0}.xls", type == 1 ? "finalised" : "pending"));
            Response.ContentType = "application/vnd.ms-excel";
            return PartialView("_BrandCostsTable", model);
        }

        [Authorize(Roles = "Administrator,AccountUser")]
        public ActionResult ExportProfitsBrand(int? month21, int type, string clients, bool showEUR, OrderDateMode dateMode = OrderDateMode.Eta)
        {
            var model = new BrandDocumentsTableModel
            {
                ListModel = new BrandDocumentsListModel
                {
                    Month21 = month21 ?? Utilities.GetMonth21FromDate(DateTime.Today),
                    SearchType = BrandDocumentsListModel.SearchTypeOrder,
                    DateMode = dateMode,
                    ShowEUR = showEUR,
                    IncludePoLines = true
                },

            };
            if (type == 1)
                model.LinesPredicate = l => l.Header.process_id > 200;
            else
            {
                model.LinesPredicate = l => l.Header.process_id <= 200;
            }
            FillBrandDocumentsModel(model.ListModel);
            model.DateMode = dateMode;
            model.ShowInvoiceLink = false;
            //model.Factories = CompanyDAL.GetFactories();
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename=BrandDocuments_profits_{0}.xls", type == 1 ? "finalised" : "pending"));
            Response.ContentType = "application/vnd.ms-excel";
            return PartialView("_BrandProfitsTable", model);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ExportSpecialInv(string custIds, bool? brands = null, int? month21 = null)
        {
            var clienttIds = !string.IsNullOrEmpty(custIds) ? custIds.Split(',').Select(int.Parse).ToList() : null;
            var date = month21 != null ? Utilities.GetDateFromMonth21(month21.Value) : DateTime.Today;
            var from = Utilities.GetMonthStart(date);
            var to = Utilities.GetMonthEnd(date);
            Response.AddHeader("Content-Disposition", "attachment;filename=SpecialInvoices.xls");
            Response.ContentType = "application/vnd.ms-excel";
            var user = accountService.GetCurrentUser();
            return PartialView("_SpecialInvoicesTable", new InvoiceListModel
            {
                Invoices = invoicesDal.GetByCriteria(clienttIds, from, to, brands, invoice_sequence: user?.Company?.invoice_sequence).Where(i => i.invoice_type_id != Invoice_type.CreditNoteReturn || i.AmountCN > 0).ToList(),
                From = from,
                To = to,
                Month21 = month21 ?? Utilities.GetMonth21FromDate(DateTime.Today),
                InvoiceTypes = invoiceTypeDal.GetAll().Where(it => it.showOnForm == true).ToList(),
                AllowEdit = false,
                ForPrint = true,
                brands = brands,
                Brands = brandsDAL.GetAll(),
                ActionName = "Admin",
                custIds = custIds,
                ShowTotals = false
            });
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AuthoriseCreditNotes(int? month21 = null)
        {
            var date = month21 != null ? Utilities.GetDateFromMonth21(month21.Value) : DateTime.Today;
            var from = Utilities.GetMonthStart(date);
            var to = Utilities.GetMonthEnd(date);
            return
                View(new InvoiceListModel
                {
                    Invoices = invoicesDal.GetByCriteria(null, from, to, filterbyStatus: false).Where(inv => inv.status == "T").ToList(),
                    From = from,
                    To = to,
                    Month21 = month21 ?? Utilities.GetMonth21FromDate(DateTime.Today),
                    Brands = brandsDAL.GetAll(),
                    AllowEdit = true,
                    UrlReturnTo = Url.Action("AuthoriseCreditNotes", new { month21 }),
                    InvoiceTypes = invoiceTypeDal.GetAll().Where(it => it.showOnForm == true).ToList(),
                    ShowGenerateButton = true
                });

        }

        public ActionResult GenerateEbNumber(int id, int? month21 = null)
        {
            invoicesDal.ActivateCreditNote(id);
            return RedirectToAction("AuthoriseCreditNotes", new { month21 });
        }

        private void ProcessAllocationBasedPriceCalculation(BrandDocumentsListModel model)
        {
            var linesDict = model.Lines.Where(l => l.Header.stock_order == Order_header.StockOrderCalloff
                        && l.Header.po_req_etd >= model.Invoices_AllocationBasedValueCalculationStartDate
                        && model.Invoices_AllocationBasedValueCalculationClientsIds.Contains(l.Header.userid1)
                        && model.Invoices_AllocationBasedValueCalculationFactoryIds.Contains(l.Cust_Product?.MastProduct?.factory_id)
                        ).ToDictionary(l => l.linenum);
            var lineIds = linesDict.Keys.ToList();

            var linesWithAllocs = unitOfWork.OrderLinesRepository.Get(l => lineIds.Contains(l.linenum), includeProperties: "SOAllocations.StockLine.PorderLines")
                            .ToList();
            foreach (var l in linesWithAllocs)
            {
                linesDict[l.linenum].SOAllocations = l.SOAllocations;
            }

        }

        //public ActionResult ExportBbsTable(int? month21, int type, string clients, OrderDateMode datemode, bool showeur, string factories, bool? uk)
        //{

        //}
    }
}
