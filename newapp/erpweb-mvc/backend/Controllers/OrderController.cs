using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
//using erp.DAL.EF;
//using erp.Model.DAL;
using backend.ContainerLoadService;
using System.Threading.Tasks;
using System.Runtime.Caching;
using erp.Model;
using backend.Models;
using backend.Properties;
//using Microsoft.Ajax.Utilities;
using erp.Model.Dal.New;
using erp.DAL.EF.New;
using DotNetOpenAuth.Messaging;
using Microsoft.Ajax.Utilities;
using NLog;
using Expression = System.Linq.Expressions.Expression;
using Utilities = erp.Model.Utilities;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize(Roles = "Administrator,FactoryUser")]
    public class OrderController : BaseController
    {
        //
        // GET: /Order/

        private ICompanyDAL companyDal;
        private ILocationDAL locationDal;
        private IBrandsDAL brandsDal;
        private ICurrenciesDAL currenciesDal;
        private ICustproductsDAL custproductsDal;
        private IOrderHeaderDAL orderHeaderDAL;
        private IRangeDAL rangeDAL;
        private IDistProductsDal distProductsDal;
        private ISalesForecastDal salesForecastDal;
        private ISalesDataDal salesDataDal;
        private IMastProductsDal mastProductsDal;
        private IOrderLinesDAL orderLinesDAL;
        private IShipmentsDal shipmentsDal;
        
        private readonly IDeliveryLocationsDAL deliveryLocationsDAL;
        private readonly IContainerTypesDal containerTypesDal;
        private readonly IOrderLineExportDal orderLineExportDal;
        private readonly IStockOrderAllocationDAL stockOrderAllocationDAL;
        private readonly IContractSalesForecastLinesDal contractSalesForecastLinesDal;
        private readonly IAdminPermissionsDal adminPermissionsDal;
        private readonly IOrderService orderService;
        private readonly IAccountService accountService;
        private readonly IMailHelper mailHelper;
        private ICategory1DAL category1Dal;

        public OrderController(ICompanyDAL companyDal,
            ILocationDAL locationDal, IBrandsDAL brandsDal, ICurrenciesDAL currenciesDal,
            ICustproductsDAL custproductsDal, IOrderHeaderDAL orderHeaderDAL, IRangeDAL rangeDAL,
            IDistProductsDal distProductsDal, ISalesForecastDal salesForecastDal, ISalesDataDal salesDataDal,
            IMastProductsDal mastProductsDal, IOrderLinesDAL orderLinesDAL, IShipmentsDal shipmentsDal, ICategory1DAL category1Dal,
            IUnitOfWork unitOfWork, IClientPagesAllocatedDAL clientPagesAllocatedDal, IUserDAL userDal, IRoleDAL roleDal, IPermissionDAL permissionDal,
            ILoginHistoryDetailDAL loginHistoryDetailDal, IAdminPagesDAL adminPagesDal, IAdminPagesNewDAL adminPagesNewDal,
            ILoginhistoryDAL loginhistoryDal, ILogger logger, IDeliveryLocationsDAL deliveryLocationsDAL,
            IContainerTypesDal containerTypesDal, IOrderLineExportDal orderLineExportDal,
            IStockOrderAllocationDAL stockOrderAllocationDAL, IContractSalesForecastLinesDal contractSalesForecastLinesDal,
            IAdminPermissionsDal adminPermissionsDal, IOrderService orderService, IAccountService accountService,
            IMailHelper mailHelper) :
            base(unitOfWork, loginHistoryDetailDal, companyDal, adminPagesDal, adminPagesNewDal, clientPagesAllocatedDal, accountService)
        {
            this.category1Dal = category1Dal;

            this.companyDal = companyDal;
            this.locationDal = locationDal;
            this.brandsDal = brandsDal;
            this.currenciesDal = currenciesDal;
            this.custproductsDal = custproductsDal;
            this.orderHeaderDAL = orderHeaderDAL;
            this.rangeDAL = rangeDAL;
            this.distProductsDal = distProductsDal;
            this.salesForecastDal = salesForecastDal;
            this.salesDataDal = salesDataDal;
            this.orderLinesDAL = orderLinesDAL;
            this.shipmentsDal = shipmentsDal;
            this.mastProductsDal = mastProductsDal;
            
            this.deliveryLocationsDAL = deliveryLocationsDAL;
            this.containerTypesDal = containerTypesDal;
            this.orderLineExportDal = orderLineExportDal;
            this.stockOrderAllocationDAL = stockOrderAllocationDAL;
            this.contractSalesForecastLinesDal = contractSalesForecastLinesDal;
            this.adminPermissionsDal = adminPermissionsDal;
            this.orderService = orderService;
            this.accountService = accountService;
            this.mailHelper = mailHelper;
        }

        public ActionResult Order(int? id)
        {
            var user = CurrentUser;
            var model = new CreateOrderModel
            {
                Order = id != null ? orderHeaderDAL.GetById(id.Value) : new Order_header(),
                isEdit = id != null ? true : false,
                DeliveryAddresses = deliveryLocationsDAL.GetForClient(user.company_id).Select(dl => new { dl.unique_id, dl.del1 }).ToDictionary(d => d.unique_id, d => d.del1),
                User = user
            };

            model.Order.orderdate = DateTime.Now;

            ViewBag.Title = string.Format("{0}Order {1}", (model.isEdit ? "" : "Create "), model.Order.custpo);

            return View(model);
        }

        [HttpPost]
        public ActionResult Order(CreateOrderModel model)
        {
            if (model != null && model.Order.orderid > 0)
            {
                if (model.isEdit)
                {
                    orderHeaderDAL.Update(model.Order);
                }
                else
                {
                    orderHeaderDAL.Create(model.Order);
                }
                return View("OrderStep2");
            }
            return RedirectToAction("Order");
        }

        public ActionResult OrderStep2(int? id)
        {
            return View("OrderStep2");
        }

        public ActionResult Calculation(int id)
        {
            List<int> ids = new int[] { id }.ToList();
            CargoInfo cargoInfo = PrepareCargo(ids);
            ViewBag.order_ids = string.Join(",", ids.ToArray());
            return View(cargoInfo);
        }

        public ActionResult CalculationMulti(string ids)
        {
            CargoInfo cargoInfo = PrepareCargo(company.Common.Utilities.GetIdsFromString(ids));
            ViewBag.order_ids = ids;
            return View("Calculation", cargoInfo);
        }

        private CargoInfo PrepareCargo(List<int> order_ids)
        {
            List<Order_lines> orderLines = orderLinesDAL.GetByOrderIds(order_ids);
            CargoInfo result = new CargoInfo();
            Order_header order = null;
            Container_types containerType = null;
            if (orderLines != null && orderLines.Count > 0)
            {
                order = orderLines[0].Header;
                if (order != null && order.container_type != null)
                {
                    containerType = containerTypesDal.GetById(order.container_type.Value);
                }
                if (containerType != null && containerType.height != null && containerType.width != null && containerType.length != null)
                {
                    var cargoList = new List<CargoUnit>();
                    var cargoGroups = new List<Group>();
                    var factory_ids =
                        orderLines.Where(l => l.Header.loading_factory != null)
                                  .Select(l => l.Header.loading_factory.Value)
                                  .Union(
                                      orderLines.Where(l => l.Cust_Product.MastProduct.factory_id != null)
                                                .Select(l => l.Cust_Product.MastProduct.factory_id.Value)).Distinct().ToList();
                    //TODO: find factory sequences and organize them into groups
                    var factories = companyDal.GetFactories(factory_ids);
                    int seq = 1;
                    var groups = factories.GroupBy(f => f.loading_seq).OrderBy(g => g.Key).ToList();
                    foreach (var g in groups)
                    {
                        var factList = g.ToList();
                        for (int i = 0; i < factList.Count; i++)
                        {
                            factList[i].loading_seq = seq;
                        }

                        cargoGroups.Add(new Group { GroupName = string.Format("Group{0}", seq), Sequence = seq });
                        seq++;
                    }

                    foreach (var orderLine in orderLines)
                    {
                        //if (orderLine.Cust_Product.cprod_mast != null)
                        {
                            var mastProd = orderLine.Cust_Product.MastProduct; //Mast_productsDAL.GetById(orderLine.Cust_Product.cprod_mast.Value);

                            int qty = Convert.ToInt32(orderLine.orderqty);

                            if (mastProd.units_per_pallet_single != null && mastProd.units_per_pallet_single > 0)
                            {
                                int numofPalletes = qty / mastProd.units_per_pallet_single.Value;
                                int numofExtraCartons = qty % mastProd.units_per_pallet_single.Value;
                                if (numofPalletes > 0)
                                {
                                    if (mastProd.pallet_width != null && mastProd.pallet_length != null &&
                                        mastProd.pallet_height != null)
                                    {
                                        var cargo = new CargoUnit();
                                        cargo.Name = mastProd.asaq_name.Substring(0, Math.Min(mastProd.asaq_name.Length, 25)) +
                                                     "_palette";
                                        cargo.Width = mastProd.pallet_width.Value;
                                        if (mastProd.pallet_height > 0)
                                            cargo.Height = mastProd.pallet_height.Value;
                                        else
                                            cargo.Height = 2000; //mastProd.pack_height.Value;

                                        cargo.Length = mastProd.pallet_length.Value;
                                        cargo.Quantity = numofPalletes;
                                        cargo.Weight = (mastProd.pack_GW != null ? mastProd.pack_GW.Value : 0) * mastProd.units_per_pallet_single.Value;
                                        cargo.MaxWeightOnTop = mastProd.maxweight_pallet;
                                        SetGroup(cargo, factories, mastProd, cargoGroups);
                                        cargoList.Add(cargo);
                                    }
                                }
                                if (numofExtraCartons > 0)
                                {

                                    int numOfCartons = 0;
                                    int numOfProducts = 0;

                                    if (mastProd.units_per_carton != null && mastProd.units_per_carton != 0)
                                    {
                                        numOfCartons = numofExtraCartons / mastProd.units_per_carton.Value;
                                        numOfProducts = numofExtraCartons % mastProd.units_per_carton.Value;
                                    }
                                    else
                                        numOfProducts = numofExtraCartons;

                                    if (numOfCartons > 0)
                                    {
                                        var cargo = new CargoUnit();
                                        cargo.Name = mastProd.asaq_name.Substring(0, Math.Min(mastProd.asaq_name.Length, 25));
                                        cargo.Width = mastProd.carton_width.Value;
                                        cargo.Length = mastProd.carton_length.Value;
                                        cargo.Height = mastProd.carton_height.Value;
                                        cargo.MaxWeightOnTop = mastProd.maxweight_carton;
                                        cargo.Quantity = numOfCartons;
                                        cargo.Weight = (mastProd.pack_GW != null ? mastProd.pack_GW.Value : 0) * mastProd.units_per_carton.Value;
                                        SetGroup(cargo, factories, mastProd, cargoGroups);
                                        cargoList.Add(cargo);

                                    }
                                    if (numOfProducts > 0)
                                    {
                                        var cargo = new CargoUnit();
                                        cargo.Name = mastProd.asaq_name.Substring(0, Math.Min(mastProd.asaq_name.Length, 25));
                                        cargo.Width = mastProd.pack_width.Value;
                                        cargo.Length = mastProd.pack_length.Value;
                                        cargo.Height = mastProd.pack_height.Value;
                                        cargo.Quantity = numOfProducts;
                                        cargo.Weight = (mastProd.pack_GW != null ? mastProd.pack_GW.Value : 0);
                                        cargo.MaxWeightOnTop = mastProd.maxweight_unit;
                                        SetGroup(cargo, factories, mastProd, cargoGroups);
                                        cargoList.Add(cargo);
                                    }
                                }
                            }
                            else
                            {
                                int numOfCartons = 0;
                                int numOfProducts = 0;

                                if (mastProd.units_per_carton != null && mastProd.units_per_carton != 0)
                                {
                                    numOfCartons = qty / mastProd.units_per_carton.Value;
                                    numOfProducts = qty % mastProd.units_per_carton.Value;
                                }
                                else
                                    numOfProducts = qty;

                                if (numOfCartons > 0)
                                {
                                    var cargo = new CargoUnit();
                                    cargo.Name = mastProd.asaq_name.Substring(0, Math.Min(mastProd.asaq_name.Length, 25));
                                    cargo.Width = mastProd.carton_width.Value;
                                    cargo.Length = mastProd.carton_length.Value;
                                    cargo.Height = mastProd.carton_height.Value;
                                    cargo.MaxWeightOnTop = mastProd.maxweight_carton;
                                    cargo.Weight = (mastProd.pack_GW != null ? mastProd.pack_GW.Value : 0) * mastProd.units_per_carton.Value;
                                    cargo.Quantity = numOfCartons;
                                    SetGroup(cargo, factories, mastProd, cargoGroups);
                                    cargoList.Add(cargo);

                                }
                                if (numOfProducts > 0)
                                {
                                    var cargo = new CargoUnit();
                                    cargo.Name = mastProd.asaq_name.Substring(0, Math.Min(mastProd.asaq_name.Length, 25));
                                    cargo.Width = mastProd.pack_width.Value;
                                    cargo.Length = mastProd.pack_length.Value;
                                    cargo.Height = mastProd.pack_height.Value;
                                    cargo.MaxWeightOnTop = mastProd.maxweight_unit;
                                    cargo.Weight = (mastProd.pack_GW != null ? mastProd.pack_GW.Value : 0);
                                    cargo.Quantity = numOfProducts;
                                    SetGroup(cargo, factories, mastProd, cargoGroups);
                                    cargoList.Add(cargo);
                                }
                            }

                        }
                    }
                    result.CargoList = cargoList.ToArray();
                    result.Groups = cargoGroups.ToArray();
                    result.ContainerTypes = new[] { new ContainerTypeInfo { Name = containerType.container_type_desc, Length = containerType.length.Value, Width = containerType.width.Value, Height = containerType.height.Value } };
                }
                else
                    throw new ArgumentException("Container and/or its dimensions not specified");
            }

            return result;
        }

        private void SetGroup(CargoUnit cargo, List<Company> factories, Mast_products mastProd, List<Group> cargoGroups)
        {
            var factory = factories.FirstOrDefault(f => f.user_id == mastProd.factory_id);
            if (factory != null)
            {
                var cg = cargoGroups.FirstOrDefault(g => g.Sequence == factory.loading_seq);
                if (cg != null)
                    cargo.GroupName = cg.GroupName;
            }
        }

        public ActionResult StartCalculation(CargoInfo cargoInfo)
        {
            var client = new ContainerLoadingServiceClient();
            var results = client.GetResults(cargoInfo, true);
            if (results.sessionId != null)
            {
                bool added = MemoryCache.Default.Add(results.sessionId, results, DateTimeOffset.UtcNow.AddHours(1));
            }
            else
            {
                results.sessionId = string.Empty;
            }
            return Json(results.sessionId);
        }

        public ActionResult DisplayResults(string session_id, string sorder_ids)
        {
            if (!string.IsNullOrEmpty(session_id))
            {
                var result = (CalculationResult)MemoryCache.Default.Get(session_id);
                if (result != null)
                {
                    var model = new ContainerLoadResultsModel { CalculationResult = result };
                    List<int> order_ids = company.Common.Utilities.GetIdsFromString(sorder_ids);
                    model.Orders = new List<Order_header>();
                    model.Customers = new List<Company>();
                    foreach (var orderId in order_ids)
                    {
                        model.Orders.Add(orderHeaderDAL.GetById(orderId));
                        if (model.Orders.Last().userid1 != null)
                            model.Customers.Add(companyDal.GetById(model.Orders.Last().userid1.Value));
                    }

                    model.ETD = orderHeaderDAL.GetMaxEtd(order_ids);

                    return View("Results", model);
                }
                else
                {
                    ViewBag.error = "Calculation results have expired. Go back to order page and click button Calculate";
                    return View("DisplayCalculationError");
                }
            }
            else
            {
                ViewBag.error = "Calculation not done properly. Check that dimensions for products/cartons/pallets are correct and greater than zero.";
                return View("DisplayCalculationError");
            }
        }

        public ActionResult GetContainerLoadingImage(string session_id, int cont_index, int? segment_index)
        {
            var result = (CalculationResult)MemoryCache.Default.Get(session_id);
            if (result != null)
            {
                byte[] imageData;
                if (segment_index == null)
                    imageData = result.Containers[cont_index].Picture;
                else
                {
                    imageData = result.Containers[cont_index].Segments[segment_index.Value].Picture;
                }
                return File((byte[])imageData, "image/bmp");
            }
            return new EmptyResult();
        }

        //Stock order allocations
        public ActionResult SoAllocations()
        {
            var model = new StockOrderAllocationsModel
            {
                Factories = orderLineExportDal.GetFactories(),
                Lines = null,
                DateType = 0,
                From = DateTime.Today.AddMonths(-1),
                Clients = orderLineExportDal.GetClients().Select(c => new CheckBoxItem { Code = c.user_id, IsChecked = true, Label = c.customer_code }).ToList(),
                IncludeDiscontinued = true,
                IncludePalletQty = true,
                Excel = true
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult SoAllocations(StockOrderAllocationsModel m)
        {
            if (ModelState.IsValid /*&& !string.IsNullOrEmpty(m.factory_ids)*/)
            {
                var factory_ids = !string.IsNullOrEmpty(m.factory_ids) ? m.factory_ids.Split(',').Select(int.Parse).ToList() :
                    orderLineExportDal.GetFactories().Select(f => f.user_id).ToList();
                var clients_ids = m.Clients.Where(c => c.IsChecked).Select(c => c.Code).ToList();
                var allowedStockCodes = new int[] { 1, 2, 3, 4 };

                var lines = orderLineExportDal.GetForCriteria(factory_ids, client_ids: clients_ids, includeDiscontinued: m.IncludeDiscontinued).
                    Where(l => l.orderqty > 0 && (m.IgnoreStockCodes || (l.cprod_stock_code != null && allowedStockCodes.Contains(l.cprod_stock_code.Value)))).ToList();
                AddAllocationsToCOLines(lines.Where(l => l.stock_order == 8));
                orderLineExportDal.GetAllocationLines(lines.Where(l => l.stock_order == 1), "st"); //add CO lines allocations to stock order lines
                var dayofweek = Convert.ToInt32(DateTime.Today.DayOfWeek);
                var firstweek = dayofweek == 0 ? DateTime.Today.AddDays(1) : DateTime.Today.AddDays(8 - dayofweek);
                var lastweekend = firstweek.AddDays(7 * 14 + 6);
                PrepareData();
                var clients = Properties.Settings.Default.StockAllocationClients.Split(',').Select(int.Parse).ToList();
                var cprod_ids = lines.Where(l => l.cprod_id != null).Select(l => l.cprod_id.Value).Distinct().ToList();
                var forecasts = salesForecastDal.GetForPeriod(cprod_ids, company.Common.Utilities.GetMonthFromDate(firstweek), company.Common.Utilities.GetMonthFromDate(lastweekend)).GroupBy(f => f.cprod_id).ToDictionary(g => g.Key.Value, g => g.ToList());
                var salesdata = salesDataDal.GetForPeriod(cprod_ids, company.Common.Utilities.GetMonthFromNow(-12), company.Common.Utilities.GetMonthFromNow(-1)).GroupBy(f => f.cprod_id).ToDictionary(g => g.Key.Value, g => g.ToList());
                var cslines = contractSalesForecastLinesDal.GetForPeriod(cprod_ids, DateTime.Today, lastweekend).GroupBy(f => f.cprod_id).ToDictionary(g => g.Key.Value, g => g.ToList());
                var allocations = stockOrderAllocationDAL.GetByProducts(cprod_ids).GroupBy(f => f.cprod_id).ToDictionary(g => g.Key, g => g.ToList());
                var arrivingOrders = new List<OrderMgtmDetail>(); //erp.Model.DAL.OrderMgmtDetailDAL.GetByProducts(cprod_ids, DateTime.Today, lastweekend).GroupBy(f => f.cprod_id).ToDictionary(g => g.Key.Value, g => g.ToList());
                var model = new StockOrderAllocationsModel
                {
                    Lines = lines,
                    FirstWeek = firstweek,
                    CallOffOrders = GetCalloffOrders(lines, m.DateType == 1 ? m.From : null, m.DateType == 0 ? m.From : null),
                    StockOrders = GetStockOrders(lines, m.DateType == 1 ? m.From : null, m.DateType == 0 ? m.From : null).OrderBy(s => s.orderdate).ToList(),
                    Products = (from l in lines /*where m.DateType == 1 ? l.req_eta >= m.From : l.po_req_etd >= m.From*/
                                where clients.Contains(l.cprod_user ?? 0)
                                group l by l.cprod_id into g
                                orderby g.First().factory_ref, g.First().cprod_code1
                                select new Product
                                {
                                    Prod = new Cust_products
                                    {
                                        MastProduct = new Mast_products
                                        {
                                            factory_ref = g.First().factory_ref,
                                            units_per_pallet_single = g.First().units_per_pallet_single,
                                            Factory = new Company
                                            {
                                                factory_code = g.First().factory_code
                                            },
                                            min_ord_qty = g.First().min_ord_qty
                                        },
                                        cprod_id = g.First().cprod_id.Value,
                                        cprod_code1 = g.First().cprod_code1,
                                        cprod_name = g.First().cprod_name,
                                        cprod_stock = g.First().cprod_stock,
                                        cprod_stock_code = g.First().cprod_stock_code,
                                        cprod_status = g.First().cprod_status

                                    },
                                    Allocations = allocations.ContainsKey(g.Key.Value) ? allocations[g.Key.Value] : new List<Stock_order_allocation>(),
                                    Forecasts = forecasts.ContainsKey(g.Key.Value) ? forecasts[g.Key.Value] : new List<Sales_forecast>(),
                                    SalesData = salesdata.ContainsKey(g.Key.Value) ? salesdata[g.Key.Value] : new List<Sales_data>(),
                                    CS_Lines = cslines.ContainsKey(g.Key.Value) ? cslines[g.Key.Value] : new List<Contract_sales_forecast_lines>(),
                                    ArrivingOrders = new List<OrderMgtmDetail>() //arrivingOrders.ContainsKey(g.Key.Value) ? arrivingOrders[g.Key.Value] : new List<OrderMgtmDetail>(),
                                }).ToList(),
                    IncludePalletQty = m.IncludePalletQty,
                    From = m.From
                };
                foreach (var prod in model.Products)
                {
                    prod.StockAvg3Months = prod.SalesData.Where(s => s.month21 >= company.Common.Utilities.GetMonthFromNow(-3)).Average(s => s.sales_qty);

                    var stock = prod.Prod.cprod_stock ?? 0.0;

                    var balance = stock;
                    double weekForecastSum = 0.0;
                    double weekCFSum = 0.0;  //contract forecast
                    //Calculate balance from today to firstweek 
                    var baseDate = DateTime.Today;
                    for (var d = 0; d < (model.FirstWeek - DateTime.Today).TotalDays; d++)
                    {
                        var day = baseDate.AddDays(d);
                        var monthCode = company.Common.Utilities.GetMonthFromDate(day);
                        var daysInMonth = System.DateTime.DaysInMonth(day.Year, day.Month);
                        var forecast = prod.Forecasts.FirstOrDefault(f => f.month21 == monthCode);
                        if (forecast != null && forecast.sales_qty != null)
                        {
                            weekForecastSum += forecast.sales_qty.Value * 1.0 / daysInMonth;
                        }
                        var cfSum = prod.CS_Lines.Where(l => l.Forecast.startmonth != null && l.Forecast.monthduration != null && day >= l.Forecast.startmonth && day <= l.Forecast.startmonth.Value.AddMonths(l.monthduration != null ? l.monthduration.Value : l.Forecast.monthduration.Value)).Sum(l => l.qty);
                        weekCFSum += cfSum.Value * 1.0 / daysInMonth;
                    }
                    var arrivalsSum = prod.ArrivingOrders.Where(o => o.req_eta_norm != null && o.req_eta_norm.Value.Date > baseDate && o.req_eta_norm.Value.Date < model.FirstWeek).Sum(o => o.orderqty) ?? 0.0;
                    balance += arrivalsSum - weekCFSum - weekForecastSum;

                    baseDate = model.FirstWeek;
                    prod.StockValues = new List<SOProductStock>();
                    for (int i = 0; i < 14; i++)
                    {
                        prod.StockValues.Add(new SOProductStock { Balance = balance, Date = model.FirstWeek.AddDays(i * 7), Product = prod });
                        weekForecastSum = 0.0;
                        weekCFSum = 0.0;  //contract forecast
                        for (var d = 0; d < 7; d++)
                        {
                            var day = baseDate.AddDays(i * 7 + d);
                            var monthCode = company.Common.Utilities.GetMonthFromDate(day);
                            var daysInMonth = System.DateTime.DaysInMonth(day.Year, day.Month);
                            var forecast = prod.Forecasts.FirstOrDefault(f => f.month21 == monthCode);
                            if (forecast != null && forecast.sales_qty != null)
                            {
                                weekForecastSum += forecast.sales_qty.Value * 1.0 / daysInMonth;
                            }
                            var cfSum = prod.CS_Lines.Where(l => l.Forecast.startmonth != null && l.Forecast.monthduration != null && day >= l.Forecast.startmonth && day <= l.Forecast.startmonth.Value.AddMonths(l.monthduration != null ? l.monthduration.Value : l.Forecast.monthduration.Value)).Sum(l => l.qty);
                            weekCFSum += cfSum.Value * 1.0 / daysInMonth;
                        }
                        arrivalsSum = prod.ArrivingOrders.Where(o => o.req_eta_norm != null && o.req_eta_norm.Value.Date >= baseDate.AddDays(i * 7) && o.req_eta_norm.Value.Date <= baseDate.AddDays(i * 7 + 6)).Sum(o => o.orderqty) ?? 0.0;
                        balance += arrivalsSum - weekCFSum - weekForecastSum;
                        //14 weeks in the future

                    }

                }
                //#if !DEBUG
                var factory_code = companyDal.GetById(factory_ids[0]).factory_code;
                if (m.Excel)
                {
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename=StockAllocation_{0}.xls", factory_ids.Count > 1 ? factory_code.Substring(0, 2) : factory_code));
                    Response.ContentType = "application/vnd.ms-excel";
                }
                //#endif

                return View("ExportTable", model);
            }
            return RedirectToAction("SoAllocations");
        }

        public ActionResult SOAllocationsManagement()
        {
            var model = new SOAllocationsManagementModel();
            return View(model);
        }

        public ActionResult GetCOOrders(int cprod_id)
        {
            return
                Json(
                    GetCOOrdersByCriteria(cprod_id, DateTime.Today.AddYears(-1),
                        DateTime.Today.AddYears(1)).Select(o => new { o.orderid, o.custpo, o.req_eta }));
        }

        public ActionResult GetDataForAllocations(int cprod_id, int[] ids)
        {
            var coLines = GetCOLinesByIds(cprod_id, ids);
            var soLineIds = new HashSet<int>();
            foreach (var cl in coLines)
            {
                soLineIds.AddRange(cl.SOAllocations.Select(a => a.st_line ?? 0));
            }
            return Json(
                new
                {
                    COLines = coLines
                        .Select(l => new
                        {
                            l.orderid,
                            l.Header.custpo,
                            l.orderqty,
                            l.linenum,
                            SOAllocations = l.SOAllocations.Select(so => new
                            {
                                id = so.unique_link_ref,
                                balance = so.StockLine.Header.Balance,
                                so.StockLine.Header.custpo,
                                so.StockLine.orderqty,
                                so.st_line,
                                so.StockLine.orderid,
                                so.alloc_qty,
                                so.StockLine.Header.po_req_etd
                            })
                        }),
                    AvailableSOLines = GetAvailableStockOrderLines(cprod_id, soLineIds.ToList(), DateTime.Today.AddYears(-1),
                        DateTime.Today.AddMonths(2))
                        .Select(l => new
                        {
                            l.orderid,
                            l.Header.custpo,
                            l.orderqty,
                            l.linenum,
                            balance = l.orderqty - l.Allocations.Sum(a => a.alloc_qty),
                            l.Header.po_req_etd
                        })
                });
        }

        public ActionResult UpdateAllocations(Stock_order_allocation[] allocations)
        {
            UpdateAllocations(allocations);
            return Json(allocations);
        }



        private void AddAllocationsToCOLines(IEnumerable<Order_line_export> lines)
        {
            orderLineExportDal.GetAllocationLines(lines);
        }

        public ActionResult OMExport()
        {
            var currUser = accountService.GetCurrentUser();
            var company = companyDal.GetById(currUser?.company_id ?? 0);
            ViewBag.breadcrumbs = new List<BreadCrumb> { new BreadCrumb { Text = "Order management export" } };
            var isCurrentUserFactoryUser = company != null && company.user_type == (int)Company_User_Type.Factory;
            var model = new OrderExportModel
            {
                Clients = orderHeaderDAL.GetClientsOnOrders(isCurrentUserFactoryUser ? new[] { company.user_id } : null, combined: true).OrderBy(c => c.customer_code).ToList(),
                Factories = GetFactories(isCurrentUserFactoryUser, company),
                factoryids = isCurrentUserFactoryUser ? company.user_id.ToString() : "",
                Locations = locationDal.GetAll().Where(l => l.show_on_omexport == true).ToList(),
                Brands = brandsDal.GetAll(),//.Select(b=>new CheckBoxItem {Code = b.brand_id,IsChecked = true, Label = b.brandname }).ToList(),
                CWBrands = custproductsDal.GetXYBrandsFromProducts(),
                location_id = 0,
                ETD_From = DateTime.Today.AddMonths(-3),
                ETD_To = DateTime.Today.AddYears(1),
                MonthlySummary = false,
                IncludeSpares = true,
                SparesOnly = false,
                MonthlySummaryBy = MonthSummaryBy.ETD,
                OrderBySeqNumber = false,
                RegularOnly = false,
                IncludeSalesForecast = true,
                IncludeSalesHistory = true,
                HighlightLowForecasts = true,
                ShowBrandRangeColumn = true,
                ShowFactoryCode = false,
                ShowSales = false,
                factory_id = isCurrentUserFactoryUser && (company.combined_factory ?? 0) == 0 ? (int?)company.user_id : null,
                IsCurrentUserFactoryUser = isCurrentUserFactoryUser,
                EnableContainerPriceSelection = Settings.Default.OMExport_UsersForFinancialInfo.Contains(accountService.GetCurrentUser().userid),
                EnableExtraValueFieldsSelection = Settings.Default.OMExport_UseridsForExtraValueFields == null
                                                ||
                                                Settings.Default.OMExport_UseridsForExtraValueFields.Contains(
                                                    accountService.GetCurrentUser().userid),
                OrderByList = new[] { ExportOrderBy.ETD, ExportOrderBy.ETA, ExportOrderBy.Client, ExportOrderBy.ClientETD, ExportOrderBy.OrderDate }.ToList(),
                OrderTypeList = new[] { new LookupItem(OrderExportModel.OrderTypeRegular,"Regular only"),new LookupItem(OrderExportModel.OrderTypeStock,"Stock orders only"),
                                            new LookupItem(OrderExportModel.OrderTypeCallOffAndRegular,"Regular and Calloff"),new LookupItem(OrderExportModel.OrderTypeSpares,"Spares")  }.ToList()

            };
            return View("ExportCriteria", model);
        }



        private List<Company> GetFactories(bool isCurrentUserFactoryUser, Company company)
        {
            return isCurrentUserFactoryUser ?
                company.combined_factory > 0 ? companyDal.GetFactoriesByCombinedCode(company.combined_factory.Value) : new List<Company>() { companyDal.GetById(company.user_id) } :
                unitOfWork.CompanyRepository.GetFactories(true).OrderBy(f => f.factory_code).ToList();
        }

        [HttpPost]
        public ActionResult OMExport(OrderExportModel m)
        {
            var productIds = new List<int>();
            foreach (var key in Request.Form.Keys)
            {
                string k = key.ToString();
                if (!k.StartsWith("chk_")) continue;
                var id = int.Parse(k.Replace("chk_", ""));
                if (!productIds.Contains(id))
                    productIds.Add(id);
            }
            var clients9Ref = Settings.Default.OMExport_9RefClients.Split(',').Select(int.Parse);
            m.Factories = companyDal.GetFactories(true);
            m.Ranges = rangeDAL.GetAll();
            m.Products = CollectProducts(productIds, m);
            m.ContainerTypes = unitOfWork.ContainerTypeRepository.Get().ToList();
            m.Currencies = currenciesDal.GetAll();
            m.ProductRanges = unitOfWork.CustProductRangeRepository.Get().ToList();
            m.Locations = locationDal.GetAll().Where(l => l.show_on_omexport == true).ToList();

            var custpoList = (!string.IsNullOrEmpty(m.POCriteria) ? m.POCriteria.Split(',').ToList() : null);
            var from = m.ETD_From > DateTime.Today.AddYears(-1) ? DateTime.Today.AddYears(-1) : m.ETD_From;
            var to = m.ETD_To < DateTime.Today ? DateTime.Today : m.ETD_To;
            var sFactoryIds = m.factoryids;
            List<int> factoryIds = null;
            if (!string.IsNullOrEmpty(sFactoryIds))
                factoryIds = company.Common.Utilities.GetIdsFromString(sFactoryIds);

            var lines = (m.client_id != null
                             ? orderLinesDAL.GetByExportCriteria(productIds, null, m.client_id, from, to, m.factory_id, custpoList, GetFactoryIds(factoryIds))
                             : orderLinesDAL.GetByExportCriteria(null, productIds, m.client_id, from, to, m.factory_id, custpoList, GetFactoryIds(factoryIds)));
            m.AllLines = lines;
            //lines = lines.Where(l => l.orderid == 19969).ToList();

            m.Lines =
                lines.Where(l => (l.Header.po_req_etd >= m.ETD_From || m.ETD_From == null)
                && (l.Header.po_req_etd <= m.ETD_To || m.ETD_To == null)
                && ((m.location_id ?? 0) == 0 || (l.Header.location_override != null && l.Header.location_override == m.location_id) ||
                        (l.Header.location_override == null && l.Cust_Product.MastProduct.Factory.consolidated_port == m.location_id))).ToList();

            m.CombinedOrders = new List<CombinedOrder>();
            foreach (var o in m.Lines.GroupBy(l => l.orderid))
            {
                var combined_id = o.First().Header.combined_order;
                if (combined_id > 0)
                {
                    var order_combined = m.CombinedOrders.FirstOrDefault(c => c.COrder != null && c.COrder.orderid == combined_id);
                    var co = new CombinedOrder
                    {
                        SourceOrderId = o.Key.Value,
                        COrder =
                                order_combined != null
                                    ? order_combined.COrder
                                    : orderHeaderDAL.GetById(combined_id.Value)
                    };
                    m.CombinedOrders.Add(co);
                }
                var shipments = shipmentsDal.GetForOrder(o.First().orderid ?? 0);
                foreach (var h in o)
                {
                    h.Header.Shipments = shipments;
                }
                if (o.Any(l => l.original_orderid > 0))
                {
                    var originalOrders = new Dictionary<int, Order_header>();
                    foreach (var line in o.Where(l => l.original_orderid > 0))
                    {
                        var original_orderid = line.original_orderid.Value;
                        if (!originalOrders.ContainsKey(original_orderid))
                        {
                            var existingLine = m.Lines.FirstOrDefault(l => l.orderid == original_orderid);
                            if (existingLine != null)
                                originalOrders[original_orderid] = existingLine.Header;
                            else
                                originalOrders[original_orderid] = orderHeaderDAL.GetById(original_orderid);
                        }
                        line.OriginalOrder = originalOrders[original_orderid];
                    }
                }
            }
            //m.Lines = m.RegularOnly != false ? m.Lines.Where(r => r.Header.stock_order != 1):m.Lines.Where(r=>r.Header.stock_order > 0);

            var firstPreviousMonth = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, 1);
            m.ArrivingLines = (m.client_id != null
                             ? orderLinesDAL.GetByProdIdsAndETA(productIds, null, m.client_id, firstPreviousMonth, DateTime.Today.AddMonths(13), m.factory_id)
                             : orderLinesDAL.GetByProdIdsAndETA(null, productIds, m.client_id, firstPreviousMonth, DateTime.Today.AddMonths(13), m.factory_id));

            // If is ticked Regular orders only [ ]  

            if (m.OrderType != null)
            {
                Func<Order_lines, bool> predicate;
                switch (m.OrderType)
                {
                    case OrderExportModel.OrderTypeStock:
                        predicate = (r => r.Header.stock_order == Order_header.StockOrderStock);
                        break;
                    case OrderExportModel.OrderTypeCallOffAndRegular:
                        predicate = (r => r.Header.stock_order != Order_header.StockOrderStock);
                        break;
                    case OrderExportModel.OrderTypeSpares:
                        predicate = (r => r.Header.stock_order == Order_header.StockOrderSpares);
                        break;
                    default:
                        predicate = (r => r.Header.stock_order != Order_header.StockOrderStock && r.Header.stock_order != Order_header.StockOrderCalloff);
                        break;
                }

                m.Lines = m.Lines.Where(predicate).ToList();
                m.ArrivingLines =
                    m.ArrivingLines.Where(predicate).ToList();
            }
            var oldProducts = new List<OrderProduct>();
            foreach (var prod in m.Products)
            {
                //Get all lines from to see if there were orders in last year
                var prodLines = lines.Where(l =>
                        (m.client_id != null && prod.cprod_ids.Contains(l.cprod_id.Value)) ||
                        (m.client_id == null && prod.mast_ids.Contains(l.Cust_Product.cprod_mast))).ToList();
                //filtered lines for display
                prod.Lines =
                    m.Lines.Where(
                        l =>
                        (m.client_id != null && prod.cprod_ids.Contains(l.cprod_id.Value)) ||
                        (m.client_id == null && prod.mast_ids.Contains(l.Cust_Product.cprod_mast))).ToList();
                if (prodLines == null || prodLines.Count == 0 || prodLines.Max(l => l.Header.po_req_etd) < Utilities.Min(DateTime.Today.AddYears(-1), from ?? DateTime.Today))
                    oldProducts.Add(prod);
            }
            foreach (var orderProduct in oldProducts)
            {
                m.Products.Remove(orderProduct);
            }
            m.Show9Ref = m.client_id != null && clients9Ref.Contains(m.client_id.Value);

            if (m.client_id != null)
                m.Client = companyDal.GetById(m.client_id.Value);
            m.Ports = m.ShowSuggestedOrder ? m.Ports = unitOfWork.PortsRepository.Get().ToList() : new List<ports>();

            m.OrderService = orderService;
            if (Settings.Default.OMExport_ForwardLinesClients?.Contains(m.client_id) == true)
            {
                var cprod_codes = m.Products.Select(p => p.Prod.cprod_code1).ToList();
                m.ForwardOrderLines = unitOfWork.ForwardOrderLinesRepository.Get(l => cprod_codes.Contains(l.product)).ToList();
            }


            //#if !DEBUG
            if (m.Excel)
            {
                Response.AddHeader("Content-Disposition", "attachment;filename=OMExport.xls");
                Response.ContentType = "application/vnd.ms-excel";
            }

            //#endif

            return View(m);
        }

        private List<OrderProduct> CollectProducts(List<int> productIds, OrderExportModel m)
        {
            var result = new List<OrderProduct>();
            var productList = (m.client_id != null
                                   ? GetCustProductsForIds(productIds)
                                   : custproductsDal.GetForMastIds(productIds));
            //Order_headerDAL.GetProductsOnOrders(m.factory_id, m.client_id, string.Empty,m.IncludeSpares)
            //               .Where(p => productIds.Contains(m.client_id != null ? p.cprod_id : p.MastProduct.mast_id));
            var dist_products = distProductsDal.GetByIds(productList.Select(p => p.cprod_id).ToList());
            List<Sales_forecast> forecasts = null;
            List<Sales_data> salesHistory = null;

            var cprod_ids = productList.Select(p => p.cprod_id).ToList();
            var mast_ids = productList.Select(p => p.MastProduct?.mast_id ?? 0).ToList();
            var bundle_ids = m.client_id != null ? unitOfWork.CustProductBundleRepository.Get(b => b.Components.Any(c => cprod_ids.Contains(c.cprod_id))).Select(b => b.id).ToList() :
                                unitOfWork.CustProductBundleRepository.Get(b => b.Components.Any(c => mast_ids.Contains(c.Component.cprod_mast.Value))).Select(b => b.id).ToList();
            if (m.IncludeSalesForecast)
                forecasts = m.client_id != null ?
                    salesForecastDal.GetForPeriod(cprod_ids,
                    company.Common.Utilities.GetMonthFromDate(DateTime.Today),
                    company.Common.Utilities.GetMonthFromNow(11)) :
                    salesForecastDal.GetForMastProdAndPeriod(mast_ids,
                        company.Common.Utilities.GetMonthFromDate(
                                DateTime.Today),
                        company.Common.Utilities.GetMonthFromNow
                            (11));
            if (m.IncludeSalesHistory)
                salesHistory = m.client_id != null
                    ? salesDataDal.GetForPeriod(cprod_ids,
                        company.Common.Utilities.GetMonthFromNow(-12),
                        company.Common.Utilities.GetMonthFromNow(-1))
                    : salesDataDal.GetForMastProdAndPeriod(mast_ids,
                        company.Common.Utilities.GetMonthFromNow(-12),
                        company.Common.Utilities.GetMonthFromNow(-1));

            if (m.ShowExtraValueFields)
            {
                var date = DateTime.Today;
                var companies_BookedInDate = company.Common.Utilities.GetIdsFromString(Settings.Default.OMExport_CompaniesUsingBookedInDate);
                var useBookedInDate = m.client_id != null && companies_BookedInDate.Contains(m.client_id.Value);
                m.UseSalesOrders = m.client_id != null && company.Common.Utilities.GetIdsFromString(Settings.Default.OMExport_ClientsUsingSalesOrders).Contains(m.client_id.Value);
                if (m.client_id != null)
                {

                    if (m.UseSalesOrders)
                    {
                        var tmpProductSoldQtys = unitOfWork.SalesOrdersRepository.GetQuery(s => (s.delivery_reason == null || !s.delivery_reason.Contains("DISPLAY"))
                                              && s.cprod_id != null && cprod_ids.Contains(s.cprod_id.Value), includeProperties: "Dealer").ToList();

                        m.ProductSoldQtys = tmpProductSoldQtys.Where(s => (s.Dealer == null || (s.Dealer != null && s.Dealer.isInternal != true))).
                                        GroupBy(l => l.cprod_id).Select(g => new { g.Key, Total = g.Sum(l => l.order_qty) }).
                                        ToDictionary(r => r.Key, r => r.Total);
                    }
                    else
                    {
                        m.ProductSoldQtys = unitOfWork.SalesDataRepository.GetQuery(s => cprod_ids.Contains(s.cprod_id.Value)).
                                        GroupBy(l => l.cprod_id).Select(g => new { g.Key, Total = g.Sum(l => l.sales_qty) }).
                                        ToDictionary(r => r.Key, r => r.Total);

                    }

                    /*
                    m.ProductSoldQtys = m.UseSalesOrders ?
                                        unitOfWork.SalesOrdersRepository.GetQuery(s =>(s.delivery_reason == null || !s.delivery_reason.Contains("DISPLAY")) 
                                                                                      && s.cprod_id != null && cprod_ids.Contains(s.cprod_id.Value)).
                                        GroupBy(l => l.cprod_id).Select(g => new { g.Key, Total = g.Sum(l => l.order_qty) }).                                        
                                        ToDictionary(r => r.Key, r => r.Total)
                                        :
                                        unitOfWork.SalesDataRepository.GetQuery(s=> cprod_ids.Contains(s.cprod_id.Value)).
                                        GroupBy(l => l.cprod_id).Select(g => new { g.Key, Total = g.Sum(l => l.sales_qty) }).
                                        ToDictionary(r => r.Key, r => r.Total);
                    */

                    m.DisplaysSoldQtys = m.UseSalesOrders
                        ? unitOfWork.SalesOrdersRepository.GetQuery(
                                s =>
                                    s.delivery_reason.Contains("DISPLAY") && s.cprod_id != null &&
                                    cprod_ids.Contains(s.cprod_id.Value), includeProperties: "Dealer").
                            GroupBy(l => l.cprod_id).Select(g => new { g.Key, Total = g.Sum(l => l.order_qty) }).
                            ToDictionary(r => r.Key, r => r.Total)
                        : new Dictionary<int?, int?>();

                    /*
                    if (m.UseSalesOrders)
                    {
                        var tmpDisplaysSoldQtys =
                            unitOfWork.SalesOrdersRepository.GetQuery(
                                    s =>
                                        s.delivery_reason.Contains("DISPLAY") && s.cprod_id != null &&
                                        cprod_ids.Contains(s.cprod_id.Value), includeProperties: "Dealer").ToList();

#if DEBUG
                        var test = tmpDisplaysSoldQtys.Where(s => (s.Dealer == null || (s.Dealer != null && s.Dealer._internal != true))).ToList();
                        var testTrue = tmpDisplaysSoldQtys.Where(s => (s.Dealer == null || (s.Dealer != null && s.Dealer._internal == true))).ToList();
                        var testCustomer = tmpDisplaysSoldQtys.Where(s => (s.Dealer == null || (s.Dealer != null && s.Dealer.customer == "C08302"))).ToList();
#endif



                        m.DisplaysSoldQtys = tmpDisplaysSoldQtys.Where(s => (s.Dealer == null || (s.Dealer != null && s.Dealer._internal != true))).GroupBy(l => l.cprod_id)
                            .Select(g => new { g.Key, Total = g.Sum(l => l.order_qty) }).ToDictionary(r => r.Key, r => r.Total);
                    }
                    else
                    {
                        m.DisplaysSoldQtys = new Dictionary<int?, int?>();
                    }
                    */

                    /*unitOfWork.OrderLinesRepository.GetQuery(l => cprod_ids.Contains(l.cprod_id.Value) && l.Header.status != "X" 
                                    && l.Header.status != "Y" && l.Header != null, includeProperties: "Header").
                            GroupBy(l => l.cprod_id).Select(g => new { g.Key, Total = g.Sum(l => l.orderqty) }).
                            ToDictionary(r => r.Key, r => r.Total);*/
                    m.ProductDeliveredQtys = unitOfWork.OrderLinesRepository.GetQuery(l => cprod_ids.Contains(l.cprod_id.Value) && l.Header.status != "X"
                                        && l.Header.status != "Y"
                                        && ((l.Header.req_eta <= date && (!useBookedInDate || l.Header.booked_in_date != null)) ||
                                            (useBookedInDate && l.Header.booked_in_date != null)
                                            )
                                        && l.Header != null, includeProperties: "Header").
                                GroupBy(l => l.cprod_id).Select(g => new { g.Key, Total = g.Sum(l => l.orderqty) }).
                                ToDictionary(r => r.Key, r => r.Total);
                }
                else
                {

                    m.ProductSoldQtys = /*useSalesOrders ?
                                        unitOfWork.SalesOrdersRepository.GetQuery(s => s.cprod_id != null && mast_ids.Contains(s.Product.cprod_mast.Value), includeProperties: "Product").
                                        GroupBy(l => l.Product.cprod_mast).Select(g => new { g.Key, Total = g.Sum(l => l.order_qty) }).
                                        ToDictionary(r => r.Key, r => r.Total)
                                        :*/
                                        unitOfWork.SalesDataRepository.GetQuery(s => mast_ids.Contains(s.Product.cprod_mast.Value), includeProperties: "Product").
                                        GroupBy(l => l.Product.cprod_mast).Select(g => new { g.Key, Total = g.Sum(l => l.sales_qty) }).
                                        ToDictionary(r => r.Key, r => r.Total);
                    /*m.ProductSoldQtys = unitOfWork.OrderLinesRepository.GetQuery(l => mast_ids.Contains(l.Cust_Product.cprod_mast.Value) && l.Header.status != "X"
                                        && l.Header.status != "Y" && l.Header != null, includeProperties: "Header, Cust_Product").
                                GroupBy(l => l.Cust_Product.cprod_mast).Select(g => new { g.Key, Total = g.Sum(l => l.orderqty) }).
                                ToDictionary(r => r.Key, r => r.Total);*/
                    m.ProductDeliveredQtys = unitOfWork.OrderLinesRepository.GetQuery(l => mast_ids.Contains(l.Cust_Product.cprod_mast.Value) && l.Header.status != "X"
                                        && l.Header.status != "Y"
                                        && ((l.Header.req_eta <= date && (!useBookedInDate || l.Header.booked_in_date != null)) ||
                                            (useBookedInDate && l.Header.booked_in_date != null)
                                            )
                                        && l.Header != null, includeProperties: "Header, Cust_Product").
                                GroupBy(l => l.Cust_Product.cprod_mast).Select(g => new { g.Key, Total = g.Sum(l => l.orderqty) }).
                                ToDictionary(r => r.Key, r => r.Total);
                }
                if (m.UseSalesOrders)
                {
                    var bundleSale = unitOfWork.SalesOrdersRepository.GetQuery(s => s.bundle_id != null && bundle_ids.Contains(s.bundle_id.Value), includeProperties: "Bundle.Components.Component")
                                    .ToList()
                                        .SelectMany(s => s.Bundle.Components, (s, c) => new { id = m.client_id != null ? c.cprod_id : c.Component.cprod_mast, s.order_qty }).
                                            GroupBy(sc => sc.id).
                                            ToDictionary(
                                                    g => g.Key,
                                                    g => g.Sum(sc => sc.order_qty));
                    foreach (var key in bundleSale.Keys)
                    {
                        if (m.ProductSoldQtys.ContainsKey(key))
                            m.ProductSoldQtys[key] += bundleSale[key];
                        else
                            m.ProductSoldQtys[key] = bundleSale[key];
                    }

                }

            }

            foreach (var prod in productList.GroupBy(p => m.client_id != null ? p.cprod_id.ToString() : p.MastProduct.factory_ref))
            {
                var product = prod.First();
                //product.cprod_code1 = string.Join(" ", prod.Select(p => p.cprod_code1).ToList());
                List<string> cprodCodes = new List<string>();
                foreach (var p in prod)
                {
                    if (m.client_id != null)
                    {
                        var distProd =
                            dist_products.FirstOrDefault(
                                dp => dp.dist_cprod_id == p.cprod_id && dp.client_id == m.client_id);
                        if (distProd != null && !string.IsNullOrEmpty(distProd.dist_special_code))
                        {
                            cprodCodes.Add(distProd.dist_special_code);
                            continue;
                        }

                    }
                    cprodCodes.Add(p.cprod_code1);
                }
                product.cprod_code1 = string.Join(" ", cprodCodes);

                var orderProd = new OrderProduct { Prod = product, id = product.cprod_id > 0 ? product.cprod_id : product.MastProduct.mast_id };
                if (m.client_id == null)
                    //For case when there are multiple mast_ids for same factory_ref
                    orderProd.mast_ids = prod.Select(p => (int?)p.MastProduct.mast_id).Distinct().ToList();

                if (product.cprod_id > 0)
                {
                    orderProd.cprod_ids = prod.Select(p => p.cprod_id).ToList();
                    orderProd.BrandRange = company.Common.Extensions.IfNotNull(product.Brand, b => b.brandname) ?? product.client_range;
                    if (m.IncludeSalesHistory)
                    {
                        orderProd.SalesData = new List<Sales_data>();
                        foreach (var custProduct in prod)
                        {
                            orderProd.SalesData.AddRange(salesHistory.Where(h => h.cprod_id == custProduct.cprod_id));
                        }
                    }
                    if (m.IncludeSalesForecast)
                    {
                        orderProd.Forecasts = new List<Sales_forecast>();
                        foreach (var custProduct in prod)
                        {
                            orderProd.Forecasts.AddRange(forecasts.Where(f => f.cprod_id == custProduct.cprod_id));
                        }

                    }

                }
                else
                {
                    if (m.IncludeSalesHistory)
                    {
                        orderProd.SalesData = new List<Sales_data>();
                        foreach (var custProduct in prod)
                        {
                            orderProd.SalesData.AddRange(salesHistory.Where(h => h.cprod_id == custProduct.cprod_id));
                        }
                    }
                    if (m.IncludeSalesForecast)
                    {
                        orderProd.Forecasts = new List<Sales_forecast>();
                        foreach (var custProduct in prod)
                        {
                            orderProd.Forecasts.AddRange(forecasts.Where(f => f.cprod_id == custProduct.cprod_id));
                        }

                    }

                }
                result.Add(orderProd);
            }

            var shipmentDates = GetFirstShipmentDates(m.client_id != null, productIds);
            foreach (var prod in result)
            {
                if (shipmentDates.ContainsKey(prod.id))
                    prod.FirstShipmentEtd = shipmentDates[prod.id];
            }


            return result;

        }

        private List<Cust_products> GetCustProductsForIds(List<int> ids)
        {
            var products = unitOfWork.CustProductRepository.Get(p => ids.Contains(p.cprod_id), includeProperties: "MastProduct.Factory, Color, ExtraData").ToList();
            foreach (var p in products)
                p.cprod_stock_codes = new[] { p.cprod_stock_code ?? 0 }.ToList();
            return products;
        }

        private Dictionary<int?, DateTime?> GetFirstShipmentDates(bool userCprodIds, List<int> productIds)
        {
            var result = new Dictionary<int?, DateTime?>();
            var shipments = userCprodIds ? custproductsDal.ProductFirstShipmentDates(productIds) : mastProductsDal.ProductFirstShipmentDates(productIds);
            return shipments.ToDictionary(kv => (int?)kv.id, kv => kv.Date);
        }

        public ActionResult GetClientsForFactory(string factory_ids = null)
        {
            if (factory_ids == null)
                return Json(orderHeaderDAL.GetClientsOnOrders(combined: true).OrderBy(c => c.customer_code).ToList(), JsonRequestBehavior.AllowGet);
            var factoryIds = company.Common.Utilities.GetIdsFromString(factory_ids);
            return Json(orderHeaderDAL.GetClientsOnOrders(GetFactoryIds(factoryIds), combined: true).OrderBy(u => u.customer_code).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCategories()
        {
            return Json(category1Dal.GetAll().OrderBy(c => c.cat1_name), JsonRequestBehavior.AllowGet);
        }


        private void PrepareData()
        {
            var url = string.Format("{0}/auto_updates/allocated_unallocated.asp", Settings.Default.aspsite_root);
            var req = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                req.GetResponse();
            }
            catch (WebException we)
            {
                EventLog.WriteEntry(Settings.Default.EventLogSource,
                                    string.Format("Call to allocated_unallocated.asp failed. Error: {0}", we.Message));
            }


        }

        private List<CalloffOrder> GetCalloffOrders(List<Order_line_export> lines, DateTime? etaFrom = null, DateTime? etdFrom = null)
        {
            var containers = unitOfWork.ContainerTypeRepository.Get().ToList();
            return (from l in lines
                    where l.stock_order == 8 && (etaFrom == null || l.req_eta >= etaFrom) && (etdFrom == null || l.po_req_etd >= etdFrom)
                    group l by l.orderid into g
                    where g.Sum(l => l.orderqty) > 0
                    select
                        new CalloffOrder
                        {
                            orderdate = g.First().orderdate,
                            po_req_etd = g.First().po_req_etd,
                            req_eta = g.First().req_eta,
                            orderid = g.Key.Value,
                            containerName = GetContainerName(containers, g.First().container_type),
                            custpo = g.First().custpo
                        }).OrderBy(c => c.po_req_etd).ToList();
        }

        private List<Stockorder> GetStockOrders(List<Order_line_export> lines, DateTime? etaFrom = null, DateTime? etdFrom = null)
        {
            return (from l in lines
                    where l.stock_order == 1 /*&& (etaFrom == null || l.req_eta >= etaFrom) && (etdFrom == null || l.po_req_etd >= etdFrom)*/
                    group l by l.orderid into g
                    where g.Sum(l => l.orderqty) > 0
                    select
                        new Stockorder()
                        {
                            orderdate = g.First().orderdate,
                            po_req_etd = g.First().po_req_etd,
                            po_ready_date = g.First().po_ready_date,
                            Qty = g.Sum(le => le.orderqty),
                            custpo = g.First().custpo,
                            orderid = g.Key.Value,
                            Lines = g.ToList(),
                            Balance = g.Sum(le => le.orderqty - le.AllocatedLines.Sum(a => a.AllocQty))
                        }).ToList();
        }


        private string GetContainerName(List<Container_types> containers, int? container_id)
        {
            var container = containers.FirstOrDefault(c => c.container_type_id == container_id);
            if (container != null)
                return container.container_type_desc;
            else
                return string.Empty;
        }


        public ActionResult ProductList(string text, string factory_ids, int? client_id, int? location_id = null, bool spares = true,
            bool discontinued = false, bool regular_only = false, int? brand_user_id = null, bool outofstockonly = false, string analysis_d = null,
            int? bsValue = null, bool spares_only = true)
        {
            string txt = null;

            if (string.IsNullOrEmpty(factory_ids) && client_id == null)
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);

            string excludedDistrubutors = Properties.Settings.Default.OMExport_ExcludedDistributors;
            string excludedCustProductsUsers = Properties.Settings.Default.OMExport_ExcludedCustProductsUsers;

            var factoryIds = company.Common.Utilities.GetIdsFromString(factory_ids);
            var productsOnOrders = orderHeaderDAL.GetProductsOnOrders(location_id, null, client_id, txt, spares, discontinued,
                brand_user_id, GetFactoryIds(factoryIds), outofstockonly, analysis_d, excludedDistrubutors, spares_only: spares_only, excluded_custproducts_cprodusers: excludedCustProductsUsers);

            string[] keywords = HttpUtility.UrlDecode(text).Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (!String.IsNullOrEmpty(text))
            {
                productsOnOrders = productsOnOrders.Where(p => keywords.Any(k => p.MastProduct.factory_ref.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0
                                                                        || p.cprod_code1?.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0
                                                                        || p.cprod_name?.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
            }
            if (bsValue == 1 || bsValue == 2)
            {
                productsOnOrders = productsOnOrders.Where(p => bsValue == 1 ? IsBsProduct(p.cprod_code1) : !IsBsProduct(p.cprod_code1)).ToList();
            }
            return Json(productsOnOrders.Select(p => new {
                factory_ref = ConvertToWrappable(p.MastProduct.factory_ref),
                cprod_code1 = ConvertToWrappable(p.cprod_id > 0 ? p.cprod_code1 : string.Empty),
                cprod_name = ConvertToWrappable(p.cprod_id > 0 ? p.cprod_name : p.MastProduct.asaq_name),
                id = p.cprod_id > 0 ? p.cprod_id : p.MastProduct.mast_id
            }), JsonRequestBehavior.AllowGet);
        }

        private bool IsBsProduct(string code)
        {
            return System.Text.RegularExpressions.Regex.Match(code, "[0-9]{11}").Success;
        }

        //Fixed texts that are too long
        private string ConvertToWrappable(string s)
        {
            return s.Replace("+", "+ ").Replace(",", ", ").Replace("/", "/ ");
        }

        private IList<int> GetFactoryIds(int? factory_id)
        {
            if (factory_id == null)
                return null;
            return GetFactoryIds(new[] { factory_id ?? 0 });
        }

        private IList<int> GetFactoryIds(IList<int> factory_ids)
        {
            //int[] factoryIds = null;
            //if (factory_id != null)
            //{
            //    if (factory_id > 0)
            //        factoryIds = new[] { factory_id.Value };
            //    else
            //    {
            //        factoryIds = CompanyRepository.GetCombinedFactories(factory_id.Value * -1).Select(c => c.user_id).ToArray();
            //    }
            //}
            var currUser = accountService.GetCurrentUser();
            var company = companyDal.GetById(currUser?.company_id ?? 0);
            var isCurrentUserFactoryUser = company != null && company.user_type == (int)Company_User_Type.Factory;

            List<int> factoryIds = new List<int>();
            if (factory_ids != null && factory_ids.Count > 0)
            {
                foreach (var id in factory_ids)
                {
                    if (id > 0)
                        factoryIds.Add(id);
                    else
                        factoryIds.AddRange(unitOfWork.CompanyRepository.Get(c => c.combined_factory == id * -1).Select(c => c.user_id).ToList());
                }
            }
            else
                factoryIds = GetFactories(isCurrentUserFactoryUser, company).Select(c => c.user_id).ToList();
            return factoryIds;
        }


        public ActionResult GetSoOverAllocations()
        {
            return View(stockOrderAllocationDAL.GetOverAllocations());
        }

        public ActionResult GetCallOffLines(int so_line)
        {
            return Json(stockOrderAllocationDAL.GetAllocationCalloffLines(so_line).OrderBy(l => l.linedate).ToList());
        }

        public ActionResult GetAvailableStockLines(int cprod_id)
        {
            return Json(stockOrderAllocationDAL.GetAvailableStockLines(cprod_id));
        }

        public ActionResult GetFactories(int? location_id = null)
        {
            return Json(companyDal.GetFactoriesForLocation(location_id).Select(c => new { c.user_id, c.factory_code }).OrderBy(f => f.factory_code), JsonRequestBehavior.AllowGet);
            //return Json(CompanyRepository.GetFactories(true,location_id).Select(c => new { c.user_id, c.factory_code }).OrderBy(f => f.factory_code),JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult QuantityAnalysisRender(int id, Guid statsKey)
        {
            if (statsKey == new Guid(Properties.Settings.Default.StatsKey))
            {
                var model = new QuantityAnalysisModel { Order = unitOfWork.OrderRepository.Get(o => o.orderid == id, includeProperties: "Lines.Cust_Product, Client").FirstOrDefault() };
                var cprodIds = model.Order.Lines.Select(l => l.cprod_id).ToList();
                var from = DateTime.Today.AddMonths(-12);
                var to = DateTime.Today;
                var company_id = model.Order.userid1;
                model.ProductLineHistory =
                    unitOfWork.OrderLinesRepository.GetQuery(l => cprodIds.Contains(l.cprod_id) &&
                                          (l.Header.orderdate >= from || from == null) &&
                                          (l.Header.orderdate <= to || to == null) &&
                                          (l.Header.userid1 == company_id || company_id == null)).
                                          GroupBy(l => l.cprod_id).
                                          ToDictionary(g => g.Key, g => g.ToList());



                return View("QuantityAnalysis", model);
            }
            ViewBag.message = "No key";
            return View("Message");
        }

        [AllowAnonymous]
        public ActionResult QuantityAnalysis(int id, Guid statsKey)
        {
            if (statsKey == new Guid(Settings.Default.StatsKey))
            {
                var webClient = new WebClient();
                var body =
                    webClient.DownloadString(string.Format("{0}/{1}", WebUtilities.GetSiteUrl(),
                                                           Url.Action("QuantityAnalysisRender",
                                                                      new { id, statsKey = Settings.Default.StatsKey })));

                var order = unitOfWork.OrderRepository.Get(o => o.orderid == id, includeProperties: "Lines.Cust_Product, Client").FirstOrDefault();
                if (order != null)
                {
                    //In the future, send email to all users in permissions table for cusid=order.userid1 , now just to fixed e-mail
                    var permissions = adminPermissionsDal.GetByCompany(order.userid1 ?? 0);
                    var recipients = new List<string>();
                    //recipients = permissions.Select(p => p.User.user_email).ToList();
                    recipients.Add(Settings.Default.OrderQuantityAnalysis_to);
                    var subject = string.Format("Quantity analysis for order id: {0} (#{1})", order.orderid,
                                                order.custpo);
                    mailHelper.SendMail(Settings.Default.FromAccount, string.Join(",", recipients), subject,
                                        body);
                    //ViewBag.message = "OK";
                    //return View("Message");
                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
            }
            ViewBag.message = "Unknown order id";
            return View("Message");
        }
        public ActionResult OrderingPatternsAverageWeek()
        {
            var ordersFrom = DateTime.Today.AddYears(-1);
            var ordersTo = DateTime.Now.AddDays(-1);

            var model = new OrderingPatternsAverage
            {
                OrderingPatterns = orderLinesDAL.GetOrderingPatterns(Settings.Default.CustomersForOrderingPatterns, ordersFrom, ordersTo)

            };

            return View("OrderingPatternsAverageWeek", model);
        }


        public ActionResult ArrivalsReport(string clients = "", string factories = "", DateTime? forDate = null, int months = 6)
        {
            if (forDate == null)
                forDate = DateTime.Today;
            var clientsIds = company.Common.Utilities.GetIdsFromString(clients) ?? new List<int>();
            var factoryIds = company.Common.Utilities.GetIdsFromString(factories) ?? new List<int>();

            var startDate = Utilities.GetFirstDayInWeek(forDate.Value);
            var endDate = startDate.AddMonths(months);

            var useFactories = factoryIds != null && factoryIds.Count > 0;
            var statuses = new[] { "X", "Y" };

            var lines = unitOfWork.OrderLinesRepository.Get(l => clientsIds.Contains(l.Header.userid1 ?? 0)
                    && (!useFactories || factoryIds.Contains(l.Cust_Product.MastProduct.factory_id ?? 0))
                    && l.Header.req_eta >= startDate && l.Header.req_eta < endDate && !statuses.Contains(l.Header.status), includeProperties: "Header, Cust_Product.MastProduct").ToList();
            return View(new ArrivalsReportModel
            {
                StartDate = startDate,
                Clients = string.Join(",", unitOfWork.CompanyRepository.Get(c => clientsIds.Contains(c.user_id)).Select(c => c.customer_code)),
                Factories = useFactories ? string.Join(",", unitOfWork.CompanyRepository.Get(c => factoryIds.Contains(c.user_id)).Select(c => c.factory_code)) : string.Empty,
                ProductTotalsBeforeStartDate = unitOfWork.OrderLinesRepository.GetQuery(l => clientsIds.Contains(l.Header.userid1 ?? 0)
                    && (!useFactories || factoryIds.Contains(l.Cust_Product.MastProduct.factory_id ?? 0))
                    && l.Header.req_eta < startDate && !statuses.Contains(l.Header.status), includeProperties: "Header, Cust_Product.MastProduct")
                    .GroupBy(l => l.cprod_id).Select(g => new { g.Key, Total = g.Sum(l => l.orderqty) }).ToDictionary(t => t.Key, t => t.Total),
                Lines = lines
            });
        }

        public ActionResult ContainerCalculationResult(int orderid, int clientId = 1, int container = 1)
        {
            Containercalculation_order calculations;
            var calculator = new ContainerCalculator(unitOfWork);
            return Json(calculator.Calculate(out calculations, orderid, clientId, container), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContainerCalculation()
        {
            return View();
        }




        public ActionResult OrderAdjustmentReport(double minimum = 10, string excludedClients = "200", string excludedContainers = "3,4,5,7")
        {
            var excludedIds = company.Common.Utilities.GetIdsFromString(excludedClients);
            var pendingOrderLines = orderLinesDAL.GetByExportCriteria(etd_from: DateTime.Today, excludedClients: excludedIds);

            var cprod_ids = pendingOrderLines.Select(l => l.cprod_id).Distinct().ToList();
            var order_ids = pendingOrderLines.Select(l => l.orderid).Distinct().ToList();

            var orders = unitOfWork.OrderRepository.Get(o => order_ids.Contains(o.orderid)).ToList();
            var products = unitOfWork.CustProductRepository.Get(p => cprod_ids.Contains(p.cprod_id), includeProperties: "MastProduct").ToList();

            var result = new List<Tuple<Order_header, double?>>();
            var excludedContainerTypes = company.Common.Utilities.GetIdsFromString(excludedContainers);
            var containerTypes = unitOfWork.ContainerTypeRepository.Get(t => !excludedContainerTypes.Contains(t.container_type_id)).ToList();

            foreach (var o in orders)
            {
                var lines = pendingOrderLines.Where(l => l.orderid == o.orderid).ToList();
                if (lines.Count > 0)
                {
                    lines[0].Header.loading_perc = o.loading_perc;
                    o.po_req_etd = lines[0].Header.po_req_etd;
                    o.Client = new Company { customer_code = lines[0].Header.Client.customer_code };
                }

                foreach (var l in lines)
                {
                    l.Cust_Product = products.FirstOrDefault(p => p.cprod_id == l.cprod_id);
                    if (l.orderqty != l.orig_orderqty)
                    {
                        //recalculate pallets, cartons and units
                        var qty = l.orderqty;
                        if (l.Cust_Product.MastProduct.units_per_pallet_single > 0)
                        {
                            l.pallet_qty = Convert.ToInt32(l.orderqty) / l.Cust_Product.MastProduct.units_per_pallet_single;
                            qty -= l.pallet_qty * l.Cust_Product.MastProduct.units_per_pallet_single;
                        }
                        if (l.Cust_Product.MastProduct.units_per_carton > 0)
                        {
                            l.mc_qty = Convert.ToInt32(qty) / l.Cust_Product.MastProduct.units_per_carton;
                            qty -= l.mc_qty * l.Cust_Product.MastProduct.units_per_carton;
                        }
                        l.unit_qty = Convert.ToInt32(qty);
                    }
                }

                Containercalculation_order calculations;
                ContainerCalculator calculator = new ContainerCalculator(unitOfWork);
                var percentage = calculator.Calculate(out calculations, orderLines: lines, containerTypes: containerTypes);
                if (percentage > 0 && o.loading_perc > 90 && Math.Abs((percentage - o.loading_perc) ?? 0) > minimum)
                    result.Add(new Tuple<Order_header, double?>(o, percentage));
            }
            ViewBag.Data = result;
            ViewBag.Minimum = minimum;

            return View();

        }

        private List<Order_header> GetCOOrdersByCriteria(int cprod_id, DateTime? from = null, DateTime? to = null)
        {

            var excludedStatuses = new[] { "X", "Y" };
            return unitOfWork.OrderLinesRepository.Get(l => l.Header.stock_order == Order_header.StockOrderCalloff &&
                                l.cprod_id == cprod_id && (from == null || l.Header.req_eta >= from) &&
                                (to == null || l.Header.req_eta <= to) && !excludedStatuses.Contains(l.Header.status) && l.orderqty > 0, includeProperties: "Header").ToList().
                                Distinct(new OrderLineByHeaderComparer()).Select(l => l.Header).ToList();

        }

        private List<Order_lines> GetCOLinesByIds(int cprod_id, IList<int> ids)
        {
            var excludedStatuses = new[] { "X", "Y" };
            var lines = unitOfWork.OrderLinesRepository.Get(l => l.cprod_id == cprod_id && l.Header.stock_order == Order_header.StockOrderCalloff && ids.Contains(l.orderid.Value),
               includeProperties: "Header, SOAllocations.StockLine.Header").ToList();
                
                        
            var orderids = new HashSet<int?>();
            foreach (var l in lines)
            {
                foreach (var a in l.SOAllocations)
                {
                    if (a.StockLine.orderid != null)
                        orderids.Add(a.StockLine.orderid.Value);
                }
            }
            //orderid, po_req_etd dictionary
            var po_etds = unitOfWork.POrderLineRepository.Get(l => orderids.Contains(l.OrderLine.orderid), includeProperties: "Header, OrderLine").
                        GroupBy(l => l.OrderLine.orderid).Select(g => new { orderid = g.Key, g.FirstOrDefault().Header.po_req_etd }).Distinct().ToDictionary(o => o.orderid, o => o.po_req_etd);
            var po_balances = unitOfWork.OrderLinesRepository.Get(l => l.cprod_id == cprod_id && orderids.Contains(l.orderid), includeProperties: "Allocations").
                GroupBy(l => l.orderid).Select(g => new { orderid = g.Key, g.FirstOrDefault().Header.custpo, total = g.Sum(l => l.orderqty - l.Allocations.Sum(a => a.alloc_qty)) }).ToDictionary(o => o.orderid);
            foreach (var l in lines)
            {
                foreach (var a in l.SOAllocations)
                {
                    if (a.StockLine != null && a.StockLine.orderid != null && po_etds.ContainsKey(a.StockLine.orderid))
                    {
                        a.StockLine.Header.po_req_etd = po_etds[a.StockLine.orderid];
                        if (po_balances.ContainsKey(a.StockLine.orderid))
                            a.StockLine.Header.Balance = po_balances[a.StockLine.orderid].total;
                    }
                }
            }
            return lines;            
        }

        private List<Order_lines> GetAvailableStockOrderLines(int cprod_id, IList<int> idsToSkip = null, DateTime? from = null, DateTime? to = null)
        {
            
                var excludedStatuses = new[] { "X", "Y" };
                var lines = unitOfWork.OrderLinesRepository.
                Get(l => l.Header.stock_order == 1 && l.cprod_id == cprod_id && !idsToSkip.Contains(l.linenum) && !excludedStatuses.Contains(l.Header.status)
                && (from == null || l.Header.req_eta >= from) &&
                   (to == null || l.Header.req_eta <= to), includeProperties: "Header, Allocations").ToList()
                   .Where(l => l.orderqty - l.Allocations.Sum(a => a.alloc_qty) > 0).ToList();
                var orderids = lines.Select(l => l.orderid).ToList();

                var po_etds = unitOfWork.POrderLineRepository.Get(l => orderids.Contains(l.OrderLine.orderid), includeProperties: "OrderLine,Header").
                                GroupBy(l => l.OrderLine.orderid)
                                .Select(g => new { orderid = g.Key, g.FirstOrDefault().Header.po_req_etd }).Distinct().ToDictionary(o => o.orderid, o => o.po_req_etd);
                foreach (var l in lines)
                {
                    if (po_etds.ContainsKey(l.orderid))
                        l.Header.po_req_etd = po_etds[l.orderid];
                }
                return lines;
            
        }

        public void UpdateAllocations(IList<Stock_order_allocation> allocations)
        {
            
                foreach (var a in allocations)
                {
                    a.date_allocation = DateTime.Now;
                    if (a.unique_link_ref == 0)
                    {
                        unitOfWork.StockOrderAllocationRepository.Insert(a);
                    }

                    else
                    {
                        var oldAlloc =
                            unitOfWork.StockOrderAllocationRepository.Get(al => al.unique_link_ref == a.unique_link_ref).FirstOrDefault();
                        if (oldAlloc != null)
                        {
                            oldAlloc.alloc_qty = a.alloc_qty;
                            oldAlloc.date_allocation = a.date_allocation;
                        }

                    }
                }
                unitOfWork.Save();
            
        }

    }
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }

    public class ContainerCalculator
    {
        private IUnitOfWork unitOfWork;

        public ContainerCalculator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public double? Calculate(out Containercalculation_order calculations, int? orderid = null, int clientId = 1, int container = 1, double fillFactor = 0.9, string bathsExceptions = "902,903,904", string mirrors = "1130,1140,1131,1141", string factories = "14,15", string productsForRestriction = "7742", IList<Order_lines> orderLines = null, List<Container_types> containerTypes = null, bool adjustUsingPreviousCalculations = false, string clientsForFinalAdjustment = "322", double hardcode_multiple = 1.2)
        {
            var lines = orderLines ??
                unitOfWork.OrderLinesRepository.Get(l => l.orderid == orderid,
                    includeProperties: "Cust_Product.MastProduct").ToList();
            var header = orderLines?.FirstOrDefault()?.Header ?? unitOfWork.OrderRepository.GetByID(orderid);
            //var client = unitOfWork.CompanyRepository.GetByID(clientId);
            var container_type_id = header.container_type ?? 0;
            var containerType = containerTypes != null ? containerTypes.FirstOrDefault(ct => ct.container_type_id == container_type_id) : unitOfWork.ContainerTypeRepository.GetByID(container_type_id);

            var clientIds = company.Common.Utilities.GetNullableIntsFromString(clientsForFinalAdjustment);

            //find customer requirement (P = pallets, NP = cartons, Z = either)
            //var cust_loading = client.user_loading;
            double? result = 0;

            List<Containercalculation_order> historicCalculations = null;
            Dictionary<int?, double?> historicCorrectiveFactors = null;
            if (adjustUsingPreviousCalculations)
            {
                var mastIds = lines.Select(l => l.Cust_Product?.cprod_mast).ToList();
                historicCalculations = unitOfWork.ContainerCalculationRepository.Get(c => c.Products.Any(p => mastIds.Contains(p.mast_id)), includeProperties: "Products,ContainerType,Order").ToList();
                historicCorrectiveFactors = historicCalculations.SelectMany(c => c.Products).GroupBy(p => p.mast_id).ToDictionary(g => g.Key, g => g.Where(p => p.CorrectiveFactorPerUnit != 0).Average(p => p.CorrectiveFactorPerUnit));
            }

            calculations = new Containercalculation_order
            {
                orderid = orderid,
                container_type_id = container_type_id,
                fillfactor = fillFactor,
                bathsExceptions = bathsExceptions,
                mirrors = mirrors,
                factories = factories,
                Order = new Order_header { orderid = header.orderid, custpo = header.custpo, req_eta = header.req_eta },
                productsForRestriction = productsForRestriction
            };
            calculations.Products = new List<Containercalculation_order_product>();

            if (containerType != null)
            {
                double? c2baths = 0, c3baths = 0, c2_pallet_sqm = 0, c3_pallet_sqm = 0;
                double? containers_used = 0;

                double? totalUnits = 0;
                double? totalPrice = 0;
                double? totalPallet = 0;
                double? totalExactPallets = 0;
                double? totsqm = 0, totcbm = 0;

                var exceptions = company.Common.Utilities.GetNullableIntsFromString(bathsExceptions);
                var mirrorIds = company.Common.Utilities.GetNullableIntsFromString(mirrors);
                var factoryIds = company.Common.Utilities.GetNullableIntsFromString(factories);

                var cont_area = containerType.usable_sqm ?? containerType.width * 1.0 / 1000 * containerType.length * 1.0 / 1000;
                var cont_cbm = containerType.usable_cbm ?? cont_area * containerType.height * 1.0 / 1000 * fillFactor;

                
                var bath_adjustment = 0;
                var bath_adjustment_tot = 0;
                double? bath_adjustment_total_sqm = 0;

                foreach (var l in lines.Where(li => li.orderqty > 0).OrderByDescending(li => li.linedate))
                {
                    var mastProduct = l.Cust_Product?.MastProduct;

                    var cont_w = containerType.width;
                    var cont_l = containerType.length;
                    var cont_h = containerType.height;


                    double? bath_adjustment_sqm = 0;

                    double? totalsqm = 0;
                    double? totalcbm = 0;
                    
                    double? adjustcbm = 0.0;
                    double? exactpallets = 0;
                    double? grandtotal_cbm = 0;

                    var rowprice = l.RowPrice;
                    var rowqty = l.orderqty;

                    //packed units
                    var ordl_pallets = l.pallet_qty;

                    //unit measures
                    var u_carton = mastProduct?.units_per_carton;
                    var u_pallet = mastProduct?.units_per_pallet_single;
                    var u_gp20p = mastProduct?.units_per_20pallet;
                    var u_gp20np = mastProduct?.units_per_20nopallet;
                    var u_gp40p = mastProduct?.units_per_40pallet_gp;
                    var u_gp40np = mastProduct?.units_per_40nopallet_gp;
                    var u_hq40p = mastProduct?.units_per_40pallet_hc;
                    var u_hq40np = mastProduct?.units_per_40nopallet_hc;

                    //pack dimensions
                    var pack_l = mastProduct?.pack_length;
                    var pack_w = mastProduct?.pack_width;
                    var pack_h = mastProduct?.pack_height;
                    var pack_cbm = (pack_l / 1000) * (pack_w / 1000) * (pack_h / 1000);

                    //pack_loading_ratio adjustment added 21/10/2010
                    pack_cbm = pack_cbm * (mastProduct?.pack_loading_ratio);

                    //master carton dimensions
                    var carton_l = mastProduct?.carton_length;
                    var carton_w = mastProduct?.carton_width;
                    var carton_h = mastProduct?.carton_height;
                    var carton_cbm = (carton_l / 1000) * (carton_w / 1000) * (carton_h / 1000);

                    //pack_loading_ratio adjustment added 21/10/2010
                    carton_cbm = carton_cbm * mastProduct?.pack_loading_ratio;

                    double? pallet_l = 0, pallet_w = 0, pallet_h = 0, pallet_cbm, pallet_sqm = 0;
                    //pallet dimensions
                    if (ordl_pallets > 0)
                    {
                        pallet_l = mastProduct?.pallet_length;
                        pallet_w = mastProduct?.pallet_width;
                        pallet_h = mastProduct?.pallet_height;
                        pallet_sqm = pallet_l / 1000 * pallet_w / 1000;
                        pallet_cbm = pallet_sqm * pallet_h / 1000;
                    }

                    //added code for C class products on pallets under pallet qty
                    if (mastProduct?.pallet_length > 0 && mastProduct?.pack_width > 0)
                    {
                        pallet_l = mastProduct?.pallet_length;
                        pallet_w = mastProduct?.pallet_width;
                    }

                    //make the pallet_length the bigger item
                    if (mastProduct?.pallet_length < mastProduct?.pallet_width)
                    {
                        pallet_w = pallet_l;
                        pallet_l = mastProduct?.pallet_width;
                    }

                    // code from WC
                    if (mastProduct?.category1 == Category1.category1_baths && pallet_l > (containerType.width * 0.65))
                    {
                        pallet_l = containerType.width;

                        //here we will add negative cbm - the space at the end of the bath pallets cannot be used for pallets but can be used for boxes
                        // 21/03/2014 - THIS ADJUSTMENT TO BE REMOVED FOR SEVERAL CG BATHS
                        if (!exceptions.Contains(l.Cust_Product?.cprod_brand_cat))
                        {
                            adjustcbm = -((containerType.width - pallet_l) * pallet_w * cont_h) / 2.0;
                            pallet_l = cont_w;
                            bath_adjustment = 1;
                        }

                    }

                    pallet_sqm = (pallet_l / 1000) * (pallet_w / 1000);

                    // added 30/05/2016 - to allow for loading of burlington furniture pallets
                    if (mastProduct?.category1 == Category1.Furniture && l.pallet_qty > 0)
                    {
                        pallet_sqm *= 1.15;
                    }

                    // calculate number of bulk units (OLD DATA) - TO REMOVE LATER
                    //If u_pallet > 0 and rowqty >= u_pallet then qty_pallet = rowqty / u_pallet end if
                    //  If u_pallet > 0 and rowqty >= u_pallet then qty_pallet2 = rowqty / u_pallet end if
                    //    If u_carton > 0 and rowqty >= u_carton then qty_carton = rowqty / u_carton end if
                    //      qty_carton2 = Cint(qty_carton) - CDbl(qty_carton)
                    //      If qty_carton2< 0 then carton_cbm2 = Cint(qty_carton) * CDbl(carton_cbm) else carton_cbm2 = (Cint(qty_carton) + 1) * CDbl(carton_cbm) end if

                    //NEW CALCULATIONS!
                    var no_of_pallets = l.pallet_qty;
                    var no_of_cartons = l.mc_qty;
                    var no_of_units = l.unit_qty;

                    if (pallet_sqm != 0)
                        totalsqm = pallet_sqm * no_of_pallets;

                    //if pallets are double stacked then half the used sqm
                    if (pallet_h > 0)
                    {
                        if (cont_h / pallet_h > 2.05 && no_of_pallets > 0)
                            totalsqm = totalsqm / 2;
                    }
                    if (carton_cbm != 0)
                        totalcbm = carton_cbm * no_of_cartons;
                    if (pack_cbm != 0)
                        totalcbm += pack_cbm * no_of_units;


                    if (mastProduct?.product_group == "C" && no_of_units > 0 && mastProduct?.category1 == Category1.category1_baths)
                    {
                        totalcbm = 0;
                        totalsqm = pallet_sqm * no_of_pallets;
                        if (mastProduct?.units_per_pallet_single == 2)
                        {
                            c2baths += no_of_units;
                            if (pallet_sqm > c2_pallet_sqm)
                                c2_pallet_sqm = pallet_sqm;
                        }
                        else if (mastProduct?.units_per_pallet_single == 3)
                        {
                            c3baths += no_of_units;
                            if (pallet_sqm > c3_pallet_sqm)
                                c3_pallet_sqm = pallet_sqm;
                        }

                    }

                    //code added to use SQM for special mirrors for part filled pallet
                    if (mirrorIds.Contains(l.cprod_id) && no_of_cartons > 0 && mastProduct?.units_per_pallet_single > 0)
                    {
                        totalcbm = 0;
                        totalsqm = pallet_sqm *
                                   Convert.ToInt32(no_of_cartons / mastProduct?.units_per_pallet_single + 0.5);
                    }

                    grandtotal_cbm += totalcbm;
                    containers_used = grandtotal_cbm / cont_cbm;

                    if (factoryIds.Contains(mastProduct?.factory_id))
                    {
                        if (mastProduct?.units_per_carton == 1 && no_of_cartons > 0)
                        {
                            no_of_units += no_of_cartons;
                        }
                        grandtotal_cbm = 0;
                        totalsqm = 0;
                        if (u_pallet > 0)
                            exactpallets = no_of_pallets + no_of_units * 1.0 / u_pallet;
                        else
                        {
                            exactpallets = no_of_pallets;
                        }
                    }

                    bath_adjustment_tot += bath_adjustment;
                    bath_adjustment_sqm = bath_adjustment * totalsqm;
                    bath_adjustment_total_sqm += bath_adjustment_sqm;

                    totalUnits += rowqty;
                    totalPrice += rowprice;
                    totalPallet += no_of_pallets;
                    totalExactPallets += exactpallets;

                    var factor = adjustUsingPreviousCalculations && historicCorrectiveFactors.ContainsKey(mastProduct.mast_id) ? 1 + historicCorrectiveFactors[mastProduct.mast_id] * l.orderqty : 1;

                    totalsqm = (totalsqm * factor) ?? 0;
                    grandtotal_cbm = (grandtotal_cbm * factor) ?? 0;

                    totsqm += totalsqm;
                    totcbm += grandtotal_cbm;

                    calculations.Products.Add(new Containercalculation_order_product
                    {
                        mast_id = mastProduct.mast_id,
                        MastProduct = new Mast_products { mast_id = mastProduct.mast_id, factory_ref = mastProduct.factory_ref },
                        sqm = totalsqm,
                        cbm = grandtotal_cbm,
                        qty = l.orderqty
                    });
                }

                //we need to add the extra SQM for C baths in mixed pallets

                var c2pallets = 0;
                var c3pallets = 0;

                c2pallets = Convert.ToInt32((c2baths + 0.499) / 2);
                var c2extra = c2baths - (c2pallets * 2);
                if (c2extra > 0)
                    c2pallets += 1;

                c3pallets = Convert.ToInt32((c3baths + 0.332) / 3);
                var c3extra = c3baths - (c3pallets * 3);
                if (c2extra == 1 && c3extra == 1)
                    c3extra = 0;

                if (c3extra > 0)
                    c3pallets += 1;

                totsqm = totsqm + (c2_pallet_sqm * c2pallets) + (c3_pallet_sqm * c3pallets);

                //LETS DETERMINE HOW MANY CONTAINERS ARE USED assuming only max 90% of SQM can be used
                //var sqmcont = totsqm / (cont_area * 0.90);

                var sqmcont2 = totsqm / (cont_area);
                // --- COPIED FROM WC CODE
                var bath_adjustment2 = bath_adjustment_total_sqm / (cont_area);

                /*double? full_sqmcont = 0;
                if (Convert.ToInt32(sqmcont) - sqmcont > 0)
                    full_sqmcont = Convert.ToInt32(sqmcont) - 1;
                else
                    full_sqmcont = Convert.ToInt32(sqmcont);*/
                //var used_sqm = totsqm - (full_sqmcont * 0.90 * cont_area);

                //var bal_sqm = (cont_area * 0.90) - used_sqm;
                //bal_sqm is how many sqm left on the last container

                /*double? average_pallet_sqm = 0;
                if (totalPallet > 0)
                    average_pallet_sqm = totsqm / totalPallet;*/

                //NOW LOOK AT MASTER CARTONS & SINGLE PACKAGES when there are no pallets
                var full_containers1 = totcbm / (cont_cbm);

                double? DPceramic = 0;
                //now look at mixed pallets of DP ceramic
                if (header.container_type == Container_types.Gp20)
                {
                    // for 20 foot container we can have 10 pallets
                    DPceramic = totalExactPallets / 10 * 0.89;
                }
                else
                {
                    //for 40 foot GP we can have 21 pallets
                    DPceramic = totalExactPallets / 21 * 0.89;
                }
                // code for restricting progress of orders where quantity is not between 90% and 110% of container
                //var custProductsIdsRestriction = company.Common.Utilities.GetNullableIntsFromString(productsForRestriction);
                //var hasRestrictedProducts = lines.Any(l => custProductsIdsRestriction.Contains(l.cprod_id));



                //var realperc = sqmcont2 + full_containers1 + DPceramic;
                var perc = (sqmcont2 + full_containers1) - (0.088 * bath_adjustment2) + DPceramic;
                result = perc != null ? Math.Round(perc.Value / 0.925 * 100, 0) : 0;

                //added 26/04/2017 - for JS client we want 80% full to appear as 100% full due to forced pallet loading. (modified 14/06/2017)

                double? hardcode_multiple2 = 1;
                if (clientIds.Contains(header.userid1) && result != null)
                {
                    hardcode_multiple2 = (sqmcont2 + full_containers1 + DPceramic) / (sqmcont2 + full_containers1 * hardcode_multiple + DPceramic) + 0.0001;
                    result = Math.Round(result.Value / (hardcode_multiple2 ?? 1.0), 0);
                }

                calculations.DPceramic = DPceramic;
                calculations.bath_adjustment2 = bath_adjustment2;
                calculations.full_containers1 = full_containers1;
                calculations.sqmcont2 = sqmcont2;
                calculations.hardcode_multiple = hardcode_multiple2;
            }



            return result;
        }

        

    }

}

